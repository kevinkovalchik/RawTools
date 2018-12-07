// Copyright 2018 Kevin Kovalchik & Christopher Hughes
// 
// Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// Kevin Kovalchik and Christopher Hughes do not claim copyright of
// any third-party libraries ditributed with RawTools. All third party
// licenses are provided in accompanying files as outline in the NOTICE.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using System.Collections;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools.Utilities;
using RawTools.Algorithms;
using Serilog;

namespace RawTools.Algorithms.ExtractData
{
    static class Extract
    {
        public static (ScanIndex, PrecursorScanCollection, ScanDependentsCollections) ScanIndicesPrecursorsDependents(IRawFileThreadManager rawFileAccessor)
        {
            Log.Information("Extracting scan indices");
            ConcurrentDictionary<int, ScanData> allScans;
            allScans = new ConcurrentDictionary<int, ScanData>();
            Dictionary<int, ScanData> orphanScans = new Dictionary<int, ScanData>();
            MSOrderType AnalysisOrder;

            ConcurrentBag<int> ms1 = new ConcurrentBag<int>();
            ConcurrentBag<int> ms2 = new ConcurrentBag<int>();
            ConcurrentBag<int> ms3 = new ConcurrentBag<int>();
            ConcurrentBag<int> msAny = new ConcurrentBag<int>();

            ConcurrentDictionary<int, PrecursorScanData> precursorScans = new ConcurrentDictionary<int, PrecursorScanData>();
            ConcurrentDictionary<int, IScanDependents> dependents = new ConcurrentDictionary<int, IScanDependents>();

            

            var staticRawFile = rawFileAccessor.CreateThreadAccessor();
            staticRawFile.SelectMsData();

            // populate the scan indices
            IEnumerable<int> scans = staticRawFile.GetFilteredScanEnumerator(staticRawFile.GetFilterFromString("")); // get all scans

            // get ms order of experiment
            Console.Write("Determing MS analysis order... ");
            AnalysisOrder = (from x in scans select staticRawFile.GetScanEventForScanNumber(x).MSOrder).Max();
            Console.WriteLine("Done!");

            object lockTarget = new object();

            ProgressIndicator P = new ProgressIndicator(scans.Count(), "Extracting scan indices");

            int chunkSize = Constants.MultiThreading.ChunkSize(scans.Count());

            var batches = scans.Chunk(chunkSize);

            Parallel.ForEach(batches, batch =>
            {
                ScanData ms1ScanData;
                ScanData ms2ScanData;
                ScanData ms3ScanData;
                foreach (int scan in batch)
                {
                    //var rawFile = rawFileAccessor.CreateThreadAccessor();

                    using (var rawFile = rawFileAccessor.CreateThreadAccessor())
                    {
                        rawFile.SelectMsData();

                        IScanEvent scanEvent = rawFile.GetScanEventForScanNumber(scan);
                        ms1ScanData = new ScanData();
                        ms2ScanData = new ScanData();
                        ms3ScanData = new ScanData();
                        ms1ScanData.MassAnalyzer = scanEvent.MassAnalyzer;
                        ms1ScanData.MSOrder = scanEvent.MSOrder;

                        if (ms1ScanData.MSOrder == MSOrderType.Ms)
                        {
                            ms1.Add(scan);
                            msAny.Add(scan);

                            var scanDependents = rawFile.GetScanDependents(scan, 4);
                            dependents.TryAdd(scan, scanDependents);

                            // check if the ms1 scan has dependent scans
                            if (scanDependents == null)
                            {
                                // there are no scan dependents
                                ms1ScanData.HasDependents = false;
                                allScans.TryAdd(scan, ms1ScanData);
                                return;
                            }
                            else
                            {
                                ms1ScanData.HasDependents = true;
                                allScans.TryAdd(scan, ms1ScanData);
                            }

                            for (int i = 0; i < scanDependents.ScanDependentDetailArray.Length; i++)
                            {
                                int ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;
                                ms2.Add(ms2Scan);
                                msAny.Add(ms2Scan);

                                if (AnalysisOrder == MSOrderType.Ms2) // it is ms2
                                {
                                    ms2ScanData = new ScanData();
                                    scanEvent = rawFile.GetScanEventForScanNumber(ms2Scan);
                                    ms2ScanData.MassAnalyzer = scanEvent.MassAnalyzer;
                                    ms2ScanData.MSOrder = scanEvent.MSOrder;
                                    ms2ScanData.HasDependents = false;
                                    ms2ScanData.HasPrecursors = true;
                                    allScans.TryAdd(ms2Scan, ms2ScanData);

                                    precursorScans.TryAdd(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                                }
                                else // it is ms3
                                {
                                    var ms2Dependents = rawFile.GetScanDependents(ms2Scan, 4).ScanDependentDetailArray;

                                    ms2ScanData = new ScanData();
                                    scanEvent = rawFile.GetScanEventForScanNumber(ms2Scan);
                                    ms2ScanData.MassAnalyzer = scanEvent.MassAnalyzer;
                                    ms2ScanData.MSOrder = scanEvent.MSOrder;
                                    ms2ScanData.HasPrecursors = true;

                                    if (ms2Dependents.Length != 0) // make sure there is ms3 data
                                    {
                                        int ms3Scan = ms2Dependents[0].ScanIndex;
                                        ms3.Add(ms3Scan);
                                        msAny.Add(ms3Scan);

                                        scanEvent = rawFile.GetScanEventForScanNumber(ms3Scan);
                                        precursorScans.TryAdd(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                                        precursorScans.TryAdd(ms3Scan, new PrecursorScanData(ms3scan: ms3Scan, ms2Scan: ms2Scan, masterScan: scan));
                                        ms2ScanData.HasDependents = true;

                                        ms3ScanData = new ScanData();
                                        ms3ScanData.HasPrecursors = true;
                                        ms3ScanData.MassAnalyzer = scanEvent.MassAnalyzer;
                                        ms3ScanData.MSOrder = scanEvent.MSOrder;
                                        allScans.TryAdd(ms3Scan, ms3ScanData);
                                    }
                                    else
                                    {
                                        // there is no ms3 scan, so we only add the ms2 scan
                                        precursorScans.TryAdd(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                                        ms2ScanData.HasDependents = false;
                                    }
                                    allScans.TryAdd(ms2Scan, ms2ScanData);
                                }
                            }
                        }
                        lock (lockTarget)
                        {
                            P.Update();
                        }
                    }
                }
            });
            P.Done();

            HashSet<int> allKeys = new HashSet<int>(allScans.Keys);

            P = new ProgressIndicator(scans.Count(), "Checking for orphaned scans");
            foreach (int scan in scans)
            {
                if (allKeys.Contains(scan))
                {
                    continue;
                }
                else
                {
                    ScanData scanData = new ScanData();
                    var scanEvent = staticRawFile.GetScanEventForScanNumber(scan);
                    scanData.MassAnalyzer = scanEvent.MassAnalyzer;
                    scanData.MSOrder = scanEvent.MSOrder;
                    orphanScans.Add(scan, scanData);
                }
                P.Update();
            }
            P.Done();

            Console.WriteLine();
            Console.WriteLine("================ Scan indexing report ================");
            Console.WriteLine($"Total scans in file: {staticRawFile.RunHeaderEx.SpectraCount}");
            Console.WriteLine($"Scans linked: {allScans.Count()}");

            Console.WriteLine();
            Console.WriteLine("Orphan scans:");

            if (orphanScans.Count() > 0)
            {
                foreach (var scan in orphanScans)
                {
                    Console.WriteLine($"\tScan: {scan.Key}, MSOrder: {scan.Value.MSOrder}");
                }

                Console.WriteLine("\nThe above scans will not be present in the output data. You should");
                Console.WriteLine("manually check them to ensure they are not critical to you analysis.");
            }
            else
            {
                Console.WriteLine("None!");
            }
            Console.WriteLine();
            if (staticRawFile.RunHeaderEx.SpectraCount == allScans.Count() + orphanScans.Count())
            {
                Console.WriteLine("All scans accounted for!");
            }
            else
            {
                Console.WriteLine($"Number of scans unaccounted for: {staticRawFile.RunHeaderEx.SpectraCount - (allScans.Count() + orphanScans.Count())}");
                Console.WriteLine("If this number is alarming, please contact the RawTools authors\n" +
                    "by posting an issue at:\n" +
                    "https://github.com/kevinkovalchik/RawTools/issues");
            }
            Console.WriteLine("======================================================");
            Console.WriteLine();

            // we need to order the scan enumerators before sending them out
            var orderedMs1 = ms1.ToList();
            var orderedMs2 = ms2.ToList();
            var orderedMs3 = ms3.ToList();
            var orderedMsAny = msAny.ToList();
            orderedMs1.Sort();
            orderedMs2.Sort();
            orderedMs3.Sort();
            orderedMsAny.Sort();

            ScanIndex scanIndex = new ScanIndex();
            scanIndex.allScans = allScans.ConvertToDictionary();
            scanIndex.AnalysisOrder = AnalysisOrder;
            scanIndex.ScanEnumerators.Add(MSOrderType.Any, orderedMsAny.ToArray());
            scanIndex.ScanEnumerators.Add(MSOrderType.Ms, orderedMs1.ToArray());
            scanIndex.ScanEnumerators.Add(MSOrderType.Ms2, orderedMs2.ToArray());
            scanIndex.ScanEnumerators.Add(MSOrderType.Ms3, orderedMs3.ToArray());
            scanIndex.TotalScans = staticRawFile.RunHeaderEx.SpectraCount;

            return (scanIndex, new PrecursorScanCollection(precursorScans), new ScanDependentsCollections(dependents));
        }

        public static ScanIndex ScanIndices(IRawDataPlus rawFile)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            Log.Information("Extracting scan indices");
            Dictionary<int, ScanData> allScans;
            allScans = new Dictionary<int, ScanData>();
            MSOrderType AnalysisOrder;

            List<int> ms1 = new List<int>();
            List<int> ms2 = new List<int>();
            List<int> ms3 = new List<int>();
            List<int> msAny = new List<int>();

            // populate the scan indices
            IEnumerable<int> scans = rawFile.GetFilteredScanEnumerator(rawFile.GetFilterFromString("")); // get all scans

            ProgressIndicator P = new ProgressIndicator(scans.Count(), "Extracting scan indices");

            foreach (int scan in scans)
            {
                IScanEvent scanEvent = rawFile.GetScanEventForScanNumber(scan);
                ScanData scanData = new ScanData();
                scanData.MassAnalyzer = scanEvent.MassAnalyzer;
                scanData.MSOrder = scanEvent.MSOrder;

                allScans.Add(scan, scanData);
                msAny.Add(scan);

                if (allScans[scan].MSOrder == MSOrderType.Ms)
                {
                    ms1.Add(scan);
                }
                if (allScans[scan].MSOrder == MSOrderType.Ms2)
                {
                    ms2.Add(scan);
                }
                if (allScans[scan].MSOrder == MSOrderType.Ms3)
                {
                    ms3.Add(scan);
                }
                P.Update();
            }
            P.Done();

            // determine the msorder of the experiment
            if ((ms1.Count > 0) & (ms2.Count == 0) & (ms3.Count == 0))
            {
                AnalysisOrder = MSOrderType.Ms;
            }
            else
            {
                if ((ms1.Count > 0) & (ms2.Count > 0) & (ms3.Count == 0))
                {
                    AnalysisOrder = MSOrderType.Ms2;
                }
                else
                {
                    AnalysisOrder = MSOrderType.Ms3;
                }
            }

            ScanIndex scanIndex = new ScanIndex();
            scanIndex.allScans = allScans;
            scanIndex.AnalysisOrder = AnalysisOrder;
            scanIndex.ScanEnumerators.Add(MSOrderType.Any, msAny.ToArray());
            scanIndex.ScanEnumerators.Add(MSOrderType.Ms, ms1.ToArray());
            scanIndex.ScanEnumerators.Add(MSOrderType.Ms2, ms2.ToArray());
            scanIndex.ScanEnumerators.Add(MSOrderType.Ms3, ms3.ToArray());

            return scanIndex;
        }
        
        public static (CentroidStreamCollection centroids, SegmentScanCollection segments) MsData(IRawDataPlus rawFile, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            CentroidStreamCollection centroids = new CentroidStreamCollection();
            SegmentScanCollection segments = new SegmentScanCollection();
            var scans = index.allScans;
            //var lockTarget = new object(); // this is so we can keep track of progress in the parallel loop

            ProgressIndicator P = new ProgressIndicator(scans.Count(), "Extracting scan data");
            P.Start();

            foreach(int scan in scans.Keys)
            {
                // first get out the mass spectrum
                if (index.allScans[scan].MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    centroids[scan] = new CentroidStreamData(rawFile.GetCentroidStream(scan, false));
                }
                else
                {
                    segments[scan] = new SegmentedScanData(rawFile.GetSegmentedScanFromScanNumber(scan, null));
                }
                //lock (lockTarget)
                //{
                //    P.Update();
                //}
                P.Update();
            }
            P.Done();

            return (centroids, segments);
        }

        /// <summary>
        /// Gets precursor scans by looking at the scan dependents of the scans. This is for DDA files.
        /// </summary>
        /// <param name="rawFile"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (PrecursorScanCollection PrecursorScans, ScanDependentsCollections ScanDependents)
            DependentsAndPrecursorScansByScanDependents(IRawDataPlus rawFile, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            Log.Information("Extracting scan dependents/precursor scans");
            PrecursorScanCollection precursorScans = new PrecursorScanCollection();
            ScanDependentsCollections dependents = new ScanDependentsCollections();

            int ms2Scan = -1;
            int ms3Scan = -1;
            IEnumerable<int> scans = index.ScanEnumerators[MSOrderType.Ms];
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Indexing linked scan events");
            
            foreach (int scan in scans)
            {
                var scanDependents = rawFile.GetScanDependents(scan, 4);
                dependents[scan] = scanDependents;

                // check if the ms1 scan has dependent scans
                if (scanDependents == null)
                {
                    continue;
                }

                for (int i = 0; i < scanDependents.ScanDependentDetailArray.Length; i++)
                {
                    if (index.AnalysisOrder == MSOrderType.Ms2) // it is ms2
                    {
                        ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;
                        precursorScans[ms2Scan] = new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan);
                    }
                    else // it is ms3
                    {
                        ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;
                        var ms2Dependents = rawFile.GetScanDependents(ms2Scan, 4).ScanDependentDetailArray;

                        if (ms2Dependents.Length != 0) // make sure there is ms3 data
                        {
                            ms3Scan = ms2Dependents[0].ScanIndex;
                            precursorScans[ms2Scan] = new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan);
                            precursorScans[ms3Scan] = new PrecursorScanData(ms3scan: ms3Scan, ms2Scan: ms2Scan, masterScan: scan);
                        }
                        else
                        {
                            // there is no ms3 scan, so we only add the ms2 scan
                            precursorScans[ms2Scan] = new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan);
                        }
                    }
                }
                progress.Update();
            }
            
