﻿// Copyright 2018 Kevin Kovalchik & Christopher Hughes
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
using System.IO;
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
        public static void UniversalDDA(IRawFileThreadManager rawFileThreadManager, WorkflowParameters parameters, QcDataCollection qcDataCollection)
        {
            MethodDataContainer methodData;
            CentroidStreamCollection centroidStreams;
            SegmentScanCollection segmentScans;
            TrailerExtraCollection trailerExtras;
            PrecursorMassCollection precursorMasses;
            RetentionTimeCollection retentionTimes;
            ScanEventReactionCollection reactions;
            ScanMetaDataCollectionDDA metaData = null;
            PrecursorPeakCollection peakData = null;
            int nScans;

            var staticRawFile = rawFileThreadManager.CreateThreadAccessor();
            staticRawFile.SelectInstrument(Device.MS, 1);

            var err = staticRawFile.FileError;

            if (err.HasError)
            {
                Console.WriteLine("ERROR: {0} reports error code: {1}. The associated message is: {2}",
                    Path.GetFileName(staticRawFile.FileName), err.ErrorCode, err.ErrorMessage);
                Console.WriteLine("Skipping this file");

                Log.Error("{FILE} reports error code: {ERRORCODE}. The associated message is: {ERRORMESSAGE}",
                    Path.GetFileName(staticRawFile.FileName), err.ErrorCode, err.ErrorMessage);

                return;
            }

            //staticRawFile.CheckIfBoxcar();

            (ScanIndex Index, PrecursorScanCollection precursorScans, ScanDependentsCollections scanDependents) =
                Extract.ScanIndicesPrecursorsDependents(rawFileThreadManager, MaxProcesses: parameters.MaxProcesses);

            nScans = Index.ScanEnumerators[MSOrderType.Ms2].Length;

            using (var rawFile = rawFileThreadManager.CreateThreadAccessor())
            {
                reactions = Extract.ScanEvents(rawFile, Index);

                methodData = Extract.MethodData(rawFile, Index);

                (centroidStreams, segmentScans) = Extract.MsData(rawFile: rawFile, index: Index);

                trailerExtras = Extract.TrailerExtras(rawFile, Index);

                precursorMasses = Extract.PrecursorMasses(rawFile, precursorScans, trailerExtras, Index);

                retentionTimes = Extract.RetentionTimes(rawFile, Index);
            }

            if (parameters.ParseParams.Parse | parameters.ParseParams.Quant | parameters.ParseParams.Metrics | parameters.RefineMassCharge | parameters.QcParams.QcDirectory != null)
            {
                peakData = AnalyzePeaks.AnalyzeAllPeaks(centroidStreams, retentionTimes, precursorMasses, precursorScans, Index, parameters.MaxProcesses);

                if (parameters.RefineMassCharge)
                {
                    MonoIsoPredictor.RefineMonoIsoMassChargeValues(parameters, centroidStreams, precursorMasses, trailerExtras, peakData, precursorScans);
                }

                metaData = MetaDataProcessingDDA.AggregateMetaDataDDA(centroidStreams, segmentScans, methodData, precursorScans,
                    trailerExtras, precursorMasses, retentionTimes, scanDependents, reactions, Index, parameters.MaxProcesses);
            }

            QuantDataCollection quantData = null;
            if (parameters.ParseParams.Quant)
            {
                quantData = Quantification.Quantify(centroidStreams, segmentScans, parameters, methodData, Index);
            }

            RawMetricsDataDDA rawMetrics = null;
            if (parameters.ParseParams.Metrics | parameters.QcParams.QcDirectory != null)
            {
                rawMetrics = MetaDataProcessingDDA.GetMetricsDataDDA(metaData, methodData, staticRawFile.FileName, retentionTimes, Index, peakData, precursorScans, quantData);
            }

            if (parameters.ParseParams.Metrics)
            {
                MetricsWriter.WriteMatrix(rawMetrics, null, staticRawFile.FileName, parameters.ParseParams.OutputDirectory);
            }

            if (parameters.ParseParams.AllScanData)
            {
                allScansWriter.WriteAllScans(centroidStreams, segmentScans, retentionTimes, methodData, Index, parameters, staticRawFile.FileName);
            }

            if (parameters.ParseParams.Parse | parameters.ParseParams.Quant)
            {
                string matrixFileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, staticRawFile.FileName, "_Matrix.txt");

                /*
                ParseWriter writerDDA = new ParseWriter(matrixFileName, centroidStreams, segmentScans, metaData, retentionTimes,
                precursorMasses, precursorScans, peakData, trailerExtras, Index, quantData);
                writerDDA.WriteMatrixDDA(methodData.AnalysisOrder);
                */
                MatrixWriter.ParseQuantDDA(matrixFileName, centroidStreams, segmentScans, metaData, retentionTimes,
                    precursorMasses, precursorScans, peakData, trailerExtras, Index, quantData);
            }

            if (parameters.ParseParams.WriteMgf)
            {
                //ParseWriter writerMGF = new ParseWriter(centroidStreams, segmentScans, parameters, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData, Index);
                //writerMGF.WriteMGF(staticRawFile.FileName);

                MgfWriter.WriteMGF(staticRawFile.FileName, centroidStreams, segmentScans, parameters, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData, Index);
            }

            if (parameters.ParseParams.WriteMgfLevels != null)
            {
                MgfLevelsWriter.WriteLevelsMgf(staticRawFile.FileName, centroidStreams, segmentScans, parameters, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData, Index);
            }

            if (parameters.ParseParams.WriteFaimsMgf)
            {
                FaimsMgfWriter.WriteFaimsMGF(staticRawFile.FileName, centroidStreams, segmentScans, parameters, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData, Index);
            }

            if (parameters.ParseParams.Chromatogram != null)
            {
                ChromatogramWriter.WriteChromatogram(centroidStreams, segmentScans, retentionTimes, methodData, Index, parameters, staticRawFile.FileName);
            }

            if (parameters.ParseParams.Xic != null)
            {
                XicWriter.WriteXic(centroidStreams, segmentScans, retentionTimes, methodData, Index, parameters, staticRawFile.FileName);
            }

            if (parameters.QcParams.QcDirectory != null)
            {
                qcDataCollection = QC.QcWorkflow.LoadOrCreateQcCollection(parameters);

                SearchMetricsContainer searchMetrics = new SearchMetricsContainer(staticRawFile.FileName, staticRawFile.CreationDate, methodData);

                // check if the raw file already exists in the QC data with a different name
                if (QcWorkflow.CheckIfFilePresentInQcCollection(staticRawFile.FileName, qcDataCollection))
                {
                    Log.Information("A file with the same creation date and time as {File} already exists in the QC data", staticRawFile.FileName);

                    Console.WriteLine("A file with the same creation date and time as {File} already exists in the QC data. Skipping to next file.",
                        staticRawFile.FileName);
                }
                else
                {
                    if (parameters.QcParams.PerformSearch)
                    {
                        Search.WriteSearchMGF(parameters, centroidStreams, segmentScans, retentionTimes, precursorMasses, precursorScans, trailerExtras, methodData,
                            Index, staticRawFile.FileName);

                        Search.RunSearch(parameters, methodData, staticRawFile.FileName);

                        searchMetrics = SearchQC.ParseSearchResults(searchMetrics, parameters, staticRawFile.FileName, nScans);
                    }

                    QcDataContainer qcData = new QcDataContainer();
                    qcData.DDA = rawMetrics;
                    qcData.SearchMetrics = searchMetrics;

                    QC.QcWorkflow.UpdateQcCollection(qcDataCollection, qcData, methodData, staticRawFile.FileName);
                }
            }
        }
    }

    
}
