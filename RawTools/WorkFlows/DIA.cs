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
using RawTools.Data.IO;
using RawTools.QC;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Business;
using Serilog;
using RawTools.Algorithms.ExtractData;
using RawTools.Algorithms.Analyze;

namespace RawTools.WorkFlows
{
    static class WorkFlowsDIA
    {
        public static void ParseDIA(IRawFileThreadManager rawFile, WorkflowParameters parameters)
        {
            var staticRawFile = rawFile.CreateThreadAccessor();
            staticRawFile.SelectInstrument(Device.MS, 1);

            staticRawFile.CheckIfBoxcar();

            ScanIndex Index = Extract.ScanIndices(rawFile.CreateThreadAccessor());

            TrailerExtraCollection trailerExtras = Extract.TrailerExtras(rawFile.CreateThreadAccessor(), Index);

            MethodDataContainer methodData = Extract.MethodData(rawFile.CreateThreadAccessor(), Index);

            (CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans) = 
                Extract.MsData(rawFile: rawFile.CreateThreadAccessor(), index: Index);

            RetentionTimeCollection retentionTimes = Extract.RetentionTimes(rawFile.CreateThreadAccessor(), Index);

            ScanMetaDataCollectionDIA metaData = MetaDataProcessingDIA.AggregateMetaDataDIA(centroidStreams, segmentScans, methodData,
                trailerExtras, retentionTimes, Index);

            RawMetricsDataDIA metrics = null;
            if (parameters.ParseParams.Metrics)
            {
                metrics = MetaDataProcessingDIA.GetMetricsDataDIA(metaData, methodData, staticRawFile.FileName, retentionTimes, Index);
                MetricsWriter.WriteMatrix(metrics, staticRawFile.FileName, parameters.ParseParams.OutputDirectory);
            }

            if (parameters.ParseParams.Parse)
            {
                string matrixFileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, staticRawFile.FileName, "._parse.txt");

                ParseWriter writerDIA = new ParseWriter(matrixFileName, centroidStreams, segmentScans, metaData, retentionTimes, trailerExtras, Index);
                writerDIA.WriteMatrixDIA();
            }

            // I'm not sure what goes into a DIA mgf file, so we aren't making one yet
            //if (parameters.ParseParams.WriteMgf)
            //{
            //    ParseWriter writerMGF = new ParseWriter(centroidStreams, segmentScans, parameters, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData, Index);
            //    writerMGF.WriteMGF(staticRawFile.FileName);
            //}

        }

        public static void QcDIA(IRawDataPlus rawFile, WorkflowParameters parameters)
        {
            rawFile.SelectInstrument(Device.MS, 1);

            rawFile.CheckIfBoxcar();

            ScanIndex Index = Extract.ScanIndices(rawFile);

            MethodDataContainer methodData = Extract.MethodData(rawFile, Index);

            CentroidStreamCollection centroidStreams = new CentroidStreamCollection();

            SegmentScanCollection segmentScans = new SegmentScanCollection();

            (centroidStreams, segmentScans) = Extract.MsData(rawFile: rawFile, index: Index);

            TrailerExtraCollection trailerExtras = Extract.TrailerExtras(rawFile, Index);

            RetentionTimeCollection retentionTimes = Extract.RetentionTimes(rawFile, Index);

            ScanMetaDataCollectionDIA metaData = MetaDataProcessingDIA.AggregateMetaDataDIA(centroidStreams, segmentScans, methodData,
                trailerExtras, retentionTimes, Index);

            RawMetricsDataDIA metrics = MetaDataProcessingDIA.GetMetricsDataDIA(metaData, methodData, rawFile.FileName, retentionTimes, Index);

            QcDataCollectionDIA qcDataCollection = QC.QcWorkflow.LoadOrCreateQcCollectionDIA(parameters);
            
            QC.QcWorkflow.UpdateQcCollection(qcDataCollection, metrics, methodData, rawFile.FileName);
        }
    }

    
}
