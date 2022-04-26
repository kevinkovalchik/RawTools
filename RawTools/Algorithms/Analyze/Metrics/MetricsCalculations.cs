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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
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

namespace RawTools.Algorithms.Analyze.Metrics
{
    static class MetricsCalculations
    {
        public static double GetMeanMs2ScansPerCycle(Dictionary<int, int> MS2ScansPerCycle)
        {
            return MS2ScansPerCycle.MeanFromDict();
        }

        public static double GetMedianMs2FractionConsumingTop80PercentTotalIntensity(Dictionary<int, double> FractionConsumingTop80PercentTotalIntensity, ScanIndex Index)
        {
            return (from x in Index.ScanEnumerators[MSOrderType.Ms2]
                    select FractionConsumingTop80PercentTotalIntensity[x]).ToArray().Percentile(50);
        }

        public static double GetMedianDutyCycle(Dictionary<int, double> DutyCycles, ScanIndex Index)
        {
            return (from x in Index.ScanEnumerators[MSOrderType.Ms] select DutyCycles[x]).ToArray().Percentile(50);
        }

        public static double GetMedianMSFillTime(Dictionary<int, double> FillTimes, ScanIndex Index, MSOrderType MsOrder)
        {
            return (from x in Index.ScanEnumerators[MsOrder] select FillTimes[x]).ToArray().Percentile(50);
        }

        public static double GetMedianSummedMSIntensity(Dictionary<int, double> SummedIntensities, ScanIndex Index, MSOrderType MsOrder)
        {
            return (from x in Index.ScanEnumerators[MsOrder] select SummedIntensities[x]).ToArray().Percentile(50);
        }

        public static double[] GetFaimsVoltages(Dictionary<int, double> FaimsVoltages, ScanIndex Index, MSOrderType MsOrder)
        {
            //return (double[])(from x in Index.ScanEnumerators[MsOrder] select FaimsVoltages[x]).ToArray().Distinct();
            List<double> faimsVoltageSet = new List<double>();
            faimsVoltageSet = (from x in Index.ScanEnumerators[MsOrder] select FaimsVoltages[x]).ToList();
            var DistinctFaimsVoltages = faimsVoltageSet.Distinct().ToArray();
            return DistinctFaimsVoltages;
        }

        public static int NumberOfEsiFlags(ScanMetaDataCollectionDDA metaData, ScanIndex index)
        {
            return NumberOfEsiFlags(metaData.SummedIntensity, index);
        }

        public static int NumberOfEsiFlags(ScanMetaDataCollectionDIA metaData, ScanIndex index)
        {
            return NumberOfEsiFlags(metaData.SummedIntensity, index);
        }

        public static int NumberOfEsiFlags(Dictionary<int, double> SummedIntensity, ScanIndex index)
        {
            int flags = 0;

            int[] scans = index.ScanEnumerators[MSOrderType.Ms];

            for (int i = 2; i < scans.Length; i++)
            {
                if (SummedIntensity[scans[i]] / SummedIntensity[scans[i - 1]] < 0.1)
                {
                    flags += 1;
                }
                if (SummedIntensity[scans[i]] / SummedIntensity[scans[i - 1]] > 10)
                {
                    flags += 1;
                }
            }
            return flags;
        }

        public static (double TimeBefore, double TimeAfter, double FractionAbove) ChromIntensityMetrics(ScanMetaDataCollectionDDA metaData, RetentionTimeCollection retentionTimes, ScanIndex index)
        {
            return ChromIntensityMetrics(metaData.SummedIntensity, retentionTimes, index);
        }

        public static (double TimeBefore, double TimeAfter, double FractionAbove) ChromIntensityMetrics(ScanMetaDataCollectionDIA metaData, RetentionTimeCollection retentionTimes, ScanIndex index)
        {
            return ChromIntensityMetrics(metaData.SummedIntensity, retentionTimes, index);
        }

        public static (double TimeBefore, double TimeAfter, double FractionAbove) ChromIntensityMetrics(Dictionary<int, double> SummedIntensity, RetentionTimeCollection retentionTimes, ScanIndex index)
        {
            double firstRtToExceed10 = 0;
            double lastRtToExceed10 = 0;
            double proportionCovered;
            var scans = index.ScanEnumerators[MSOrderType.Ms];
            var reversedScans = scans.Reverse();
            var totalIntList = (from x in scans select SummedIntensity[x]).ToArray();

            // get Q1 of total intensity from all scans
            double threshold = totalIntList.Max() / 10;

            // get first RT which exceeds Q1
            for (int i = 0; i < scans.Length; i++)
            {
                int scan = scans[i];
                if (totalIntList.MovingAverage(i, 20) > threshold)
                {
                    firstRtToExceed10 = retentionTimes[scan];
                    break;
                }
            }

            for (int i = scans.Length - 1; i >= 0; i--)
            {
                int scan = scans[i];
                if (totalIntList.MovingAverage(i, 20) > threshold)
                {
                    lastRtToExceed10 = retentionTimes[scan];
                    break;
                }
            }

            // get proportion of run encompassed by these times
            //proportionCovered = (lastRtToExceedQ1 - firstRtToExceedQ1) / metrics.TotalAnalysisTime;
            proportionCovered = (lastRtToExceed10 - firstRtToExceed10) / retentionTimes[index.ScanEnumerators[MSOrderType.Ms].Last()];

            return (firstRtToExceed10, retentionTimes[index.ScanEnumerators[MSOrderType.Ms].Last()] - lastRtToExceed10, proportionCovered);
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