            progress.Done();

            return (precursorScans, dependents);
        }

        /// <summary>
        /// Gets precursor scans by looking at the scan dependents of the scans. This is for DDA files.
        /// </summary>
        /// <param name="rawFile"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (PrecursorScanCollection PrecursorScans, ScanDependentsCollections ScanDependents)
            DependentsAndPrecursorScansByScanDependentsParallel(IRawFileThreadManager rawFileManager, ScanIndex index)
        {
            //rawFile.SelectInstrument(Device.MS, 1);
            Log.Information("Extracting scan dependents/precursor scans");
            ConcurrentDictionary<int, PrecursorScanData> precursorScans = new ConcurrentDictionary<int, PrecursorScanData>();
            ConcurrentDictionary<int, IScanDependents> dependents = new ConcurrentDictionary<int, IScanDependents>();

            //Dictionary<int, PrecursorScanData> precursorScans = new Dictionary<int, PrecursorScanData>();
            //Dictionary<int, IScanDependents> dependents = new Dictionary<int, IScanDependents>();

            //ConcurrentBag<(int scan, PrecursorScanData data)> precursorBag = new ConcurrentBag<(int scan, PrecursorScanData data)>();
            //ConcurrentBag<(int scan, IScanDependents data)> dependentsBag = new ConcurrentBag<(int scan, IScanDependents data)>();

            var updateProgressLock = new object();
            var addLockTarget = new object();

            IEnumerable<int> scans = index.ScanEnumerators[MSOrderType.Ms];
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Indexing linked scan events");
            
            var batches = scans.Chunk(Constants.MultiThreading.ChunkSize(scans.Count()));

            Parallel.ForEach(batches, Constants.MultiThreading.Options(), batch =>
           {
               int ms2Scan = -1;
               int ms3Scan = -1;
               IScanDependents scanDependents;
               var rawFile = rawFileManager.CreateThreadAccessor();
               rawFile.SelectInstrument(Device.MS, 1);
               foreach (int scan in batch)
               {
                   scanDependents = rawFile.GetScanDependents(scan, 2);
                   dependents.AddOrUpdate(scan, scanDependents, (a, b) => b);

                   // check if the ms1 scan has dependent scans
                   if (scanDependents == null)
                   {
                       return;
                   }

                   for (int i = 0; i < scanDependents.ScanDependentDetailArray.Length; i++)
                   {
                       if (index.AnalysisOrder == MSOrderType.Ms2) // it is ms2
                       {
                           ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;
                           //precursorScans.AddOrUpdate(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan), (a, b) => b);
                           precursorScans.TryAdd(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                       }
                       else // it is ms3
                       {
                           ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;
                           var ms2Dependents = rawFile.GetScanDependents(ms2Scan, 2).ScanDependentDetailArray;

                           if (ms2Dependents.Length != 0) // make sure there is ms3 data
                           {
                               ms3Scan = ms2Dependents[0].ScanIndex;
                               precursorScans.AddOrUpdate(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan), (a, b) => b);
                               precursorScans.AddOrUpdate(ms3Scan, new PrecursorScanData(ms3scan: ms3Scan, ms2Scan: ms2Scan, masterScan: scan), (a, b) => b);
                           }
                           else
                           {
                               // there is no ms3 scan, so we only add the ms2 scan
                               precursorScans.AddOrUpdate(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan), (a, b) => b);
                           }
                       }
                   }
                   lock (updateProgressLock)
                   {
                       progress.Update();
                   }
               }
               
           });

