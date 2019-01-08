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
using System.IO;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools.WorkFlows;
using RawTools;
using RawTools.Utilities;
using RawTools.Data.IO;
using RawTools.QC;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Interfaces;
using Serilog;

namespace RawTools.QC
{
    static class Search
    {
        public static void WriteSearchMGF(WorkflowParameters parameters, CentroidStreamCollection centroids, SegmentScanCollection segments, RetentionTimeCollection retentionTimes,
            PrecursorMassCollection precursorMasses, PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, MethodDataContainer methodData,
            ScanIndex index, string rawFileName, bool fixedScans = false)
        {
            var pars = parameters.QcParams.SearchParameters;
            int[] scans = AdditionalMath.SelectRandomScans(scans: index.ScanEnumerators[MSOrderType.Ms2],
                num: parameters.QcParams.NumberSpectra, fixedScans: parameters.QcParams.FixedScans);

            string mgfFile = ReadWrite.GetPathToFile(parameters.QcParams.QcSearchDataDirectory, rawFileName, ".mgf");

            MgfWriter.WriteMGF(rawFileName, centroids, segments, parameters, retentionTimes, precursorMasses, precursorScans,
                trailerExtras, methodData, index, outputFile: mgfFile, scans: scans);
        }

        public static void RunSearch(WorkflowParameters parameters, MethodDataContainer methodData, string rawFileName)
        {
            string mgfFile = Path.Combine(parameters.QcParams.QcSearchDataDirectory, Path.GetFileName(rawFileName) + ".mgf");
            string outputFile = Path.Combine(parameters.QcParams.QcSearchDataDirectory, Path.GetFileName(rawFileName) + ".pep.xml");

            if (parameters.QcParams.SearchAlgorithm == SearchAlgorithm.XTandem)
            {
                XTandem.RunXTandem(parameters, methodData, mgfFile, outputFile, genDecoy: true);
            }

            if (parameters.QcParams.SearchAlgorithm == SearchAlgorithm.IdentiPy)
            {
                var pars = parameters;
                Identipy.RunIdentipy(parameters, methodData, mgfFile, outputFile);
            }
        }
    }
}
