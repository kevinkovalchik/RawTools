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
using System.Collections.Generic;
using System.Collections.Concurrent;
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
    class ScanData
    {
        public MSOrderType MSOrder;
        public MassAnalyzerType MassAnalyzer;
        public bool HasPrecursors;
        public bool HasDependents;
    }

    class ScanIndex
    {
        public Dictionary<int, ScanData> allScans;
        public MSOrderType AnalysisOrder;
        public Dictionary<MSOrderType, int[]> ScanEnumerators;
        public int TotalScans;

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
            if (centroidStream.Length > 0)
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
            else
            {
                Masses = new double[0];
                Intensities = new double[0];
                Resolutions = new double[0];
                Noises = new double[0];
                Baselines = new double[0];
                SignalToNoise = new double[0];
            }
        }

        public SimpleCentroid ToSimpleCentroid()
        {
            return new SimpleCentroid(this);
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

        public SimpleCentroid(SegmentedScanData segmentedScan)
        {
            Masses = segmentedScan.Positions.ToList();
            Intensities = segmentedScan.Intensities.ToList();
        }
    }

    public class SegmentedScanData
    {
        public double[] Positions, Intensities;

        public SegmentedScanData(SegmentedScan segmentedScan)
        {
            Positions = segmentedScan.Positions;
            Intensities = segmentedScan.Intensities;
        }

        public SimpleCentroid ToSimpleCentroid()
        {
            return new SimpleCentroid(this);
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
        public double FaimsVoltage = -1;
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

    public class MethodDataContainer
    {
        public double IsolationOffset, MS2IsolationWindow, MS3IsolationWindow;
        public MSOrderType AnalysisOrder;
        public Dictionary<MSOrderType, MassAnalyzerType> MassAnalyzers;
        public MassAnalyzerType QuantAnalyzer;
        public List<MSOrderType> MSOrderEnumerator;
        public (double MS2, (double MS1Window, double MS2Window) MS3) IsolationWindow;
        public (double MS2, (double MS1Offset, double MS2Offset) MS3) IsolationWindowOffset;
        public string Instrument;
        public DateTime CreationDate;
        
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

            Reagents.Add("TMT2", (Labels: new string[] { "126", "127C" }, Masses: new double[] { 126.127726, 127.131081 }));

            Reagents.Add("TMT6", (Labels: new string[] { "126", "127N", "128C", "129N", "130C", "131N" },
                                  Masses: new double[] {  126.127726, 127.124761, 128.134436, 129.131470, 130.141145, 131.138180 }));

            Reagents.Add("TMT10", (Labels: new string[] { "126", "127N", "127C", "128N", "128C", "129N", "129C", "130N", "130C", "131N" },
                                   Masses: new double[] {  126.127726, 127.124761, 127.131081, 128.128116, 128.134436, 129.131470, 129.137790, 130.134825, 130.141145, 131.138180 }));

            Reagents.Add("TMT11", (Labels: new string[] { "126", "127N", "127C", "128N", "128C", "129N", "129C", "130N", "130C", "131N", "131C" },
                                   Masses: new double[] {  126.127726, 127.124761, 127.131081, 128.128116, 128.134436, 129.131470, 129.137790, 130.134825, 130.141145, 131.138180, 131.144499 }));

            Reagents.Add("TMT16", (Labels: new string[] { "126", "127N", "127C", "128N", "128C", "129N", "129C", "130N", "130C", "131N", "131C", "132N", "132C", "133N", "133C", "134N" },
                                   Masses: new double[] { 126.127726, 127.124761, 127.131081, 128.128116, 128.134436, 129.131470, 129.137790, 130.134825, 130.141145, 131.138180, 131.144499, 132.141535, 132.147855, 133.14489, 133.15121, 134.148245 }));

            Reagents.Add("TMT18", (Labels: new string[] { "126", "127N", "127C", "128N", "128C", "129N", "129C", "130N", "130C", "131N", "131C", "132N", "132C", "133N", "133C", "134N", "134C", "135N" },
                                   Masses: new double[] { 126.127726, 127.124761, 127.131081, 128.128116, 128.134436, 129.131470, 129.137790, 130.134825, 130.141145, 131.138180, 131.144499, 132.141535, 132.147855, 133.144890, 133.151210, 134.148245, 134.154565, 135.151600 }));

            Reagents.Add("iTRAQ4", (Labels: new string[] { "114", "115", "116", "117" },
                                    Masses: new double[] {  114.111228, 115.108263, 116.111618, 117.114973 }));

            Reagents.Add("iTRAQ8", (Labels: new string[] { "113", "114", "115", "116", "117", "118", "119", "121" },
                                    Masses: new double[] {  113.107873, 114.111228, 115.108263, 116.111618, 117.114973, 118.112008, 119.115363, 121.122072 }));
        }
    }

    class ReporterIon
    {
        public double Mass, Intensity, Noise, Resolution, Baseline, SignalToNoise, ppmMassError;

        // for all centroid stream data
        public ReporterIon(double mass, double intensity, double noise, double resolution, double baseline, double s2n, double ppmError)
        {
            Mass = mass;
            Intensity = intensity;
            Noise = noise;
            Resolution = resolution;
            Baseline = baseline;
            SignalToNoise = s2n;
            ppmMassError = ppmError;
        }

        // for just mass and intensity (useful for ITMS data)
        public ReporterIon(double mass, double intensity, double ppmError)
        {
            Mass = mass;
            Intensity = intensity;
            Baseline = -1;
            Noise = -1;
            Resolution = -1;
            SignalToNoise = -1;
            ppmMassError = ppmError;
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
        public ConcurrentBag<double> P10, P25, P50, P75;

        public DistributionMultiple()
        {
            P10 = new ConcurrentBag<double>();
            P25 = new ConcurrentBag<double>();
            P50 = new ConcurrentBag<double>();
            P75 = new ConcurrentBag<double>();
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

            distOut.P10 = P10.ToList().Percentile(50);
            distOut.P25 = P25.ToList().Percentile(50);
            distOut.P50 = P50.ToList().Percentile(50);
            distOut.P75 = P75.ToList().Percentile(50);

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
                /*
                int end = Values.Length - 1;
                double endAsDouble = Convert.ToDouble(end);
                double[] sortedValues = (double[])Values.Clone();
                Array.Sort(sortedValues);
                */

                P10 = Values.Percentile(16);
                P25 = Values.Percentile(25);
                P50 = Values.Percentile(50);
                P75 = Values.Percentile(75);
            }
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


    [Serializable]
    public class RawMetricsDataDDA
    {
        public DateTime DateAcquired;
        public string RawFileName, Instrument;
        public MassAnalyzerType MS1Analyzer, MS2Analyzer, MS3Analyzer;
        public MSOrderType MSOrder;
        public int TotalScans, MS1Scans, MS2Scans, MS3Scans;
        public int NumberOfEsiFlags;
        public double TotalAnalysisTime, MeanTopN;
        public double MS1ScanRate, MS2ScanRate, MS3ScanRate;
        public double MeanDutyCycle;
        public double MedianMS1FillTime, MedianMS2FillTime, MedianMS3FillTime;
        public double[] FaimsVoltages;
        public double MedianPrecursorIntensity;
        public double MedianSummedMS1Intensity, MedianSummedMS2Intensity;
        public double MedianBaselinePeakWidth, MedianHalfHeightPeakWidth, PeakCapacity, Gradient,
            MedianAsymmetryFactor;
        public double MedianMs2FractionConsumingTop80PercentTotalIntensity;
        public double MedianMs1IsolationInterference;
        public bool IncludesQuant = false;
        public QuantMetaData QuantMeta;
        public double TimeBeforeFirstScanToExceedPoint1MaxIntensity;
        public double TimeAfterLastScanToExceedPoint1MaxIntensity;
        public double FractionOfRunAbovePoint1MaxIntensity;
        public Distribution Ms1FillTimeDistribution, Ms2FillTimeDistribution, Ms3FillTimeDistribution;
        public ((double P10, double P50) Asymmetry, (double P10, double P50) Width) PeakShape;
    }

    [Serializable]
    public class RawMetricsDataDIA
    {
        public DateTime DateAcquired;
        public string RawFileName, Instrument;
        public MassAnalyzerType MS1Analyzer, MS2Analyzer;
        public MSOrderType MSOrder;
        public int TotalScans, MS1Scans, MS2Scans;
        public int NumberOfEsiFlags;
        public double TotalAnalysisTime;
        public double MS1ScanRate, MS2ScanRate;
        public double MeanDutyCycle;
        public double MedianMS1FillTime, MedianMS2FillTime;
        public double MedianSummedMS1Intensity, MedianSummedMS2Intensity;
        public double Gradient;
        public double MedianMs2FractionConsumingTop80PercentTotalIntensity;
        public double TimeBeforeFirstScanToExceedPoint1MaxIntensity;
        public double TimeAfterLastScanToExceedPoint1MaxIntensity;
        public double FractionOfRunAbovePoint1MaxIntensity;
        public Distribution Ms1FillTimeDistribution, Ms2FillTimeDistribution, Ms3FillTimeDistribution;
    }

    [Serializable]
    public class SearchMetricsContainer
    {
        public string RawFile, Instrument;
        public DateTime DateAcquired;
        public double IdentificationRate, MissedCleavageRate;
        public double DigestionEfficiency;
        public double ChargeRatio3to2, ChargeRatio4to2;
        public double MedianMassDrift;
        public string SearchParameters = "None";
        public double MedianPeptideScore;
        public double CutoffDecoyScore;
        public int NumPSMs;
        public int NumUniquePeptides;
        public SearchData SearchData;
        public SerializableDictionary<string, double> ModificationFrequency;

        public SearchMetricsContainer()
        { }

        public SearchMetricsContainer(string rawFile, DateTime dateAquired, MethodDataContainer methodData)
        {
            RawFile = rawFile;
            Instrument = methodData.Instrument;
            DateAcquired = dateAquired;
            DigestionEfficiency = IdentificationRate = MissedCleavageRate = -1;
            ChargeRatio3to2 = ChargeRatio4to2 = -1;
            MedianMassDrift = -1;
            SearchData = new SearchData();
            ModificationFrequency = new SerializableDictionary<string, double>();
        }
    }

    class PrecursorPeakData
    {
        public double BaselineWidth, MaximumIntensity, ParentIntensity, MaximumRetTime, ParentRetTime;
        public double Area = 0;
        public int Ms2Scan;
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
        public int InjectionTime, MasterScan, MonoisotopicMZ, ChargeState, HCDEnergy = -1, FaimsVoltage = -1;
        public List<int> SPSMasses = new List<int>();

        public TrailerExtraIndices(IRawDataPlus rawFile)
        {
            HeaderItem[] header = rawFile.GetTrailerExtraHeaderInformation();
            for (int i = 0; i < header.Length; i++)
            {
                //Console.WriteLine("{0} {1}", header[i].Label, i);
                if (header[i].Label.ToLower().Contains("injection time") & !header[i].Label.ToLower().Contains("reagent"))
                {
                    InjectionTime = i;
                }
                if (header[i].Label.ToLower().Contains("master scan") | header[i].Label.ToLower().Contains("master index"))
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
                if (header[i].Label.ToLower().Contains("cv"))
                {
                    FaimsVoltage = i;
                }
            }
        }
    }

    public class SearchParameters
    {
        public string FastaDatabase, XTandemDirectory;
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

    public enum SearchAlgorithm
    {
        None = 0,
        XTandem = 1,
    }

    public enum ExperimentType
    {
        DDA = 1,
        DIA = 2,
        PRM = 3
    }

    public class SearchData
    {
        public int PSMsWithNoMissedCleavages;
        public int TotalNumGoodPSMs;
        public int NLabelSites, KLabelSites, XLabelSites;
        public int NLabelSitesHit, KLabelSitesHit, XLabelSitesHit;
        public int NumCharge2, NumCharge3, NumCharge4;
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

    class Ms1Feature
    {
        public bool Identified;
        public PsmData PSM;
        public PrecursorPeakData Peak;
        public double MonoisotopicMZ;
        public double RT;
        public int Ms2Scan;

        public double Ms2Sum;
        public double Ms2SelfDotProduct;

        public List<double> BinnedMs2Intensities;
        public SimpleCentroid Ms2Spectrum;

        public QuantData QuantData;

        public Ms1Feature()
        {
            Identified = false;
        }
    }

    
    class GroupedMs1Feature: Dictionary<(string Run, int Ms2Scan), Ms1Feature>
    {
        public double AverageRT;
        public double AverageMonoIsoMZ;
        public double AverageScore;
        public Dictionary<((string run, int ms2scan), (string run, int ms2scan)), double> Scores = new Dictionary<((string run, int ms2scan), (string run, int ms2scan)), double>();
        public Dictionary<((string run, int ms2scan), (string run, int ms2scan)), double> SeqMatch = new Dictionary<((string run, int ms2scan), (string run, int ms2scan)), double>();

        public void UpdateAverageMassAndRT()
        {
            List<double> rts = new List<double>();
            List<double> masses = new List<double>();
            foreach (var value in Values)
            {
                rts.Add(value.Peak.MaximumRetTime);
                masses.Add(value.MonoisotopicMZ);
            }
            AverageRT = rts.Average();
            AverageMonoIsoMZ = masses.Average();
        }

        public void UpdateAverageScore()
        {
            if (Scores.Values.Count() == 0)
            {
                AverageScore = 0;
            }
            else
            {
                AverageScore = Scores.Values.Average();
            }
        }

        /*
        public double AverageRT { get { return (from x in this.Values select x.RT).Average(); } }
        public double AverageMonoIsoMZ { get { return (from x in this.Values select x.MonoisotopicMZ).Average(); } }
        */
    }

    /// <summary>
    /// Information on picked and identified features. Key is the Ms2 scan number.
    /// </summary>
    class IdData : Dictionary<int, (bool Identified, bool Picked)> { };

    class RunAndScanNumber
    {
        string Run;
        int Ms2Scan;

        public RunAndScanNumber(string run, int ms2scan)
        {
            Run = run;
            Ms2Scan = ms2scan;
        }
    }

    /// <summary>
    /// Contains data on features found in one or more runs.
    /// </summary>
    class MultiRunFeatureMatchData
    {
        public Dictionary<int, string> Runs;

        public Dictionary<int, IdData> IdData;
        
        public Dictionary<int, List<int>> Ms2Scans;

        public Dictionary<RunAndScanNumber, double> RT;
        public Dictionary<RunAndScanNumber, double> MonoisotopicMZ;

        public Dictionary<RunAndScanNumber, PsmData> PSM;
        public Dictionary<RunAndScanNumber, PrecursorPeakData> PeakData;

        public Dictionary<(RunAndScanNumber, RunAndScanNumber), double> Score;
        public Dictionary<(RunAndScanNumber, RunAndScanNumber), double> MassError;
        public Dictionary<(RunAndScanNumber, RunAndScanNumber), double> RtError;
        public Dictionary<(RunAndScanNumber, RunAndScanNumber), double> SeqMatch;

        public double AverageRt { get { return RT.Values.Average(); } }
        public double AverageMonoisotopicMZ { get { return MonoisotopicMZ.Values.Average(); } }
        
        public MultiRunFeatureMatchData()
        {
            Runs = new Dictionary<int, string>();
            IdData = new Dictionary<int, IdData>();
            Ms2Scans = new Dictionary<int, List<int>>();
            RT = new Dictionary<RunAndScanNumber, double>();
            MonoisotopicMZ = new Dictionary<RunAndScanNumber, double>();
            Score = new Dictionary<(RunAndScanNumber, RunAndScanNumber), double>();
            MassError = new Dictionary<(RunAndScanNumber, RunAndScanNumber), double>();
            RtError = new Dictionary<(RunAndScanNumber, RunAndScanNumber), double>();
            SeqMatch = new Dictionary<(RunAndScanNumber, RunAndScanNumber), double>();
            PSM = new Dictionary<RunAndScanNumber, PsmData>();
            PeakData = new Dictionary<RunAndScanNumber, PrecursorPeakData>();
        }
    }
}
