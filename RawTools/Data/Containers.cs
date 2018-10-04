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
using System.IO;
using System.Threading.Tasks;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using RawTools.Utilities;
using RawTools.Data.Collections;

namespace RawTools.Data.Containers
{
    class ScanIndex
    {
        public Dictionary<int, (MSOrderType MSOrder, MassAnalyzerType MassAnalyzer)> allScans;
        public MSOrderType AnalysisOrder;
        public Dictionary<MSOrderType, int[]> ScanEnumerators;

        public ScanIndex()
        {
            ScanEnumerators = new Dictionary<MSOrderType, int[]>();
        }
    }
    
    public class CentroidStreamData
    {
        public double[] Masses, Intensities, Resolutions, Noises, Baselines, SignalToNoise;

        public CentroidStreamData(CentroidStream centroidStream)
        {
            Masses = centroidStream.Masses;
            Intensities = centroidStream.Intensities;
            Resolutions = centroidStream.Resolutions;
            Noises = centroidStream.Noises;
            Baselines = centroidStream.Baselines;
            SignalToNoise = new double[Intensities.Length];
            for (int i = 0; i < SignalToNoise.Length; i++)
            {
                SignalToNoise[i] = Intensities[i] / Noises[i];
            }
        }
        
    }

    public class SimpleCentroid
    {
        public List<double> Masses, Intensities;

        public SimpleCentroid()
        {

        }

        public SimpleCentroid(List<double> masses, List<double> intensities)
        {
            Masses = masses;
            Intensities = intensities;
        }

        public SimpleCentroid(CentroidStreamData centroidStream)
        {
            Masses = centroidStream.Masses.ToList();
            Intensities = centroidStream.Intensities.ToList();
        }
    }

    class SegmentedScanData
    {
        public double[] Positions, Intensities;

        public SegmentedScanData(SegmentedScan segmentedScan)
        {
            Positions = segmentedScan.Positions;
            Intensities = segmentedScan.Intensities;
        }
    }

    class TrailerExtraData
    {
        public int MasterScan = -1;
        public int ChargeState = -1;
        public double InjectionTime = -1;
        public double MonoisotopicMZ = -1;
        public double HCDEnergy = -1;
        public double[] SPSMasses;
    }

    class PrecursorScanData
    {
        public int MS3Scan, MS2Scan, MasterScan;
        public PrecursorScanData(int ms2scan, int masterScan)
        {
            MS2Scan = ms2scan;
            MasterScan = masterScan;
        }

        public PrecursorScanData(int ms3scan, int ms2Scan, int masterScan)
        {
            MS3Scan = ms3scan;
            MasterScan = masterScan;
            MS2Scan = ms2Scan;
        }
    }

    class PrecursorMassData
    {
        public double MonoisotopicMZ, ParentMZ;

        public PrecursorMassData(double MonoisotopicMass, double ParentMass)
        {
            MonoisotopicMZ = MonoisotopicMass;
            ParentMZ = ParentMass;
        }
    }

    class MethodDataContainer
    {
        public double IsolationOffset, MS2IsolationWindow, MS3IsolationWindow;
        public MSOrderType AnalysisOrder;
        public Dictionary<MSOrderType, MassAnalyzerType> MassAnalyzers;
        public MassAnalyzerType QuantAnalyzer;
        public List<MSOrderType> MSOrderEnumerator;
        public (double MS2, (double MS1Window, double MS2Window) MS3) IsolationWindow;
        public (double MS2, (double MS1Offset, double MS2Offset) MS3) IsolationWindowOffset;
        public string Instrument;
        
        public MethodDataContainer()
        {
            MassAnalyzers = new Dictionary<MSOrderType, MassAnalyzerType>();
            MSOrderEnumerator = new List<MSOrderType>();
        }
    }

    class LabelingReagents
    {
        public Dictionary<string, (string[] Labels, double[] Masses)> Reagents = new Dictionary<string, (string[] Labels, double[] Masses)>();
        
