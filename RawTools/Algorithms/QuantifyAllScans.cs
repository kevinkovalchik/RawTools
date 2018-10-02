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

namespace RawTools.Algorithms
{
    static class Quantification
    {
        public static void Quantify(this QuantDataCollection quantData, RawDataCollection rawData, IRawDataPlus rawFile, string labelingReagent)
        {
            MassAnalyzerType quantAnalyzer = rawData.methodData.QuantAnalyzer;

            if (quantAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
            {
                rawData.ExtractCentroidStreams(rawFile, rawData.methodData.AnalysisOrder);
            }
            else
            {
                rawData.ExtractSegmentScans(rawFile, rawData.methodData.AnalysisOrder);
            }

            int[] scans;

            ScanIndex scanIndex = rawData.scanIndex;
            Dictionary<int, CentroidStreamData> centroidScans = rawData.centroidStreams;
            Dictionary<int, SegmentedScanData> segmentScans = rawData.segmentedScans;

            scans = scanIndex.ScanEnumerators[scanIndex.AnalysisOrder];

            ProgressIndicator progress = new ProgressIndicator(scans.Length, "Quantifying reporter ions");

            quantData.LabelingReagents = labelingReagent;

            foreach (int scan in scans)
            {
                if (quantAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    quantData.Add(scan, new QuantifyReporters(centroidScans[scan], labelingReagent).quantData);
                }
                else
                {
                    quantData.Add(scan, new QuantifyReporters(segmentScans[scan], labelingReagent).quantData);
                }

                progress.Update();
            }
            progress.Done();
            rawData.Performed.Add(Operations.Quantification);
        }
    }
}
