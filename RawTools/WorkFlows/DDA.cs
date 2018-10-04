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
using RawTools.Data.Collections;
using RawTools.Data.Extraction;
using RawTools.Data.Containers;
using RawTools.Algorithms;
using RawTools.Utilities;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Business;
using Serilog;
using RawTools.Algorithms.ExtractData;
using RawTools.Algorithms.Analyze;

namespace RawTools.WorkFlows
{
    

    static class WorkFlows
    {
        public static void DDA(IRawDataPlus rawFile, WorkflowParameters parameters)
        {
            rawFile.SelectInstrument(Device.MS, 1);
            
            rawFile.CheckIfBoxcar();
            
            ScanIndex Index = Extract.ScanIndices(rawFile);

            MethodDataContainer methodData = Extract.MethodData(rawFile, Index);

            CentroidStreamCollection centroidStreams = new CentroidStreamCollection();

            SegmentScanCollection segmentScans = new SegmentScanCollection();

            (centroidStreams, segmentScans) = Extract.MsData(rawFile: rawFile, index: Index);

            TrailerExtraCollection trailerExtras = Extract.TrailerExtras(rawFile, Index);

            PrecursorScanCollection precursorScans = Extract.PrecursorScansByScanDependents(rawFile, Index);

            PrecursorMassCollection precursorMasses = Extract.PrecursorMasses(rawFile, precursorScans, trailerExtras, Index);

            RetentionTimeCollection retentionTimes = Extract.RetentionTimes(rawFile, Index);

            ScanDependentsCollections scanDependents = Extract.ScanDependents(rawFile, Index);

            ScanMetaDataCollectionDDA metaData = MetaDataProcessing.AggregateMetaDataDDA(centroidStreams, segmentScans, methodData, precursorScans,
                trailerExtras, precursorMasses, retentionTimes, scanDependents, Index);

            PrecursorPeakCollection peakData = AnalyzePeaks.AnalyzeAllPeaks(centroidStreams, retentionTimes, precursorMasses, precursorScans, Index);
            
            QuantDataCollection quantData = null;

            if (parameters.ParseParams.Quant)
            {
                quantData = Quantification.Quantify(centroidStreams, segmentScans, parameters, methodData, Index);
            }

            MetricsData metrics = MetaDataProcessing.GetMetricsDataDDA(metaData, methodData, parameters, retentionTimes, Index, peakData, quantData);


        }
    }

    
}
