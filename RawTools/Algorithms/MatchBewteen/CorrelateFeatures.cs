using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using RawTools.QC;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.WorkFlows;
using RawTools.Utilities.MathStats;
using ThermoFisher.CommonCore.Data.FilterEnums;

namespace RawTools.Algorithms.MatchBewteen
{
    static class MatchBetween
    {
        public static Dictionary<double, int> MasterReverseRetentionTimeLookup(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans)
        {
            Dictionary<double, int> reverse = new Dictionary<double, int>();

            foreach (var key in retentionTimes.Keys)
            {
                reverse.Add(Math.Round(retentionTimes[key], 4), precursorScans[key].MasterScan);
            }

            return reverse;
        }

        public static Dictionary<double, int> ReverseRetentionTimeLookup(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans)
        {
            Dictionary<double, int> reverse = new Dictionary<double, int>();

            foreach (var key in retentionTimes.Keys)
            {
                reverse.Add(Math.Round(retentionTimes[key], 4), key);
            }

            return reverse;
        }

        public static PsmDataCollection LoadPsmData(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans, WorkflowParameters parameters, string rawFileName)
        {
            Dictionary<double, int> reverseTimeLookup = ReverseRetentionTimeLookup(retentionTimes, precursorScans);

            string QcSearchDataDirectory = parameters.QcParams.QcSearchDataDirectory;
            string resultsFile = Path.Combine(QcSearchDataDirectory, Path.GetFileName(rawFileName) + ".pep.xml");

            XElement results = XElement.Load(resultsFile);

            PsmDataCollection psms = new PsmDataCollection();
            PsmData psm;
            List<PsmData> psmList = new List<PsmData>();

            foreach (var x in results.Descendants("group").Where(x => x?.Element("protein") != null))
            {
                psm = new PsmData();
                
                psm.Decoy = x.Attribute("label").Value.StartsWith("DECOY_");

                // it is possible for each "group" in the pepXML file to have more than one protein. This just means the peptide isn't
                // unique to a single protein. However, the scoring and modifications are identical (since it is the same PSM), so we
                // can just use the first protein. That is what we do below.
                XElement domain = x.Element("protein").Element("peptide").Element("domain");
                
                psm.Hyperscore = Convert.ToDouble(domain.Attribute("hyperscore").Value);

                psm.ExpectationValue = Convert.ToDouble(domain.Attribute("expect").Value);

                psm.Charge = Convert.ToInt32(x.Attribute("z").Value);

                double measuredMZ = (Convert.ToDouble(x.Attribute("mh")?.Value) + psm.Charge) / psm.Charge;
                double matchedMZ = (Convert.ToDouble(domain?.Attribute("mh").Value) + psm.Charge) / psm.Charge;

                psm.MassDrift = (measuredMZ - matchedMZ) / matchedMZ * 1e6;

                psm.MonoisotopicMZ = measuredMZ;

                psm.RetentionTime = Math.Round(Convert.ToDouble(x.Attribute("rt").Value), 4);

                psm.Scan = reverseTimeLookup[psm.RetentionTime];

                psm.MasterScan = precursorScans[psm.Scan].MasterScan;

                psm.MasterScanRetentionTime = retentionTimes[psm.MasterScan];

                psm.Seq = x.Element("protein").Element("peptide").Element("domain").Attribute("seq").Value;

                psmList.Add(psm);
            }


            //double topDecoyScore = (from x in psmList where x.Decoy select x.Hyperscore).ToArray().Max();
            double topDecoyScore = (from x in psmList where x.Decoy select x.Hyperscore).ToArray().Percentile(99);

            //Console.WriteLine("Top decoy score: {0}", topDecoyScore);

            IEnumerable<PsmData> goodPsms = from x in psmList where !x.Decoy & x.Hyperscore > topDecoyScore select x;
            //IEnumerable<PsmData> goodPsms = from x in psmList where !x.Decoy select x;
            
            //Console.WriteLine("Median decoy expectation value: {0}", (from x in psmList where x.Decoy select x.ExpectationValue).ToArray().Percentile(50));
            //Console.WriteLine("Median non-decoy expectation value: {0}", (from x in psmList where !x.Decoy select x.ExpectationValue).ToArray().Percentile(50));

            double numDecoys = (from x in psmList where x.Decoy & x.Hyperscore > topDecoyScore select x.Hyperscore).Count();
            double numNotDecoys = (from x in psmList where !x.Decoy & x.Hyperscore > topDecoyScore select x.Hyperscore).Count();
            
            //using (var f = new StreamWriter(Path.Combine(rawFileName + ".massdrift.txt")))
            //{
            //    foreach (var x in goodPsms) f.WriteLine("{0}\t{1}",x.PeakApexRT,x.MassDrift);
            //}

            Console.WriteLine("FDR: {0}", numDecoys / numNotDecoys);
            foreach (var x in goodPsms)
            {
                psms.Add(x.Scan, x);
            }

            return psms;
        }