        public LabelingReagents()
        {

            Reagents.Add("TMT0", (Labels: new string[] { "126" }, Masses: new double[] { 126.127726 }));

            Reagents.Add("TMT2", (Labels: new string[] { "126", "127C" }, new double[] { 126.127726, 127.131081 }));

            Reagents.Add("TMT6", (Labels: new string[] { "126",      "127N",     "128C",     "129N",     "130C",     "131N" },
                                  Masses: new double[] {  126.127726, 127.124761, 128.134436, 129.131470, 130.141145, 131.138180 }));

            Reagents.Add("TMT10", (Labels: new string[] { "126",      "127N",     "127C",     "128N",     "128C",     "129N",     "129C",     "130N",     "130C",     "131N" },
                                   Masses: new double[] {  126.127726, 127.124761, 127.131081, 128.128116, 128.134436, 129.131470, 129.137790, 130.134825, 130.141145, 131.138180 }));

            Reagents.Add("TMT11", (Labels: new string[] { "126",      "127N",     "127C",     "128N",     "128C",     "129N",     "129C",     "130N",     "130C",     "131N",     "131C" },
                                   Masses: new double[] {  126.127726, 127.124761, 127.131081, 128.128116, 128.134436, 129.131470, 129.137790, 130.134825, 130.141145, 131.138180, 131.144499 }));

            Reagents.Add("iTRAQ4", (Labels: new string[] { "114",      "115",      "116",      "117" },
                                    Masses: new double[] {  114.111228, 115.108263, 116.111618, 117.114973 }));

            Reagents.Add("iTRAQ8", (Labels: new string[] { "113",      "114",      "115",      "116",      "117",      "118",      "119",      "121" },
                                    Masses: new double[] {  113.107873, 114.111228, 115.108263, 116.111618, 117.114973, 118.112008, 119.115363, 121.122072 }));
        }
    }

    class ReporterIon
    {
        public double Mass, Intensity, Noise, Resolution, Baseline;

        // for all centroid stream data
        public ReporterIon(double mass, double intensity, double noise, double resolution, double baseline)
        {
            Mass = mass;
            Intensity = intensity;
            Noise = noise;
            Resolution = resolution;
            Baseline = baseline;
        }

        // for just mass and intensity (useful for ITMS data)
        public ReporterIon(double mass, double intensity)
        {
            Mass = mass;
            Intensity = intensity;
            Baseline = -1;
            Noise = -1;
            Resolution = -1;
        }
    }

    class QuantData: Dictionary<string, ReporterIon>
    {
    }

    enum Operations
    {
        Ms1CentroidStreams,
        Ms2CentroidStreams,
        Ms3CentroidStreams,
        Ms1SegmentedScans,
        Ms2SegmentedScans,
        Ms3SegmentedScans,
        TrailerExtras,
        PrecursorScans,
        RetentionTimes,
        PrecursorMasses,
        ScanIndex,
        MethodData,
        Quantification,
        MetaData,
        PeakRetAndInt,
        PeakShape,
        PeakArea
    }

    class DistributionMultiple
    {
        public List<double> P10, P25, P50, P75;

        public DistributionMultiple()
        {
            P10 = new List<double>();
            P25 = new List<double>();
            P50 = new List<double>();
            P75 = new List<double>();
        }

        public void Add(Distribution newEntry)
        {
            P10.Add(newEntry.P10);
            P25.Add(newEntry.P25);
            P50.Add(newEntry.P50);
            P75.Add(newEntry.P75);
        }

        public Distribution GetMedians()
        {
            Distribution distOut = new Distribution();

            P10.Sort();
            P25.Sort();
            P50.Sort();
            P75.Sort();

            double[] p10out = P10.ToArray();
            double[] p25out = P25.ToArray();
            double[] p50out = P50.ToArray();
            double[] p75out = P75.ToArray();

            distOut.P10 = p10out.Percentile(50);
            distOut.P25 = p25out.Percentile(50);
            distOut.P50 = p50out.Percentile(50);
            distOut.P75 = p75out.Percentile(50);

            return distOut;
        }
    }

    //[Serializable]
    public class Distribution
    {
        public double P10, P25, P50, P75;

        public Distribution() { }

