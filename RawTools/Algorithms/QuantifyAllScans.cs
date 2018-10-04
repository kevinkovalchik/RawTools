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
using RawTools.WorkFlows;

namespace RawTools.Algorithms
{
    static class Quantification
    {
        public static QuantDataCollection Quantify(CentroidStreamCollection centroidScans, SegmentScanCollection segmentScans, WorkflowParameters parameters, MethodDataContainer methodData, ScanIndex index)
        {
            int[] scans = index.ScanEnumerators[index.AnalysisOrder];

            QuantDataCollection quantData = new QuantDataCollection();
            
            ProgressIndicator progress = new ProgressIndicator(scans.Length, "Quantifying reporter ions");

            string labelingReagents = parameters.ParseParams.LabelingReagents;

            quantData.LabelingReagents = labelingReagents;

            foreach (int scan in scans)
            {
                if (methodData.QuantAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    quantData.Add(scan, QuantifyReporters.QuantifyOneScan(centroidScans[scan], labelingReagents));
                }
                else
                {
                    quantData.Add(scan, QuantifyReporters.QuantifyOneScan(segmentScans[scan], labelingReagents));
                }

                progress.Update();
            }
            progress.Done();

            return quantData;
        }
    }
}