        public static PsmDataCollection AddPeakApex(PsmDataCollection psms, PrecursorPeakCollection peakData)
        {
            foreach (var key in psms.Keys)
            {
                var psm = psms[key];
                int scan = psm.Scan;
                psms[key].PeakApexRT = peakData[scan].MaximumRetTime;
            }

            return psms;
        }
        
        public static void AlignRT(PsmDataCollection psms1, PsmDataCollection psms2)
        {
            
        }

        public static void MatchPeaksToPSMs(this PrecursorPeakCollection peaks, PsmDataCollection psms)
        {
            var PsmKeys = psms.Keys;
            foreach (var peakKey in peaks.Keys)
            {
                if (PsmKeys.Contains(peakKey))
                {
                    peaks[peakKey].PSM = psms[peakKey];
                }
            }
        }

        public static int GetIndexOfClosest(List<double> values, double target)
        {
            double difference = 1e6;
            int currentClosest = 0;

            for (int i = 0; i < values.Count(); i++)
            {
                if (Math.Abs(target - values[i]) < difference)
                {
                    difference = Math.Abs(target - values[i]);
                    currentClosest = i;
                }
            }

            return currentClosest;
        }

        public static int GetKeyOfClosestPsmRT(Dictionary<int, PsmData> PSMs, double target)
        {
            double difference = 1e5;
            int currentClosest = 0;
            if (target < 0) target = 0;

            foreach (var element in PSMs)
            {
                if (element.Value.PeakApexRT == 0) continue;
                if (Math.Abs(target - element.Value.PeakApexRT) < difference)
                {
                    difference = Math.Abs(target - element.Value.PeakApexRT);
                    currentClosest = element.Key;
                }
            }
            return currentClosest;
        }

        public static int GetKeyOfClosestPeakRT(Dictionary<int, PrecursorPeakData> Peaks, double target)
        {
            double difference = 1e5;
            int currentClosest = 0;

            foreach (var element in Peaks)
            {
                if (element.Value.MaximumRetTime == 0) continue;

                if (Math.Abs(target - element.Value.MaximumRetTime) < difference)
                {
                    difference = Math.Abs(target - element.Value.MaximumRetTime);
                    currentClosest = element.Key;
                }
            }
            return currentClosest;
        }

        public static (int LowIndex, int HiIndex) GetBin(List<double> values, double target, double width)
        {
            int lo = GetIndexOfClosest(values, target - width / 2);
            int hi = GetIndexOfClosest(values, target + width / 2);

            return (lo, hi);
        }

        public static Dictionary<int, PsmData> GetPsmBin(Dictionary<int, PsmData> Psms, double RtTarget, double RtWidthOneSided)
        {
            int lo = GetKeyOfClosestPsmRT(Psms, RtTarget - RtWidthOneSided);

            int hi = GetKeyOfClosestPsmRT(Psms, RtTarget + RtWidthOneSided);

            var keys = Psms.Keys.ToList();

            keys.Sort();

            var bin = (from p in Psms where p.Key >= lo & p.Key <= hi select p);

            var dictOut = new Dictionary<int, PsmData>();

            foreach (var keyvalue in bin)
            {
                dictOut.Add(keyvalue.Key, keyvalue.Value);
            }

            return dictOut;
        }

        public static Dictionary<int, PrecursorPeakData> GetPeakBin(Dictionary<int, PrecursorPeakData> Peaks, double RtTarget, double RtWidthOneSided)
        {
            int lo = GetKeyOfClosestPeakRT(Peaks, RtTarget - RtWidthOneSided);

            int hi = GetKeyOfClosestPeakRT(Peaks, RtTarget + RtWidthOneSided);

            var keys = Peaks.Keys.ToList();

            keys.Sort();

            var bin = (from p in Peaks where p.Key >= lo & p.Key <= hi select p);

            var dictOut = new Dictionary<int, PrecursorPeakData>();

            foreach (var keyvalue in bin)
            {
                dictOut.Add(keyvalue.Key, keyvalue.Value);
            }

            return dictOut;
        }

