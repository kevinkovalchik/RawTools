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
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;

namespace RawTools.Data
{
    class TestContainers
    {
    }
    class ScanData
    {
        public double FillTime;
        public double RetentionTime;
        public int ScanNum;
        public CentroidStreamData CentroidSteam;
        public SegmentedScanData SegmentScan;
        public MassAnalyzerType MassAnalyzer;

        public SimpleCentroid SimpleCentroidData
        {
            get
            {
                return new SimpleCentroid(CentroidSteam);
            }
        }
    }

    class CycleDataDD
    {
        public int Charge, RefinedCharge;
        public double PrecursorMz;
        public double MonoisotopicMz, RefinedMonoisotopicMz;
    }

    class CycleDataMs2DDA : CycleDataDD
    {
        public ScanData Ms1;
        public ScanData Ms2;
    }

    class CycleDataMs3DDA : CycleDataDD
    {
        public ScanData Ms1;
        public ScanData Ms2;
        public ScanData Ms3;
    }

    class CycleDataMs2DIA
    {
        public ScanData Ms1;
        public ScanData Ms2;
    }

    class CycleDataPRM
    {
        public ScanData Ms2;
    }
}
