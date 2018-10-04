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
        public static ScanIndex ScanIndices(IRawDataPlus rawFile)
        {
            Log.Information("Extracting scan indices");
            Dictionary<int, (MSOrderType MSOrder, MassAnalyzerType MassAnalyzer)> allScans;
            allScans = new Dictionary<int, (MSOrderType MSOrder, MassAnalyzerType MassAnalyzer)>();
            MSOrderType AnalysisOrder;

            List<int> ms1 = new List<int>();
            List<int> ms2 = new List<int>();
            List<int> ms3 = new List<int>();
            List<int> msAny = new List<int>();

            // populate the scan indices
            IEnumerable<int> scans = rawFile.GetFilteredScanEnumerator(rawFile.GetFilterFromString("")); // get all scans

            foreach (int scan in scans)
            {
                IScanEvent scanEvent = rawFile.GetScanEventForScanNumber(scan);

                allScans.Add(scan, (scanEvent.MSOrder, scanEvent.MassAnalyzer));
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
            }

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
            CentroidStreamCollection centroids = new CentroidStreamCollection();
            SegmentScanCollection segments = new SegmentScanCollection();
            var scans = index.allScans;

            ProgressIndicator P = new ProgressIndicator(scans.Count(), "Extracting scan data");
            P.Start();

            foreach (int scan in scans.Keys)
            {
                // first get out the mass spectrum
                if (index.allScans[scan].MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    centroids.Add(scan, new CentroidStreamData(rawFile.GetCentroidStream(scan, false)));
                }
                else
                {
                    segments.Add(scan, new SegmentedScanData(rawFile.GetSegmentedScanFromScanNumber(scan, null)));
                }
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
        public static PrecursorScanCollection PrecursorScansByScanDependents(IRawDataPlus rawFile, ScanIndex index)
        {
            Log.Information("Extracting scan dependents/precursor scans");
            PrecursorScanCollection precursorScans = new PrecursorScanCollection();

            int ms2Scan = -1;
            int ms3Scan = -1;
            IEnumerable<int> scans = index.ScanEnumerators[MSOrderType.Ms];
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Indexing linked scan events");
            foreach (int scan in scans)
            {
                var scanDependents = rawFile.GetScanDependents(scan, 4);

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
                        precursorScans.Add(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                    }
                    else // it is ms3
                    {
                        ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;

                        if (rawFile.GetScanDependents(ms2Scan, 4).ScanDependentDetailArray.Length != 0) // make sure there is ms3 data
                        {
                            ms3Scan = rawFile.GetScanDependents(ms2Scan, 4).ScanDependentDetailArray[0].ScanIndex;
                            precursorScans.Add(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                            precursorScans.Add(ms3Scan, new PrecursorScanData(ms3scan: ms3Scan, ms2Scan: ms2Scan, masterScan: scan));
                        }
                        else
                        {
                            // there is no ms3 scan, so we only add the ms2 scan
                            precursorScans.Add(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                        }
                    }
                }
                progress.Update();
            }
            
            progress.Done();

            return precursorScans;
        }

        /// <summary>
        /// Gets precursor scans directly from the trailer extra. This is intended for DIA files, and by extension only for Ms2 experiments.
        /// </summary>
        /// <param name="rawFile"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static PrecursorScanCollection PrecursorScansByMasterScanMs2Only(IRawDataPlus rawFile, TrailerExtraCollection trailerExtras, ScanIndex index)
        {
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

                precursorScans.Add(scan, new PrecursorScanData(ms2scan: scan, masterScan: masterScan));
            }

            progress.Done();

            return precursorScans;
        }

        public static ScanDependentsCollections ScanDependents(IRawDataPlus rawFile, ScanIndex index)
        {
            ScanDependentsCollections scanDependents = new ScanDependentsCollections();
            foreach (int scan in index.ScanEnumerators[MSOrderType.Any])
            {
                scanDependents.Add(scan, rawFile.GetScanDependents(scan, 4));
            }

            return scanDependents;
        }

        public static PrecursorMassCollection PrecursorMasses(IRawDataPlus rawFile, PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, ScanIndex index)
        {
            Log.Information("Extracting precursor masses");

            PrecursorMassCollection precursorMasses = new PrecursorMassCollection();
            var scans = index.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Extracting precursor masses");

            foreach (int scan in scans)
            {
                if (index.allScans[scan].MSOrder == MSOrderType.Ms2)
                {
                    double parent_mass = rawFile.GetScanEventForScanNumber(scan).GetReaction(0).PrecursorMass;
                    precursorMasses.Add(scan, new PrecursorMassData(trailerExtras[scan].MonoisotopicMZ, parent_mass));
                }
                if (index.allScans[scan].MSOrder == MSOrderType.Ms3)
                {
                    int ms2scan = precursorScans[scan].MS2Scan;
                    double parent_mass = rawFile.GetScanEventForScanNumber(scan).GetReaction(0).PrecursorMass;
                    precursorMasses.Add(scan, new PrecursorMassData(trailerExtras[ms2scan].MonoisotopicMZ, parent_mass));
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
                try
                {
                    trailerExtra.HCDEnergy = Convert.ToDouble(rawFile.GetTrailerExtraValue(scan, indices.HCDEnergy));
                }
                catch (FormatException)
                {
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
            Log.Information("Extracting trailer extras");
            TrailerExtraCollection trailerExtras = new TrailerExtraCollection();
            TrailerExtraIndices trailerIndices = new TrailerExtraIndices(rawFile);
            Double[] spsMasses;
            var scans = index.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator P = new ProgressIndicator(scans.Count(), "Extracting trailer extras");
            P.Start();

            foreach (int scan in scans)
            {
                trailerExtras.Add(scan, ExtractOneTrailerExtra(rawFile, scan, trailerIndices));
                P.Update();
            }
            P.Done();

            return trailerExtras;
        }

        public static RetentionTimeCollection RetentionTimes(IRawDataPlus rawFile, ScanIndex index)
        {
            Log.Information("Extracting retention times");

            RetentionTimeCollection retentionTimes = new RetentionTimeCollection();
            var scans = index.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Extracting retention times");
            foreach (int scan in scans)
            {
                retentionTimes.Add(scan, rawFile.RetentionTimeFromScanNumber(scan));

                progress.Update();
            }

            progress.Done();

            return retentionTimes;
        }


        public static MethodDataContainer MethodData(IRawDataPlus rawFile, ScanIndex index)
        {
            Log.Information("Extracting method/instrument information");

            MethodDataContainer methodData = new MethodDataContainer();

            methodData.CreationDate = rawFile.CreationDate;

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