        public static Ms1FeatureCollection GetFeatureBin(this Ms1FeatureCollection Features, double RtTarget, double MassTarget, double RtTol, double MassTol)
        {
            Ms1FeatureCollection features = new Ms1FeatureCollection();

            foreach (var feature in Features)
            {
                double mass = feature.Value.MonoisotopicMZ;
                double rt = feature.Value.Peak.MaximumRetTime;
                if ((Math.Abs(mass - MassTarget) / MassTarget * 1e6 < MassTol) & (Math.Abs(rt - RtTarget) / RtTarget < RtTol))
                {
                    features.Add(feature.Value.Ms2Scan, feature.Value);
                }
            }

            return features;
        }

        public static Ms1Feature FindClosest(this Ms1FeatureCollection Features, double RtTarget, double MassTarget)
        {
            if (Features.Count() == 1) return Features[Features.Keys.FirstOrDefault()];

            Dictionary<int, double> scores = new Dictionary<int, double>();
            List<double> RtDiffs = new List<double>();
            List<double> MassDiffs = new List<double>();
            List<int> Keys = new List<int>();

            // get mass and time errors
            foreach (var feature in Features)
            {
                double mass = feature.Value.MonoisotopicMZ;
                double rt = feature.Value.Peak.MaximumRetTime;

                RtDiffs.Add(Math.Abs(rt - RtTarget) / (rt + RtTarget) * 2);
                MassDiffs.Add(Math.Abs(mass - MassTarget) / (mass + MassTarget) * 2e6);
                Keys.Add(feature.Key);
            }

            double RtDiffMax = RtDiffs.Max();
            double MassDiffMax = MassDiffs.Max();

            // calculate scores for them
            for (int i = 0; i < RtDiffs.Count(); i++)
            {
                double score = RtDiffs[i] / RtDiffMax + MassDiffs[i] / MassDiffMax;
                if (Double.IsNaN(score)) score = 0;
                scores.Add(Keys[i], score);
            }

            // find the lowest scoring
            double lowScore = 3;
            int closest = 0;

            foreach (var score in scores)
            {
                if (score.Value < lowScore)
                {
                    lowScore = score.Value;
                    closest = score.Key;
                }
            }

            return Features[closest];
        }

        public static Ms1FeatureCollection AggregateMs1Features(PrecursorPeakCollection Peaks, PsmDataCollection PSMs, PrecursorMassCollection precursorMasses, CentroidStreamCollection centroids, SegmentScanCollection segments, MethodDataContainer methodData)
        {
            Ms1FeatureCollection features = new Ms1FeatureCollection();

            ProgressIndicator P = new ProgressIndicator(Peaks.Count(), "Aggregating Ms1 features and precomputing sums and self-dot products");
            P.Start();

            foreach (var peak in Peaks)
            {
                if (peak.Value.MaximumRetTime == 0) continue;

                Ms1Feature feature = new Ms1Feature();
                SimpleCentroid spectrum;

                int ms2scan = peak.Value.Ms2Scan;

                feature.Peak = peak.Value;
                feature.MonoisotopicMZ = precursorMasses[ms2scan].MonoisotopicMZ;
                feature.Ms2Scan = ms2scan;
                feature.RT = feature.Peak.MaximumRetTime;

                if (methodData.MassAnalyzers[MSOrderType.Ms2] == MassAnalyzerType.MassAnalyzerFTMS) spectrum = centroids[ms2scan].ToSimpleCentroid();
                else spectrum = segments[ms2scan].ToSimpleCentroid();

                feature.BinnedMs2Intensities = SpectraCorrelation.BinData2(spectrum.Masses, spectrum.Intensities, 1.005, 0.4, 132, 1100);

                feature.Ms2Spectrum = spectrum;

                if (PSMs.Keys.Contains(ms2scan))
                {
                    feature.PSM = PSMs[ms2scan];
                    feature.Identified = true;
                }

                feature.Ms2Sum = feature.BinnedMs2Intensities.Sum();
                feature.Ms2SelfDotProduct = feature.BinnedMs2Intensities.SelfDotProduct();

                features.Add(ms2scan, feature);
                P.Update();
            }
            P.Done();

            return features;
        }

