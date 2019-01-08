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
using RawTools.Utilities.ScanData;
using RawTools.Data.Processing;
using RawTools.Algorithms;
using Serilog;

namespace RawTools.Data.Extraction
{/*
    NOTE: This is empty! It is here for reference in case we need it during refactoring.

    class XIC
    {
        IRawDataPlus rawFile;
        string filterMS;
        double mass, ppm;

        public XIC(IRawDataPlus file, string filter, double ionMass, double ppmTolerance)
        {
            rawFile = file;
            filterMS = filter;
            ppm = ppmTolerance;
            mass = ionMass;
        }

        public double[][] Data()
        {

            double[][] data = new double[2][];

            // next make a request for the filtered chromatogram.
            // Define settings for XIC
            ChromatogramTraceSettings traceSettings =
            new ChromatogramTraceSettings(TraceType.MassRange)
            {
                Filter = filterMS,
                MassRanges = new[] { new Range(mass, mass) }
            };

            // create the array of chromatogram settings
            IChromatogramSettings[] allSettings = { traceSettings };

            // set tolerance of +/- whatever ppm
            MassOptions tolerance = new MassOptions() { Tolerance = ppm, ToleranceUnits = ToleranceUnits.ppm };

            // read the chromatogram
            var chroData = rawFile.GetChromatogramData(allSettings, -1, -1, tolerance);

            data[0] = chroData.PositionsArray.ToArray()[0];
            data[1] = chroData.IntensitiesArray.ToArray()[0];

            return data;
        }
    }

    static class SegmentedScans
    {
        public static void ExtractSegmentScans(this RawDataCollection rawData, IRawDataPlus rawFile, IEnumerable<int> scans)
        {
            Log.Information("Extracting custom defined collection of segment scans");
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), string.Format("Extracting custom collection of segment scans"));
            Extract(rawData, rawFile, scans, progress);
            progress.Done();
        }

        public static void ExtractSegmentScans(this RawDataCollection rawData, IRawDataPlus rawFile, MSOrderType MSOrder)
        {
            if (MSOrder == MSOrderType.Ms)
            {
                if (rawData.Performed.Contains(Operations.Ms1SegmentedScans))
                {
                    return;
                }
            }
            else
            {
                if (MSOrder == MSOrderType.Ms2)
                {
                    if (rawData.Performed.Contains(Operations.Ms2SegmentedScans))
                    {
                        return;
                    }
                }
                else
                {
                    if (rawData.Performed.Contains(Operations.Ms3SegmentedScans))
                    {
                        return;
                    }
                }
            }

            Log.Information("Extracting {MSOrder} segment scans", MSOrder);
            IEnumerable<int> scans = rawData.scanIndex.ScanEnumerators[MSOrder];
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), string.Format("Extracting {0} segment scans", MSOrder));
            try
            {
                rawData.Extract(rawFile, scans, progress);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed while extracting {MSOrder} segment scans", MSOrder);
                throw e;
            }
            
            progress.Done();

            if (MSOrder == MSOrderType.Ms)
            {
                rawData.Performed.Add(Operations.Ms1SegmentedScans);
            }
            else
            {
                if (MSOrder == MSOrderType.Ms2)
                {
                    rawData.Performed.Add(Operations.Ms2SegmentedScans);
                }
                else
                {
                    rawData.Performed.Add(Operations.Ms3SegmentedScans);
                }
            }
            
        }

        private static void Extract(this RawDataCollection rawData, IRawDataPlus rawFile, IEnumerable<int> scans, ProgressIndicator progress)
        {
            foreach (int scan in scans)
            {
                try
                {
                    rawData.segmentedScans.Add(scan, new SegmentedScanData(rawFile.GetSegmentedScanFromScanNumber(scan, null)));
                }
                catch(Exception e)
                {
                    Log.Error(e, "Failed during extraction of segment scan {Scan}", scan);
                    throw e;
                }
                progress.Update();
            }
        }
    }

    static class CentroidStreams
    {
        public static void ExtractCentroidStreams(this RawDataCollection rawData, IRawDataPlus rawFile, IEnumerable<int> scans)
        {
            Log.Information("Extracting custom defined collection of centroid streams");
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), string.Format("Extracting custom collection of centroid streams"));
            Extract(rawData, rawFile, scans, progress);
            progress.Done();
        }

        public static void ExtractCentroidStreams(this RawDataCollection rawData, IRawDataPlus rawFile, MSOrderType MSOrder)
        {
            if (MSOrder == MSOrderType.Ms)
            {
                if (rawData.Performed.Contains(Operations.Ms1CentroidStreams))
                {
                    return;
                }
            }
            else
            {
                if (MSOrder == MSOrderType.Ms2)
                {
                    if (rawData.Performed.Contains(Operations.Ms2CentroidStreams))
                    {
                        return;
                    }
                }
                else
                {
                    if (rawData.Performed.Contains(Operations.Ms3CentroidStreams))
                    {
                        return;
                    }
                }
            }

            Log.Information("Extracting {MSOrder} centroid streams", MSOrder);
            IEnumerable<int> scans = rawData.scanIndex.ScanEnumerators[MSOrder];
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), string.Format("Extracting {0} centroid streams", MSOrder));
            try
            {
                rawData.Extract(rawFile, scans, progress);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed during extraction of {MSOrder} centroid streams", MSOrder);
                throw e;
            }
            
            progress.Done();

            if (MSOrder == MSOrderType.Ms)
            {
                rawData.Performed.Add(Operations.Ms1CentroidStreams);
            }
            else
            {
                if (MSOrder == MSOrderType.Ms2)
                {
                    rawData.Performed.Add(Operations.Ms2CentroidStreams);
                }
                else
                {
                    rawData.Performed.Add(Operations.Ms3CentroidStreams);
                }
            }
        }

        private static void Extract(this RawDataCollection rawData, IRawDataPlus rawFile, IEnumerable<int> scans, ProgressIndicator progress)
        {
            foreach (int scan in scans)
            {
                try
                {
                    rawData.centroidStreams.Add(scan, new CentroidStreamData(rawFile.GetCentroidStream(scan, false)));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed during extraction of centroid stream {Scan}", scan);
                    throw e;
                }
                
                progress.Update();
            }
        }
    }
    
    static class TrailerExtras
    {
        public static TrailerExtraData ExtractTrailerExtra(RawDataCollection rawData, IRawDataPlus rawFile, int scan, TrailerExtraIndices indices)
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

        public static void ExtractTrailerExtra(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            if (rawData.Performed.Contains(Operations.TrailerExtras))
            {
                return;
            }

            Log.Information("Extracting trailer extras");
            rawData.trailerExtras = new Dictionary<int, TrailerExtraData>();
            TrailerExtraIndices indices = new TrailerExtraIndices(rawFile);
            Double[] spsMasses;
            IEnumerable<int> scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Extracting trailer extra data");

            foreach (int scan in scans)
            {
                try
                {
                    rawData.trailerExtras.Add(scan, ExtractTrailerExtra(rawData, rawFile, scan, indices));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed during extraction of trailer extra {Scan}", scan);
                    throw e;
                }

                progress.Update();
            }
            progress.Done();

            rawData.Performed.Add(Operations.TrailerExtras);
        }
    }

    static class ScanIndices
    {
        public static void ExtractScanIndex(this RawDataCollection rawData, IRawDataPlus rawFile)
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

            rawData.scanIndex = new ScanIndex();
            rawData.scanIndex.allScans = allScans;
            rawData.scanIndex.AnalysisOrder = AnalysisOrder;
            rawData.scanIndex.ScanEnumerators.Add(MSOrderType.Any, msAny.ToArray());
            rawData.scanIndex.ScanEnumerators.Add(MSOrderType.Ms, ms1.ToArray());
            rawData.scanIndex.ScanEnumerators.Add(MSOrderType.Ms2, ms2.ToArray());
            rawData.scanIndex.ScanEnumerators.Add(MSOrderType.Ms3, ms3.ToArray());

            // we need to check if it is a boxcar file because those have some scan index issues
            bool isBoxCar = rawFile.GetScanEventForScanNumber(1).MassRangeCount > 1;

            rawData.Performed.Add(Operations.ScanIndex);

            if (isBoxCar)
            {
                Log.Information("Raw file looks like a boxcar run. Scan indices being adjusted to account for missing scan dependents.");
                rawData.ExtractPrecursorScans(rawFile);
                rawData.scanIndex.ScanEnumerators[rawData.scanIndex.AnalysisOrder] = rawData.precursorScans.Keys.ToArray();
            }
        }
    }

    static class MethodDataExtract
    {
        public static void ExtractMethodData(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            if (rawData.Performed.Contains(Operations.MethodData))
            {
                return;
            }
            if (!rawData.Performed.Contains(Operations.ScanIndex))
            {
                rawData.ExtractScanIndex(rawFile);
            }

            Log.Information("Extracting method/instrument information");

            rawData.methodData = new Containers.MethodDataContainer();

            rawData.methodData.AnalysisOrder = rawData.scanIndex.AnalysisOrder;
            int firstQuantScan = rawData.scanIndex.ScanEnumerators[rawData.scanIndex.AnalysisOrder][0];
            rawData.methodData.QuantAnalyzer = rawData.scanIndex.allScans[firstQuantScan].MassAnalyzer;

            int firstMs1Scan = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms][0];
            rawData.methodData.MassAnalyzers.Add(MSOrderType.Ms, rawData.scanIndex.allScans[firstMs1Scan].MassAnalyzer);

            int firstMs2Scan = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2][0];
            rawData.methodData.MassAnalyzers.Add(MSOrderType.Ms2, rawData.scanIndex.allScans[firstMs2Scan].MassAnalyzer);

            rawData.methodData.MSOrderEnumerator.Add(MSOrderType.Ms);
            rawData.methodData.MSOrderEnumerator.Add(MSOrderType.Ms2);

            int n = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2].Count();
            double ms2window = rawFile.GetScanEventForScanNumber(rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2][n / 2]).GetIsolationWidth(0);

            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                int firstMs3Scan = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms3][0];
                rawData.methodData.MassAnalyzers.Add(MSOrderType.Ms3, rawData.scanIndex.allScans[firstMs3Scan].MassAnalyzer);
                rawData.methodData.MSOrderEnumerator.Add(MSOrderType.Ms3);
                double ms3ms1window = rawFile.GetScanEventForScanNumber(firstMs3Scan).GetIsolationWidth(0);
                double ms3ms2window = rawFile.GetScanEventForScanNumber(firstMs3Scan).GetIsolationWidth(1);

                rawData.methodData.IsolationWindow = (MS2: ms2window, (MS1Window: ms3ms1window, MS2Window: ms3ms2window));
            }
            else
            {
                rawData.methodData.IsolationWindow = (MS2: ms2window, (MS1Window: -1, MS2Window: -1));
            }
            rawData.Performed.Add(Operations.MethodData);
        }
    }

    static class PrecursorScans
    {
        public static void ExtractPrecursorScans(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            if (rawData.Performed.Contains(Operations.PrecursorScans))
            {
                return;
            }
            if (!rawData.Performed.Contains(Operations.ScanIndex))
            {
                rawData.ExtractScanIndex(rawFile);
            }

            Log.Information("Extracting scan dependents/precursor scans");

            int ms2Scan = -1;
            int ms3Scan = -1;
            IEnumerable<int> scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms];
            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Indexing linked scan events");
            if (rawData.ExpType == ExperimentType.DDA)
            {
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
                        if (rawData.scanIndex.AnalysisOrder == MSOrderType.Ms2) // it is ms2
                        {
                            ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;
                            rawData.precursorScans.Add(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                        }
                        else // it is ms3
                        {
                            ms2Scan = scanDependents.ScanDependentDetailArray[i].ScanIndex;

                            if (rawFile.GetScanDependents(ms2Scan, 4).ScanDependentDetailArray.Length != 0) // make sure there is ms3 data
                            {
                                ms3Scan = rawFile.GetScanDependents(ms2Scan, 4).ScanDependentDetailArray[0].ScanIndex;
                                rawData.precursorScans.Add(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                                rawData.precursorScans.Add(ms3Scan, new PrecursorScanData(ms3scan: ms3Scan, ms2Scan: ms2Scan, masterScan: scan));
                            }
                            else
                            {
                                // there is no ms3 scan, so we only add the ms2 scan
                                rawData.precursorScans.Add(ms2Scan, new PrecursorScanData(ms2scan: ms2Scan, masterScan: scan));
                            }
                        }
                    }
                    progress.Update();
                }
            }
            if (rawData.ExpType == ExperimentType.DIA)
            {
                // if it is DIA it must be MS2
                scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2];

                // we need the trailer extra
                CheckIfDone.Check(rawData, rawFile, new List<Operations> { Operations.TrailerExtras });
                foreach (int scan in scans)
                {
                    int masterScan = rawData.trailerExtras[scan].MasterScan;

                    rawData.precursorScans.Add(scan, new PrecursorScanData(ms2scan: scan, masterScan: masterScan));
                }
            }
            if (rawData.ExpType == ExperimentType.PRM)
            {
                // if it is PRM there are ONLY ms2 scans, so we don't actually care about precursor scans
            }
            
            progress.Done();
            rawData.Performed.Add(Operations.PrecursorScans);
        }
    }

    static class RetentionTimes
    {
        public static void ExtractRetentionTimes(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            if (rawData.Performed.Contains(Operations.RetentionTimes))
            {
                return;
            }
            if (!rawData.Performed.Contains(Operations.ScanIndex))
            {
                rawData.ExtractScanIndex(rawFile);
            }

            Log.Information("Extracting retention times");

            rawData.retentionTimes = new Dictionary<int, double>();
            IEnumerable<int> scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Extracting retention times");
            foreach (int scan in scans)
            {
                try
                {
                    rawData.retentionTimes.Add(scan, rawFile.RetentionTimeFromScanNumber(scan));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed during extraction of retention time for scan {Scan}", scan);
                    throw e;
                }
                progress.Update();
            }
            progress.Done();

            rawData.Performed.Add(Operations.RetentionTimes);
        }
    }

    static class PrecursorMasses
    {
        public static void ExtractPrecursorMasses(this RawDataCollection rawData, IRawDataPlus rawFile, int scan)
        {
            if (rawData.scanIndex.allScans[scan].MSOrder == MSOrderType.Ms2)
            {
                double parent_mass = rawFile.GetScanEventForScanNumber(scan).GetReaction(0).PrecursorMass;
                rawData.precursorMasses.Add(scan, new PrecursorMassData(rawData.trailerExtras[scan].MonoisotopicMZ, parent_mass));
            }
            if (rawData.scanIndex.allScans[scan].MSOrder == MSOrderType.Ms3)
            {
                int ms2scan = rawData.precursorScans[scan].MS2Scan;
                double parent_mass = rawFile.GetScanEventForScanNumber(scan).GetReaction(0).PrecursorMass;
                rawData.precursorMasses.Add(scan, new PrecursorMassData(rawData.trailerExtras[ms2scan].MonoisotopicMZ, parent_mass));
            }
        }

        public static void ExtractPrecursorMasses(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            if (rawData.Performed.Contains(Operations.PrecursorMasses))
            {
                return;
            }

            CheckIfDone.Check(rawData, rawFile, new List<Operations> { Operations.ScanIndex, Operations.TrailerExtras, Operations.PrecursorScans });

            Log.Information("Extracting precursor masses");

            rawData.precursorMasses = new Dictionary<int, PrecursorMassData>();
            IEnumerable<int> scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Any];

            ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Extracting precursor masses");
            foreach (int scan in scans)
            {
                try
                {
                    ExtractPrecursorMasses(rawData, rawFile, scan);

                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed during extraction of precursor mass for scan {Scan}", scan);
                    throw e;
                }
                progress.Update();
            }
            progress.Done();
            rawData.Performed.Add(Operations.PrecursorMasses);
        }
    }

    static class AllData
    {
        public static void ExtractAll(this RawDataCollection rawData, IRawDataPlus rawFile, bool refineMassCharge = true)
        {
            Log.Information("Beginning extraction of all possible data");
            rawFile.SelectInstrument(Device.MS, 1);
            rawData.ExtractPrecursorScans(rawFile);
            ProgressIndicator P = new ProgressIndicator(rawData.scanIndex.allScans.Count(), "Extracting raw data");
            TrailerExtraIndices indices = new TrailerExtraIndices(rawFile);
            for (int i = 1; i <= rawData.scanIndex.allScans.Count(); i++)
            {
                try
                {
                    // first get out the mass spectrum
                    if (rawData.scanIndex.allScans[i].MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        rawData.centroidStreams.Add(i, new CentroidStreamData(rawFile.GetCentroidStream(i, false)));
                    }
                    else
                    {
                        rawData.segmentedScans.Add(i, new SegmentedScanData(rawFile.GetSegmentedScanFromScanNumber(i, null)));
                    }

                    // add the trailer extra data
                    if (!rawData.Performed.Contains(Operations.TrailerExtras))
                    {
                        rawData.trailerExtras.Add(i, TrailerExtras.ExtractTrailerExtra(rawData, rawFile, i, indices));
                    }

                    // add the retention time
                    rawData.retentionTimes.Add(i, rawFile.RetentionTimeFromScanNumber(i));
                    rawData.Performed.Add(Operations.RetentionTimes);

                    // add the precursor mass
                    PrecursorMasses.ExtractPrecursorMasses(rawData, rawFile, i);

                    P.Update();
                }
                catch (Exception e)
                {
                    Log.Error("Extraction failed on scan {Scan}", i);
                    throw e;
                }
            }
            P.Done();
            rawData.Performed.Add(Operations.TrailerExtras);
            rawData.Performed.Add(Operations.RetentionTimes);
            rawData.Performed.Add(Operations.PrecursorMasses);

            if (rawData.refineMassCharge)
            {
                P = new ProgressIndicator(rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2].Length, "Refining precursor charge state and monoisotopic mass");
                P.Start();
                int refinedCharge;
                double refinedMass;


                foreach (int scan in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2])
                {
                    // refine precursor mass and charge
                    (refinedCharge, refinedMass) =
                        MonoIsoPredictor.GetMonoIsotopicMassCharge(rawData.centroidStreams[rawData.precursorScans[scan].MasterScan],
                        rawData.precursorMasses[scan].ParentMZ, rawData.trailerExtras[scan].ChargeState);
                    rawData.trailerExtras[scan].ChargeState = refinedCharge;
                    rawData.precursorMasses[scan].MonoisotopicMZ = refinedMass;
                    P.Update();
                }
                P.Done();
            }
            

            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms2 | rawData.methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                rawData.Performed.Add(Operations.Ms1CentroidStreams);
                if (rawData.methodData.MassAnalyzers[MSOrderType.Ms2] == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    rawData.Performed.Add(Operations.Ms2CentroidStreams);
                }
                else
                {
                    rawData.Performed.Add(Operations.Ms2SegmentedScans);
                }
            }
            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                if (rawData.methodData.MassAnalyzers[MSOrderType.Ms3] == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    rawData.Performed.Add(Operations.Ms3CentroidStreams);
                }
                else
                {
                    rawData.Performed.Add(Operations.Ms3SegmentedScans);
                }
            }
            

            if (refineMassCharge)
            {
                P = new ProgressIndicator(rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2].Length, "Refining precursor charge state and monoisotopic mass");
                P.Start();
                int refinedCharge;
                double refinedMass;

                //if (!rawData.Performed.Contains(Operations.PeakRetAndInt))
                //{
                //    rawData.CalcPeakRetTimesAndInts(rawFile);
                //}

                foreach (int scan in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2])
                {
                    // refine precursor mass and charge
                    // var centroid = rawData.GetAverageScan(rawFile, scan);
                    //rawData.centroidStreams[rawData.precursorScans[scan].MasterScan]

                    (refinedCharge, refinedMass) =
                        MonoIsoPredictor.GetMonoIsotopicMassCharge(rawData.centroidStreams[rawData.precursorScans[scan].MasterScan],
                        rawData.precursorMasses[scan].ParentMZ, rawData.trailerExtras[scan].ChargeState);
                    rawData.trailerExtras[scan].ChargeState = refinedCharge;
                    rawData.precursorMasses[scan].MonoisotopicMZ = refinedMass;
                    P.Update();
                }
                P.Done();
            }
        }
    }
    */
}
