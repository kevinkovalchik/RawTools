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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using System.Collections;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools.Data.Extraction;
using RawTools.Utilities;
using RawTools.Algorithms;
using RawTools.WorkFlows;

namespace RawTools.Algorithms.Analyze
{
    static class MetaDataCalculations
    {

        public static double Ms1IsoWindow(MethodDataContainer methodData)
        {
            double isoWindow;
            if (methodData.AnalysisOrder == MSOrderType.Ms2)
            {
                isoWindow = methodData.IsolationWindow.MS2;
            }
            else
            {
                isoWindow = methodData.IsolationWindow.MS3.MS1Window;
            }
            return isoWindow;
        }

        public static Dictionary<int, int> MS2ScansPerCycle(ScanDependentsCollections scanDependents, ScanIndex index)
        {
            Dictionary<int, int> mS2ScansPerCycle = new Dictionary<int, int>();

            foreach (int scan in index.ScanEnumerators[MSOrderType.Ms])
            {
                // if the ms1 scan has no scan dependents then topN = 0
                if (scanDependents[scan] == null)
                {
                    mS2ScansPerCycle.Add(scan, 0);
                }
                else
                {
                    mS2ScansPerCycle.Add(scan, scanDependents[scan].ScanDependentDetailArray.Length);
                }
            }

            return mS2ScansPerCycle;
        }

        public static Dictionary<int, double> FillTimes(TrailerExtraCollection trailerExtras, ScanIndex index)
        {
            Dictionary<int, double> fillTimes = new Dictionary<int, double>();

            foreach (int scan in index.ScanEnumerators[MSOrderType.Any])
            {
                fillTimes.Add(scan, trailerExtras[scan].InjectionTime);
            }

            return fillTimes;
        }

        public static Dictionary<int, double> FaimsVoltages(TrailerExtraCollection trailerExtras, ScanIndex index)
        {
            Dictionary<int, double> faimsVoltages = new Dictionary<int, double>();

            foreach (int scan in index.ScanEnumerators[MSOrderType.Any])
            {
                faimsVoltages.Add(scan, trailerExtras[scan].FaimsVoltage);
            }

            return faimsVoltages;
        }


        public static Dictionary<int, double> DutyCycle(RetentionTimeCollection retentionTimes, ScanIndex index)
        {
            Dictionary<int, double> dutyCycle = new Dictionary<int, double>();

            int[] scans = index.ScanEnumerators[MSOrderType.Ms];

            for (int i = 0; i < scans.Length; i++)
            {
                var scan = scans[i];
                if (i < scans.Length - 1)
                {
                    dutyCycle.Add(scan, (retentionTimes[scans[i+1]] - retentionTimes[scan]) * 60);
                }
                else
                {
                    dutyCycle.Add(scan, Double.NaN);
                }
            }

            return dutyCycle;
        }

        public static Dictionary<int, Distribution> IntensityDistributions(CentroidStreamCollection centroidStreams,
            SegmentScanCollection segmentScans, ScanIndex index, int maxProcesses)
        {
            ConcurrentDictionary<int, Distribution> intDist = new ConcurrentDictionary<int, Distribution>();

            int chunkSize = Constants.MultiThreading.ChunkSize(index.allScans.Keys.Count());

            var batches = index.allScans.Keys.Chunk(chunkSize);

            Parallel.ForEach(batches, Constants.MultiThreading.Options(maxProcesses), batch =>
            {
                foreach (int scan in batch)
                {
                    if (index.allScans[scan].MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        intDist.AddOrUpdate(scan, new Distribution(centroidStreams[scan].Intensities), (a, b) => b);
                    }
                    else
                    {
                        intDist.AddOrUpdate(scan, new Distribution(segmentScans[scan].Intensities), (a, b) => b);
                    }
                }
                
            });

            Dictionary<int, Distribution> distOut = new Dictionary<int, Distribution>();
            foreach (var item in intDist)
            {
                distOut.Add(item.Key, item.Value);
            }
            return distOut;
        }