        public static MultiRunFeatureCollection CorrelateFeatures2(Ms1FeatureCollection features1, Ms1FeatureCollection features2, SegmentScanCollection scans1, SegmentScanCollection scans2, double rtTolerance, double massTolerance)
        {
            MultiRunFeatureCollection MatchedFeatures = new MultiRunFeatureCollection();
            int featureID = 0;

            ProgressIndicator P = new ProgressIndicator(features1.Count(), "Correlating features, first pass");
            P.Start();

            var keys1 = features1.Keys.ToList();
            var file2ScansAlreadyMatched = new HashSet<int>();
            StreamWriter unmatched = new StreamWriter("C:\\Users\\Kevin\\Documents\\GSC\\Projects\\FeatureCorrelation\\Hela\\nonmatched.txt");


            foreach (var key in keys1)
            {
                var feature = features1[key];
                double mass1 = feature.MonoisotopicMZ;
                double rt1 = feature.Peak.MaximumRetTime;
                MultiRunFeature multiFeature = new MultiRunFeature();
                multiFeature.FoundIn1 = true;
                multiFeature.IdIn1 = feature.Identified;

                multiFeature.RT1 = rt1;
                multiFeature.Mass1 = mass1;
                multiFeature.Ms2Scan1 = feature.Ms2Scan;

                Ms1FeatureCollection closeFeaturesFrom2 = GetFeatureBin(features2, rt1, mass1, rtTolerance, massTolerance);
                //Ms1FeatureCollection closeFeaturesFrom2 = features2;
                Ms1Feature closestFrom2;

                if (closeFeaturesFrom2.Count() != 0)
                {
                    if (closeFeaturesFrom2.Count() > 3)
                    { }
                    foreach (var close in closeFeaturesFrom2)
                    {
                        var score = FitScores.ModifiedSteinScott(feature, close.Value);
                        multiFeature.AllScores.Add((multiFeature.Ms2Scan1, close.Value.Ms2Scan), score);

                        if (feature.PSM != null & close.Value?.PSM != null)
                        {
                            if (feature.PSM.Seq != close.Value.PSM.Seq)
                            {
                                multiFeature.LowScores.Add((multiFeature.Ms2Scan1, close.Value.Ms2Scan), score);
                            }
                        }
                    }
                    
                    //closestFrom2 = closeFeaturesFrom2.FindClosest(rt1, mass1);
                    int closestScan2 = (from x in multiFeature.AllScores where x.Value == multiFeature.AllScores.Values.Max() select x.Key.scan2).First();
                    closestFrom2 = (from x in closeFeaturesFrom2 where x.Value.Ms2Scan == closestScan2 select x.Value).First();

                    //multiFeature.XCorr = multiFeature.AllScores.Values.Max();
                    multiFeature.XCorr = multiFeature.AllScores[(feature.Ms2Scan, closestFrom2.Ms2Scan)];
                    
                    multiFeature.FoundIn2 = true;
                    multiFeature.IdIn2 = closestFrom2.Identified;

                    multiFeature.RT2 = closestFrom2.Peak.MaximumRetTime;
                    multiFeature.Mass2 = closestFrom2.MonoisotopicMZ;
                    multiFeature.Ms2Scan2 = closestFrom2.Ms2Scan;
                    file2ScansAlreadyMatched.Add(closestFrom2.Ms2Scan);

                    //features2.Remove(closestFrom2.Ms2Scan);

                    if (feature.PSM != null & feature?.PSM?.Seq == closestFrom2?.PSM?.Seq)
                    {
                        multiFeature.ConfirmSeqMatch = true;
                    }
                    else if (feature.PSM != null & closestFrom2?.PSM?.Seq != null & feature?.PSM?.Seq != closestFrom2?.PSM?.Seq)
                    {
                        unmatched.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", multiFeature.XCorr, feature.PSM.Seq, closestFrom2.PSM.Seq, feature.PSM.ExpectationValue, closestFrom2.PSM.ExpectationValue);
                    }
                }

                MatchedFeatures.Add(featureID++, multiFeature);

                //features1.Remove(key);
                P.Update();
            }
            P.Done();

            unmatched.Close();

            P = new ProgressIndicator(features2.Count(), "Correlating features, second pass");
            P.Start();

            var keys2 = features2.Keys.ToList();

            foreach (var key in keys2)
            {
                var feature = features2[key];

                if (file2ScansAlreadyMatched.Contains(feature.Ms2Scan)) continue;

                MultiRunFeature multiFeature = new MultiRunFeature();
                multiFeature.FoundIn2 = true;
                multiFeature.IdIn2 = feature.Identified;

                multiFeature.RT2 = feature.Peak.MaximumRetTime;
                multiFeature.Mass2 = feature.MonoisotopicMZ;
                multiFeature.Ms2Scan2 = feature.Ms2Scan;

                MatchedFeatures.Add(featureID++, multiFeature);

                P.Update();
            }
            P.Done();

            return MatchedFeatures;
        }

        

    }
}
