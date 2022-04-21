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
    static class WorkFlowsSurveyScanOnly
    {
        public static void UniversalMs1(IRawFileThreadManager rawFileThreadManager, WorkflowParameters parameters)
        {
            MethodDataContainer methodData;
            CentroidStreamCollection centroidStreams;
            SegmentScanCollection segmentScans;
            TrailerExtraCollection trailerExtras;
            RetentionTimeCollection retentionTimes;
            ScanEventReactionCollection reactions;

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


            (ScanIndex Index, PrecursorScanCollection precursorScans, ScanDependentsCollections scanDependents) =
                Extract.ScanIndicesPrecursorsDependents(rawFileThreadManager, MaxProcesses: parameters.MaxProcesses);


            using (var rawFile = rawFileThreadManager.CreateThreadAccessor())
            {
                reactions = Extract.ScanEvents(rawFile, Index);

                methodData = Extract.MethodData(rawFile, Index);

                (centroidStreams, segmentScans) = Extract.MsData(rawFile: rawFile, index: Index);

                trailerExtras = Extract.TrailerExtras(rawFile, Index);

                retentionTimes = Extract.RetentionTimes(rawFile, Index);
            }

            if (parameters.Ms1OnlyParams.Ms1Only)
            {
                if (parameters.Ms1OnlyParams.Chromatogram != null)
                {
                    ChromatogramWriter.WriteChromatogram(centroidStreams, segmentScans, retentionTimes, methodData, Index, parameters, staticRawFile.FileName);
                }
            }            
        }
    }


}