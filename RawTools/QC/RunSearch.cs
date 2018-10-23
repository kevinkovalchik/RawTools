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
        public static void WriteSearchMGF(QcParameters qcParameters, RawDataCollection rawData, IRawDataPlus rawFile, bool fixedScans = false)
        {
            var pars = qcParameters.searchParameters;
            int[] scans = AdditionalMath.SelectRandomScans(scans: rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2], num: pars.NumSpectra, fixedScans: fixedScans);
            MGF.WriteMGF(rawData, rawFile, qcParameters.QcSearchDataDirectory, pars.MgfMassCutoff, scans, pars.MgfIntensityCutoff);

            if (pars.NumSpectra > rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2].Length)
            {
                pars.NumSpectra = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2].Length;
            }
        }

        public static void RunSearch(QcParameters qcParameters, RawDataCollection rawData, IRawDataPlus rawFile)
        {
            string mgfFile = Path.Combine(qcParameters.QcSearchDataDirectory, Path.GetFileName(rawData.rawFileName) + ".mgf");
            string outputFile = Path.Combine(qcParameters.QcSearchDataDirectory, Path.GetFileName(rawData.rawFileName) + ".pep.xml");

            if (qcParameters.searchParameters.SearchAlgorithm == SearchAlgorithm.XTandem)
            {
                XTandem.RunXTandem(rawData, qcParameters.searchParameters, mgfFile, outputFile, genDecoy: true);
            }

            if (qcParameters.searchParameters.SearchAlgorithm == SearchAlgorithm.IdentiPy)
            {
                var pars = qcParameters.searchParameters;
                Identipy.RunIdentipy(rawData, rawFile, qcParameters.QcSearchDataDirectory, pars, writeMGF: false);
            }
        }
    }
}