            progress.Done();

            var outScans = new PrecursorScanCollection();
            var outDependents = new ScanDependentsCollections();

            foreach (var item in precursorScans)
            {
                outScans.Add(item.Key, item.Value);
            }
            foreach (var item in dependents)
            {
                outDependents.Add(item.Key, item.Value);
            }

            // check for missing precursor information
            /*
            foreach (int scan in index.ScanEnumerators[MSOrderType.Ms2])
            {
                if (outScans.Keys.Contains(scan))
                {
                    outScans.HasPrecursorData.Add(scan, true);
                }
                else
                {
                    outScans.HasPrecursorData.Add(scan, false);
                }
            }*/

            return (outScans, outDependents);
        }

        /// <summary>
        /// Gets precursor scans directly from the trailer extra. This is intended for DIA files, and by extension only for Ms2 experiments.
        /// </summary>
        /// <param name="rawFile"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static PrecursorScanCollection PrecursorScansByMasterScanMs2Only(IRawDataPlus rawFile, TrailerExtraCollection trailerExtras, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            Log.Information("Extracting scan dependents/precursor scans");
            PrecursorScanCollection precursorScans = new PrecursorScanCollection();

            int ms2Scan = -1;
            int ms3Scan = -1;
            IEnumerable<int> scans = index.ScanEnumerators[MSOrderType.Ms];
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Indexing linked scan events");

            // if it is DIA it must be MS2
            scans = index.ScanEnumerators[MSOrderType.Ms2];
            
            foreach (int scan in scans)
            {
                int masterScan = trailerExtras[scan].MasterScan;

                precursorScans[scan] = new PrecursorScanData(ms2scan: scan, masterScan: masterScan);
            }

            progress.Done();

            return precursorScans;
        }

        public static ScanDependentsCollections ScanDependents(IRawDataPlus rawFile, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            ScanDependentsCollections scanDependents = new ScanDependentsCollections();
            foreach (int scan in index.ScanEnumerators[MSOrderType.Ms])
            {
                scanDependents[scan] = rawFile.GetScanDependents(scan, 4);
            }

            return scanDependents;
        }

        public static PrecursorMassCollection PrecursorMasses(IRawDataPlus rawFile, PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            Log.Information("Extracting precursor masses");

            PrecursorMassCollection precursorMasses = new PrecursorMassCollection();
            var scans = index.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Extracting precursor masses");

            foreach (int scan in scans)
            {
                if (index.allScans[scan].MSOrder == MSOrderType.Ms2)
                {
                    double parent_mass = rawFile.GetScanEventForScanNumber(scan).GetReaction(0).PrecursorMass;
                    precursorMasses[scan] = new PrecursorMassData(trailerExtras[scan].MonoisotopicMZ, parent_mass);
                }
                if (index.allScans[scan].MSOrder == MSOrderType.Ms3)
                {
                    int ms2scan = precursorScans[scan].MS2Scan;
                    double parent_mass = rawFile.GetScanEventForScanNumber(scan).GetReaction(0).PrecursorMass;
                    precursorMasses[scan] = new PrecursorMassData(trailerExtras[ms2scan].MonoisotopicMZ, parent_mass);
                }

                progress.Update();
            }
            progress.Done();

            return precursorMasses;
        }

        private static TrailerExtraData ExtractOneTrailerExtra(IRawDataPlus rawFile, int scan, TrailerExtraIndices indices)
        {
            TrailerExtraData trailerExtra = new TrailerExtraData();
            Double[] spsMasses;

            if (indices.InjectionTime != -1)
            {
                trailerExtra.InjectionTime = Convert.ToDouble(rawFile.GetTrailerExtraValue(scan, indices.InjectionTime));
            }

            if (indices.ChargeState != -1)
            {
                trailerExtra.ChargeState = Convert.ToInt32(rawFile.GetTrailerExtraValue(scan, indices.ChargeState));
            }

            if (indices.MonoisotopicMZ != -1)
            {
                trailerExtra.MonoisotopicMZ = Convert.ToDouble(rawFile.GetTrailerExtraValue(scan, indices.MonoisotopicMZ));
            }

            if (indices.MasterScan != -1)
            {
                try
                {
                    trailerExtra.MasterScan = Convert.ToInt32(rawFile.GetTrailerExtraValue(scan, indices.MasterScan));
                }
                // if that doesn't work the master scan is (hopefully) not applicable, we can leave it at the defaule value of -1
                catch (FormatException)
                {
                }
            }

            if (indices.HCDEnergy != -1)
            {
                var hcd = rawFile.GetTrailerExtraValue(scan, indices.HCDEnergy).ToString();
                if (hcd != "N/A" & hcd != "" & !hcd.All(x => x == ' '))
                {
                    try
                    {
                        trailerExtra.HCDEnergy = Convert.ToDouble(rawFile.GetTrailerExtraValue(scan, indices.HCDEnergy));
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            if (indices.SPSMasses.Count > 2) // if so, this means with have all SPS masses listed individually
            {
                spsMasses = new double[indices.SPSMasses.Count];
                for (int i = 0; i < indices.SPSMasses.Count; i++)
                {
                    spsMasses[i] = Convert.ToDouble(rawFile.GetTrailerExtraValue(scan, indices.SPSMasses[i]));
                }
            }
            if (indices.SPSMasses.Count == 0) // there are no SPS masses
            {
                spsMasses = new double[0];
            }
            else // they are broken into two lists of strings, comma delimited
            {
                char[] delimiter = { ' ', ',' };
                string[] stringsps1 = rawFile.GetTrailerExtraValue(scan, indices.SPSMasses[0]).ToString().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                string[] stringsps2 = rawFile.GetTrailerExtraValue(scan, indices.SPSMasses[1]).ToString().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                double[] sps1 = Array.ConvertAll(stringsps1, Convert.ToDouble);
                double[] sps2 = Array.ConvertAll(stringsps2, Convert.ToDouble);
                spsMasses = new double[sps1.Length + sps2.Length];
                sps1.CopyTo(spsMasses, 0);
                sps2.CopyTo(spsMasses, sps1.Length);
            }
            trailerExtra.SPSMasses = spsMasses;
            return trailerExtra;
        }

        public static TrailerExtraCollection TrailerExtras(IRawDataPlus rawFile, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            Log.Information("Extracting trailer extras");
            TrailerExtraCollection trailerExtras = new TrailerExtraCollection();
            TrailerExtraIndices trailerIndices = new TrailerExtraIndices(rawFile);
            Double[] spsMasses;
            var scans = index.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator P = new ProgressIndicator(scans.Count(), "Extracting trailer extras");
            P.Start();

            foreach (int scan in scans)
            {
                trailerExtras[scan] = ExtractOneTrailerExtra(rawFile, scan, trailerIndices);
                P.Update();
            }
            P.Done();

            return trailerExtras;
        }

        public static RetentionTimeCollection RetentionTimes(IRawDataPlus rawFile, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            Log.Information("Extracting retention times");

            RetentionTimeCollection retentionTimes = new RetentionTimeCollection();
            var scans = index.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Extracting retention times");
            foreach (int scan in scans)
            {
                retentionTimes[scan] = rawFile.RetentionTimeFromScanNumber(scan);

                progress.Update();
            }

            progress.Done();

            return retentionTimes;
        }

        public static ScanEventReactionCollection ScanEvents(IRawDataPlus rawFile, ScanIndex index)
        {
            rawFile.SelectMsData();

            ScanEventReactionCollection events = new ScanEventReactionCollection();

            Log.Information("Extracting scan events");
            ProgressIndicator P = new ProgressIndicator(index.ScanEnumerators[MSOrderType.Any].Length, "Extracting reaction events");
            P.Start();

            foreach (int scan in index.ScanEnumerators[index.AnalysisOrder])
            {
                events.Add(scan, rawFile.GetScanEventForScanNumber(scan).GetReaction(0));
                P.Update();
            }
            P.Done();

            return events;
        }

        public static MethodDataContainer MethodData(IRawDataPlus rawFile, ScanIndex index)
        {
            rawFile.SelectInstrument(Device.MS, 1);

            Log.Information("Extracting method/instrument information");

            MethodDataContainer methodData = new MethodDataContainer();

            methodData.CreationDate = rawFile.CreationDate;
            methodData.Instrument = rawFile.GetInstrumentData().Name;

            methodData.AnalysisOrder = index.AnalysisOrder;
            int firstQuantScan = index.ScanEnumerators[index.AnalysisOrder][0];
            methodData.QuantAnalyzer = index.allScans[firstQuantScan].MassAnalyzer;

            int firstMs1Scan = index.ScanEnumerators[MSOrderType.Ms][0];
            methodData.MassAnalyzers.Add(MSOrderType.Ms, index.allScans[firstMs1Scan].MassAnalyzer);

            int firstMs2Scan = index.ScanEnumerators[MSOrderType.Ms2][0];
            methodData.MassAnalyzers.Add(MSOrderType.Ms2, index.allScans[firstMs2Scan].MassAnalyzer);

            methodData.MSOrderEnumerator.Add(MSOrderType.Ms);
            methodData.MSOrderEnumerator.Add(MSOrderType.Ms2);

            int n = index.ScanEnumerators[MSOrderType.Ms2].Count();
            double ms2window = rawFile.GetScanEventForScanNumber(index.ScanEnumerators[MSOrderType.Ms2][n / 2]).GetIsolationWidth(0);

            if (methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                int firstMs3Scan = index.ScanEnumerators[MSOrderType.Ms3][0];
                methodData.MassAnalyzers.Add(MSOrderType.Ms3, index.allScans[firstMs3Scan].MassAnalyzer);
                methodData.MSOrderEnumerator.Add(MSOrderType.Ms3);
                double ms3ms1window = rawFile.GetScanEventForScanNumber(firstMs3Scan).GetIsolationWidth(0);
                double ms3ms2window = rawFile.GetScanEventForScanNumber(firstMs3Scan).GetIsolationWidth(1);

                methodData.IsolationWindow = (MS2: ms2window, (MS1Window: ms3ms1window, MS2Window: ms3ms2window));
            }
            else
            {
                methodData.IsolationWindow = (MS2: ms2window, (MS1Window: -1, MS2Window: -1));
            }

            return methodData;
        }
    }
}