        public static Dictionary<int, double> SummedIntensities(CentroidStreamCollection centroidStreams,
            SegmentScanCollection segmentScans, ScanIndex index, int maxProcesses)
        {
            ConcurrentDictionary<int, double> summedInt = new ConcurrentDictionary<int, double>();

            int chunkSize = Constants.MultiThreading.ChunkSize(index.allScans.Keys.Count());

            var batches = index.allScans.Keys.Chunk(chunkSize);

            Parallel.ForEach(batches, Constants.MultiThreading.Options(maxProcesses), batch =>
            {
                foreach (int scan in batch)
                {
                    if (index.allScans[scan].MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        summedInt.AddOrUpdate(scan, centroidStreams[scan].Intensities.Sum(), (a, b) => b);
                    }
                    else
                    {
                        summedInt.AddOrUpdate(scan, segmentScans[scan].Intensities.Sum(), (a, b) => b);
                    }
                }
            });

            var sumsOut = new Dictionary<int, double>();

            foreach (var item in summedInt) sumsOut.Add(item.Key, item.Value);

            return sumsOut;
        }

        public static Dictionary<int, double> Top80Frac(CentroidStreamCollection centroidStreams,
            SegmentScanCollection segmentScans, ScanIndex index, int maxProcesses)
        {
            ConcurrentDictionary<int, double> top80 = new ConcurrentDictionary<int, double>();

            int chunkSize = Constants.MultiThreading.ChunkSize(index.allScans.Keys.Count());

            var batches = index.allScans.Keys.Chunk(chunkSize);

            Parallel.ForEach(batches, Constants.MultiThreading.Options(maxProcesses), batch =>
            {
                foreach (int scan in batch)
                {
                    if (index.allScans[scan].MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        top80.AddOrUpdate(scan, centroidStreams[scan].Intensities.FractionOfScansConsumingTotalIntensity(percent: 80), (a, b) => b);
                    }
                    else
                    {
                        top80.AddOrUpdate(scan, segmentScans[scan].Intensities.FractionOfScansConsumingTotalIntensity(percent: 80), (a, b) => b);
                    }
                }
            });

            var topOut = new Dictionary<int, double>();

            foreach (var item in top80) topOut.Add(item.Key, item.Value);

            return topOut;
        }

        public static Dictionary<int, double> Ms1Interference(CentroidStreamCollection centroidStreams, PrecursorMassCollection precursorMasses,
            TrailerExtraCollection trailerExtras, PrecursorScanCollection precursorScans, ScanEventReactionCollection reactions, ScanIndex index,
            int maxProcesses)
        {
            ConcurrentDictionary<int, double> interference = new ConcurrentDictionary<int, double>();

            int chunkSize = Constants.MultiThreading.ChunkSize(index.ScanEnumerators[index.AnalysisOrder].Count());

            var batches = index.ScanEnumerators[index.AnalysisOrder].Chunk(chunkSize);

            Parallel.ForEach(batches, Constants.MultiThreading.Options(maxProcesses), batch =>
            {
                foreach (int scan in batch)
                {
                    int preScan = precursorScans[scan].MasterScan;
                    interference.AddOrUpdate(scan, Algorithms.Ms1Interference.CalculateForOneScan(centroidStreams[preScan], reactions[scan],
                                precursorMasses[scan].MonoisotopicMZ, trailerExtras[scan].ChargeState), (a, b) => b);
                }
            });

            var interferenceOut = new Dictionary<int, double>();

            foreach (var item in interference) interferenceOut.Add(item.Key, item.Value);

            return interferenceOut;
        }

        /* The instrument method is inaccesible on Linux and Mac, so we won't use this method to get the gradient time. Instead we will
         * approximate that it is the retention time of the last scan, which should usually be a safe assumption. */
        public static double GradientTime(string LcMethod)
        {
            Regex rx = new Regex(@"(?:Mixture\s+\[%\S+\]\s+)([\S\s]+)(?:\n\nPre)");
            Match gradientMatch = rx.Match(LcMethod);
            string gradient = gradientMatch.Groups[1].Value;
            string[] lines = gradient.ToString().Split('\n');
            List<string[]> gradientStrings = new List<string[]>();

            foreach (var line in lines)
            {
                gradientStrings.Add(line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }

            double gradientStart = 0;
            double gradientEnd = 0;

            for (int i = 0; i < gradientStrings.Count() - 1; i++)
            {
                if (gradientStrings.ElementAt(i + 1)[3] != gradientStrings.ElementAt(i)[3])
                {
                    gradientStart = Convert.ToDouble(gradientStrings.ElementAt(i)[0].Replace(':', '.'));
                    break;
                }
            }
            for (int i = gradientStrings.Count() - 1; i >= 0; i--)
            {
                if (gradientStrings.ElementAt(i - 1)[3] == gradientStrings.ElementAt(i)[3])
                {
                    gradientEnd = Convert.ToDouble(gradientStrings.ElementAt(i - 1)[0].Replace(':', '.'));
                    break;
                }
            }
            return gradientEnd - gradientStart;
        }
    }
}
