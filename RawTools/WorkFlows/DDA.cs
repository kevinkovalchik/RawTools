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
    static class WorkFlowsDDA
    {
        public static void ParseDDA(IRawFileThreadManager rawFile, WorkflowParameters parameters)
        {
            var staticRawFile = rawFile.CreateThreadAccessor();
            staticRawFile.SelectInstrument(Device.MS, 1);

            staticRawFile.CheckIfBoxcar();

            ScanIndex Index = Extract.ScanIndices(rawFile.CreateThreadAccessor());

            TrailerExtraCollection trailerExtras = Extract.TrailerExtras(rawFile.CreateThreadAccessor(), Index);

            MethodDataContainer methodData = Extract.MethodData(rawFile.CreateThreadAccessor(), Index);

            (CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans) = 
                Extract.MsData(rawFile: rawFile.CreateThreadAccessor(), index: Index);

            (PrecursorScanCollection precursorScans, ScanDependentsCollections scanDependents) = Extract.DependentsAndPrecursorScansByScanDependents(rawFile.CreateThreadAccessor(), Index);

            PrecursorMassCollection precursorMasses = Extract.PrecursorMasses(rawFile.CreateThreadAccessor(), precursorScans, trailerExtras, Index);

            RetentionTimeCollection retentionTimes = Extract.RetentionTimes(rawFile.CreateThreadAccessor(), Index);

            ScanMetaDataCollectionDDA metaData = MetaDataProcessingDDA.AggregateMetaDataDDA(centroidStreams, segmentScans, methodData, precursorScans,
                trailerExtras, precursorMasses, retentionTimes, scanDependents, Index);

            PrecursorPeakCollection peakData = AnalyzePeaks.AnalyzeAllPeaks(centroidStreams, retentionTimes, precursorMasses, precursorScans, Index);
            
            QuantDataCollection quantData = null;
            if (parameters.ParseParams.Quant)
            {
                quantData = Quantification.Quantify(centroidStreams, segmentScans, parameters, methodData, Index);
            }

            RawMetricsDataDDA rawMetrics = null;
            if (parameters.ParseParams.Metrics)
            {
                rawMetrics = MetaDataProcessingDDA.GetMetricsDataDDA(metaData, methodData, staticRawFile.FileName, retentionTimes, Index, peakData, precursorScans, quantData);
                MetricsWriter.WriteMatrix(rawMetrics, null, staticRawFile.FileName, parameters.ParseParams.OutputDirectory);
            }

            if (parameters.ParseParams.Parse)
            {
                string matrixFileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, staticRawFile.FileName, "._parse.txt");

                ParseWriter writerDDA = new ParseWriter(matrixFileName, centroidStreams, segmentScans, metaData, retentionTimes,
                precursorMasses, precursorScans, peakData, trailerExtras, Index, quantData);
                writerDDA.WriteMatrixDDA(methodData.AnalysisOrder);
            }
            if (parameters.ParseParams.WriteMgf)
            {
                ParseWriter writerMGF = new ParseWriter(centroidStreams, segmentScans, parameters, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData, Index);
                writerMGF.WriteMGF(staticRawFile.FileName);
            }

            if (parameters.ParseParams.Chromatogram != null)
            {
                ChromatogramWriter.WriteChromatogram(centroidStreams, segmentScans, retentionTimes, methodData, Index, parameters, staticRawFile.FileName);
            }

        }

        public static void QcDDA(IRawDataPlus rawFile, WorkflowParameters parameters)
        {
            rawFile.SelectInstrument(Device.MS, 1);

            rawFile.CheckIfBoxcar();

            ScanIndex Index = Extract.ScanIndices(rawFile);

            MethodDataContainer methodData = Extract.MethodData(rawFile, Index);

            CentroidStreamCollection centroidStreams = new CentroidStreamCollection();

            SegmentScanCollection segmentScans = new SegmentScanCollection();

            (centroidStreams, segmentScans) = Extract.MsData(rawFile: rawFile, index: Index);

            TrailerExtraCollection trailerExtras = Extract.TrailerExtras(rawFile, Index);

            (PrecursorScanCollection precursorScans, ScanDependentsCollections scanDependents) = Extract.DependentsAndPrecursorScansByScanDependents(rawFile, Index);

            PrecursorMassCollection precursorMasses = Extract.PrecursorMasses(rawFile, precursorScans, trailerExtras, Index);

            RetentionTimeCollection retentionTimes = Extract.RetentionTimes(rawFile, Index);

            ScanMetaDataCollectionDDA metaData = MetaDataProcessingDDA.AggregateMetaDataDDA(centroidStreams, segmentScans, methodData, precursorScans,
                trailerExtras, precursorMasses, retentionTimes, scanDependents, Index);

            PrecursorPeakCollection peakData = AnalyzePeaks.AnalyzeAllPeaks(centroidStreams, retentionTimes, precursorMasses, precursorScans, Index);

            RawMetricsDataDDA rawMetrics = MetaDataProcessingDDA.GetMetricsDataDDA(metaData, methodData, rawFile.FileName, retentionTimes, Index, peakData, precursorScans);

            QcDataCollectionDDA qcDataCollection = QC.QcWorkflow.LoadOrCreateQcCollectionDDA(parameters);

            SearchMetricsContainer searchMetrics = new SearchMetricsContainer(rawFile.FileName, rawFile.CreationDate, methodData);

            if (parameters.QcParams.PerformSearch)
            {
                Search.WriteSearchMGF(parameters, centroidStreams, segmentScans, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData,
                    Index, rawFile.FileName, parameters.QcParams.FixedScans);

                Search.RunSearch(parameters, methodData, rawFile.FileName);

                searchMetrics = SearchQC.ParseSearchResults(searchMetrics, parameters, rawFile.FileName);
            }

            QC.QcWorkflow.UpdateQcCollection(qcDataCollection, searchMetrics, rawMetrics, methodData, rawFile.FileName);
        }
    }

    
}
