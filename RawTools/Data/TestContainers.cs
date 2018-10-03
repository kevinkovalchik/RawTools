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