        public Distribution(double[] Values)
        {
            if (Values.Length == 0)
            {
                P10 = P25 = P50 = P75 = 0;
            }
            else
            {
                int end = Values.Length - 1;
                double endAsDouble = Convert.ToDouble(end);
                double[] sortedValues = (double[])Values.Clone();
                Array.Sort(sortedValues);

                P10 = sortedValues.Percentile(16);
                P25 = sortedValues.Percentile(25);
                P50 = sortedValues.Percentile(50);
                P75 = sortedValues.Percentile(75);
            }
        }
    }

    class ScanMetaDataDDA
    {
        public double DutyCycle, FillTime, MS2ScansPerCycle, Ms1IsolationInterference = -1;
        public Distribution IntensityDistribution;
        public double SummedIntensity;
        public double FractionConsumingTop80PercentTotalIntensity;
        
        public ScanMetaDataDDA()
        { }

        public ScanMetaDataDDA(double dutyCycle, double fillTime, double scanTime, double ms2ScansPerCycle, Distribution intensityDistribution)
        {
            DutyCycle = dutyCycle;
            FillTime = fillTime;
            MS2ScansPerCycle = ms2ScansPerCycle;
            IntensityDistribution = intensityDistribution;
        }
    }

    class ScanMetaDataDIA
    {
        public double DutyCycle, FillTime = -1;
        public Distribution IntensityDistribution;
        public double SummedIntensity;
        public double FractionConsumingTop80PercentTotalIntensity;

        public ScanMetaDataDIA()
        { }

        public ScanMetaDataDIA(double dutyCycle, double fillTime, double scanTime, Distribution intensityDistribution)
        {
            DutyCycle = dutyCycle;
            FillTime = fillTime;
            IntensityDistribution = intensityDistribution;
        }
    }

    [Serializable]
    public class QuantMetaData
    {
        public double medianReporterIntensity;
        public SerializableDictionary<string, double> medianReporterIntensityByChannel;
        public string[] quantTags;

        public QuantMetaData()
        { }
    }

    class MetricsData
    {
        public string RawFileName, Instrument;
        public MassAnalyzerType MS1Analyzer, MS2Analyzer, MS3Analyzer;
        public MSOrderType MSOrder;
        public int TotalScans, MS1Scans, MS2Scans, MS3Scans;
        public double TotalAnalysisTime, MeanTopN, MS1ScanRate, MS2ScanRate, MS3ScanRate, MeanDutyCycle, MedianMS1FillTime, MedianMS2FillTime,
            MedianMS3FillTime, MedianPrecursorIntensity, MedianSummedMS2Intensity, MedianBaselinePeakWidth, MedianHalfHeightPeakWidth, PeakCapacity, Gradient,
            MedianAsymmetryFactor, MedianMs2FractionConsumingTop80PercentTotalIntensity, MedianMs1IsolationInterference;
        public bool IncludesQuant = false;
        public QuantMetaData QuantMeta;
    }

    class PrecursorPeakData
    {
        public double BaselineWidth, MaximumIntensity, ParentIntensity, MaximumRetTime, ParentRetTime;
        public double Area = 0;
        public int FirstScan, LastScan, MaxScan, ParentScan, NScans;// PreviousScan, NextScan, NScans;
        public double[] Intensities, RetTimes;
        public bool PeakFound = false, ContainsFirstMS1Scan = false, ContainsLastMS1Scan = false;
        public int[] Scans;
        public PeakShape PeakShape;
    }

    class NewMetaData
    {
        public double DutyCycle, FillTime, MS2ScansPerCycle = -1;
        public Distribution IntensityDistribution;

        public NewMetaData()
        { }

        public NewMetaData(double dutyCycle, double fillTime, double scanTime, double ms2ScansPerCycle, Distribution intensityDistribution)
        {
            DutyCycle = dutyCycle;
            FillTime = fillTime;
            MS2ScansPerCycle = ms2ScansPerCycle;
            IntensityDistribution = intensityDistribution;
        }
    }

    class MasterDataContainer
    {
        /* The MasterDataContainer is intended to be indexed by peptide spectrum scan.  Take the following master scan and data dependent scans, for example: MS1 200, MS2 203, MS3 204
         * Data from these scans will all be in one container. The master scan is 200, the peptide scan is 203, and the quant scan is 204. If it were an MS2 experiment, the peptide and quant
         * scan indices would both be stored, but they would happen to be the same number. This allows for more general processing to take place.
         */
        int MasterScan, QuantScan, PeptideScan;
        PrecursorPeakData PrecursorPeak;
        
        QuantData QuantData;

    }

    class Width: Distribution
    {    }
    class Asymmetry: Distribution
    {    }

    class PeakShape
    {
        public double MaxRetTime;
        public Width Width;
        public Asymmetry Asymmetry;

        public PeakShape()
        { }

        public PeakShape(Width width, Asymmetry asymmetry, double peakMax)
        {
            Width = new Width();
            Asymmetry = new Asymmetry();
            MaxRetTime = peakMax;
            Width = width;
            Asymmetry = asymmetry;
        }

        public PeakShape(Distribution width, Distribution asymmetry, double peakMax)
        {
            Width = new Width();
            Asymmetry = new Asymmetry();

            MaxRetTime = peakMax;
            Width.P10 = width.P10;
            Width.P25 = width.P25;
            Width.P50 = width.P50;
            Width.P75 = width.P75;

            Asymmetry.P10 = asymmetry.P10;
            Asymmetry.P25 = asymmetry.P25;
            Asymmetry.P50 = asymmetry.P50;
            Asymmetry.P75 = asymmetry.P75;
        }
    }

    class TrailerExtraIndices
    {
        public int InjectionTime, MasterScan, MonoisotopicMZ, ChargeState, HCDEnergy = -1;
        public List<int> SPSMasses = new List<int>();

        public TrailerExtraIndices(IRawDataPlus rawFile)
        {
            HeaderItem[] header = rawFile.GetTrailerExtraHeaderInformation();
            for (int i = 0; i < header.Length; i++)
            {
                if (header[i].Label.ToLower().Contains("injection time") & !header[i].Label.ToLower().Contains("reagent"))
                {
                    InjectionTime = i;
                }
                if (header[i].Label.ToLower().Contains("master scan"))
                {
                    MasterScan = i;
                }
                if (header[i].Label.ToLower().Contains("monoisotopic"))
                {
                    MonoisotopicMZ = i;
                }
                if (header[i].Label.ToLower().Contains("charge state"))
                {
                    ChargeState = i;
                }
                if (header[i].Label.ToLower().Contains("hcd energy") & !header[i].Label.ToLower().Contains("ev"))
                {
                    HCDEnergy = i;
                }
                if (header[i].Label.ToLower().Contains("sps"))
                {
                    SPSMasses.Add(i);
                }
            }
        }
    }

    class SearchParameters
    {
        public string FastaDatabase, PythonExecutable, IdentipyScript, XTandemDirectory;
        public SearchAlgorithm SearchAlgorithm;
        public string FixedMods, NMod, KMod, XMod;
        public int NumSpectra;
        public double MgfIntensityCutoff, MgfMassCutoff;

        public SearchParameters()
        {
            FixedMods = NMod = KMod = XMod = null;
        }
        public bool FixedScans;
    }

    class QcParameters
    {
        public string RawFileDirectory, QcDirectory, QcFile;
        public SearchParameters searchParameters;
        public string QcSearchDataDirectory { get { return Path.Combine(QcDirectory, "QcSearchData"); } }
        public bool RefineMassCharge;
        public ExperimentType ExpType;
    }

    public enum SearchAlgorithm
    {
        XTandem = 1,
        IdentiPy = 2
    }

    public enum ExperimentType
    {
        DDA = 1,
        DIA = 2,
        PRM = 3
    }

    public class SearchData
    {
        public int PeptidesWithMissedCleavages;
        public int TotalNumPeptides;
        public int NLabelSites, KLabelSites, XLabelSites;
        public int NLabelSitesMissed, KLabelSitesMissed, XLabelSitesMissed;
        public int Charge2, Charge3, Charge4;
    }

    class PsmData
    {
        public double Hyperscore, ExpectationValue, MassDrift;
        public int Id;
        public bool Decoy;
        public string Seq;
        public int Start, End, Charge, MissedCleavages;
        // these mods will be in mass@aa format
        public List<Modification> Mods = new List<Modification>();
    }

    class Modification
    {
        public int Loc;
        public string AA;
        public double Mass;

        public string MassAtAa
        {
            get
            {
                return String.Concat(Mass, "@", AA);
            }
        }
    }
}
