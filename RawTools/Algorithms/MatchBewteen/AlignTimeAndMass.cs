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

namespace RawTools.Algorithms.MatchBewteen
{
    static class AlignTimeAndMass
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


            double topDecoyScore = (from x in psmList where x.Decoy select x.Hyperscore).ToArray().Percentile(95);

            //Console.WriteLine("Top decoy score: {0}", topDecoyScore);

            IEnumerable<PsmData> goodPsms = from x in psmList where !x.Decoy & x.Hyperscore > topDecoyScore select x;
            //IEnumerable<PsmData> goodPsms = from x in psmList where !x.Decoy select x;
            
            //Console.WriteLine("Median decoy expectation value: {0}", (from x in psmList where x.Decoy select x.ExpectationValue).ToArray().Percentile(50));
            //Console.WriteLine("Median non-decoy expectation value: {0}", (from x in psmList where !x.Decoy select x.ExpectationValue).ToArray().Percentile(50));

            double numDecoys = (from x in psmList where x.Decoy & x.Hyperscore > topDecoyScore select x.Hyperscore).Count();
            double numNotDecoys = (from x in psmList where !x.Decoy & x.Hyperscore > topDecoyScore select x.Hyperscore).Count();

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

        public static MultiRunFeatureCollection CorrelateFeatures(PsmDataCollection psms1, PsmDataCollection psms2, PrecursorPeakCollection peaks1, PrecursorPeakCollection peaks2, PrecursorMassCollection masses1, PrecursorMassCollection masses2, double rtTolerance, double massTolerance)
        {
            MultiRunFeatureCollection features = new MultiRunFeatureCollection();

            int ID = 0;

            foreach (var key1 in psms1.Keys)
            {
                // get basic info on the feature
                List<PsmData> possibles = new List<PsmData>();
                int ms2scan1 = psms1[key1].Scan;
                double rt1 = peaks1[key1].MaximumRetTime;
                double mass1 = masses1[key1].MonoisotopicMZ;
                bool found = false;
                MultiRunFeature feature = new MultiRunFeature();
                feature.FoundIn1 = true;
                feature.IdIn1 = true;

                feature.RT1 = rt1;
                feature.Mass1 = mass1;

                feature.Ms2Scan1 = psms1[key1].Scan;

                // look for the feature in the other set of psms



                foreach (var key2 in psms2.Keys)
                {
                    double rt2 = peaks2[key2].MaximumRetTime;
                    double mass2 = masses2[key2].MonoisotopicMZ;
                    int ms2scan2 = psms2[key2].Scan;

                    if ((Math.Abs(rt1 - rt2)/rt1 < rtTolerance) & (Math.Abs(mass1 - mass2)/mass1 * 1e6 < massTolerance))
                    {
                        //feature.Add(psms1.FileName, psms1[key1]);
                        //feature.Add(psms2.FileName, psms1[key2]);
                        feature.IdIn2 = true;
                        feature.FoundIn2 = true;

                        feature.RT2 = rt2;
                        feature.Mass2 = mass2;
                        //Console.WriteLine("{0}\t{1}", psms1[key1].Seq, psms2[key2].Seq);
                        string seq1 = psms1[key1].Seq;
                        string seq2 = psms2[key2].Seq;

                        feature.Ms2Scan2 = psms2[key2].Scan;

                        if (seq1 == seq2)
                        {
                            feature.ConfirmSeqMatch = true;
                        }

                        features.Add(ID, feature);
                        ID++;
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                foreach (var peak2 in peaks2.Values)
                {
                    double rt2 = peak2.MaximumRetTime;
                    double mass2 = masses2[peak2.Ms2Scan].MonoisotopicMZ;

                    if ((Math.Abs(rt1 - rt2) / rt1 < rtTolerance) & (Math.Abs(mass1 - mass2) / mass1 * 1e6 < massTolerance))
                    {
                        //feature.Add(psms1.FileName, psms1[key1]);
                        //feature.Add(psms2.FileName, null);
                        feature.IdIn2 = false;
                        feature.FoundIn2 = true;

                        feature.RT2 = rt2;
                        feature.Mass2 = mass2;

                        feature.Ms2Scan2 = peak2.Ms2Scan;

                        features.Add(ID, feature);
                        ID++;
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                feature.IdIn2 = false;
                feature.FoundIn2 = false;

                features.Add(ID, feature);
                ID++;
            }

            foreach (var key2 in psms2.Keys)
            {
                // get basic info on the feature
                List<PsmData> possibles = new List<PsmData>();
                int ms2scan2 = psms2[key2].Scan;
                double rt2 = peaks2[key2].MaximumRetTime;
                double mass2 = masses2[key2].MonoisotopicMZ;
                bool found = false;
                MultiRunFeature feature = new MultiRunFeature();
                feature.FoundIn2 = true;
                feature.IdIn2 = true;

                feature.RT2 = rt2;
                feature.Mass2 = mass2;

                feature.Ms2Scan2 = psms2[key2].Scan;

                // look for the feature in the other set of psms
                foreach (var key1 in psms1.Keys)
                {
                    double rt1 = peaks1[key1].MaximumRetTime;
                    double mass1 = masses1[key1].MonoisotopicMZ;
                    int ms2scan1 = psms1[key1].Scan;

                    if ((Math.Abs(rt2 - rt1) / rt2 < rtTolerance) & (Math.Abs(mass2 - mass1) / mass2 * 1e6 < massTolerance))
                    {
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                foreach (var peak1 in peaks1.Values)
                {
                    double rt1 = peak1.MaximumRetTime;
                    double mass1 = masses1[peak1.Ms2Scan].MonoisotopicMZ;

                    if ((Math.Abs(rt2 - rt1) / rt2 < rtTolerance) & (Math.Abs(mass2 - mass1) / mass2 * 1e6 < massTolerance))
                    {
                        //feature.Add(psms2.FileName, psms2[key2]);
                        //feature.Add(psms1.FileName, null);
                        feature.IdIn1 = false;
                        feature.FoundIn1 = true;

                        feature.RT1 = rt1;
                        feature.Mass1 = mass1;

                        feature.Ms2Scan1 = peak1.Ms2Scan;

                        features.Add(ID, feature);
                        ID++;
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                feature.IdIn1 = false;
                feature.FoundIn1 = false;

                features.Add(ID, feature);
                ID++;
            }

            return features;
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
                if ((Math.Abs(mass - MassTarget) / (mass + MassTarget) * 2e6 < MassTol) & (Math.Abs(rt - RtTarget) / (rt + RtTarget) * 2 < RtTol))
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

        public static MultiRunFeatureCollection MatchFeatures(PsmDataCollection psms1, PsmDataCollection psms2, PrecursorPeakCollection peaks1, PrecursorPeakCollection peaks2, PrecursorMassCollection masses1, PrecursorMassCollection masses2, double rtTolerance, double massTolerance)
        {
            AddPeakApex(psms1, peaks1);
            AddPeakApex(psms2, peaks2);

            // remove features with no associated ms1 peak
            var toRemove = (from p in psms1 where p.Value.PeakApexRT == 0 select p.Key).ToList();
            foreach (var k in toRemove) psms1.Remove(k);

            toRemove = (from p in psms2 where p.Value.PeakApexRT == 0 select p.Key).ToList();
            foreach (var k in toRemove) psms2.Remove(k);

            toRemove = (from p in peaks1 where p.Value.MaximumRetTime == 0 select p.Key).ToList();
            foreach (var k in toRemove) peaks1.Remove(k);

            toRemove = (from p in peaks2 where p.Value.MaximumRetTime == 0 select p.Key).ToList();
            foreach (var k in toRemove) peaks2.Remove(k);


            int featureID = 0;

            MultiRunFeatureCollection Features = new MultiRunFeatureCollection();
            ProgressIndicator P = new ProgressIndicator(psms1.Count(), "Correlating features: first pass");
            P.Start();

            foreach(var psm1 in psms1)
            {
                P.Update();
                if (psm1.Value.PeakApexRT == 0) continue;

                MultiRunFeature feature = new MultiRunFeature();

                double rt1 = psm1.Value.PeakApexRT;
                double mass1 = masses1[psm1.Value.Scan].MonoisotopicMZ;
                bool found = false;

                feature.RT1 = rt1;
                feature.Mass1 = mass1;
                feature.Ms2Scan1 = psm1.Value.Scan;
                feature.IdIn1 = true;
                feature.FoundIn1 = true;

                var closePsms2 = GetPsmBin(psms2, rt1, 0.5);

                while (true)
                {
                    if (closePsms2.Count() == 0) break;
                    int closestPSM2 = GetKeyOfClosestPsmRT(closePsms2, rt1);

                    double rt2 = closePsms2[closestPSM2].PeakApexRT;
                    double mass2 = masses2[closePsms2[closestPSM2].Scan].MonoisotopicMZ;

                    double tDiff = Math.Abs(rt1 - rt2) / (rt1 + rt2) * 2;
                    double mDiff = Math.Abs(mass1 - mass2) / (mass1 + mass2) * 2e6;

                    //foreach (var tol in new List<double> { 0.001, 0.002, 0.003, 0.004, 0.005, 0.006, 0.007 })
                    foreach (var tol in new List<double> { rtTolerance })
                    {
                        if (tDiff < tol & mDiff < massTolerance)
                        {
                            feature.IdIn2 = true;
                            feature.FoundIn2 = true;
                            feature.RT2 = rt2;
                            feature.Mass2 = mass2;
                            feature.Ms2Scan2 = psms2[closestPSM2].Scan;
                            found = true;

                            string seq1 = psm1.Value.Seq;
                            string seq2 = closePsms2[closestPSM2].Seq;
                            if (seq1 == seq2)
                            {
                                feature.ConfirmSeqMatch = true;
                            }

                            break;
                        }
                    }

                    if (found)
                    {
                        Features.Add(featureID, feature);
                        featureID++;
                        break;
                    }
                    else
                    {
                        closePsms2.Remove(closestPSM2);
                    }

                    if (closePsms2.Keys.Count() == 0)
                    {
                        break;
                    }
                }

                if (found) continue;

                // if we didn't find a PSM, continue on to looking for a matching ms2 scan
                var closePeaks2 = GetPeakBin(peaks2, rt1, 0.5);

                while (true)
                {
                    if (closePeaks2.Count() == 0) break;
                    int closestPeak2 = GetKeyOfClosestPeakRT(closePeaks2, rt1);

                    double rt2 = closePeaks2[closestPeak2].MaximumRetTime;
                    double mass2 = masses2[closePeaks2[closestPeak2].Ms2Scan].MonoisotopicMZ;

                    double tDiff = Math.Abs(rt1 - rt2) / (rt1 + rt2) * 2;
                    double mDiff = Math.Abs(mass1 - mass2) / (mass1 + mass2) * 2e6;

                    //foreach (var tol in new List<double> { 0.001, 0.002, 0.003, 0.004, 0.005, 0.006, 0.007 })
                    foreach (var tol in new List<double> { rtTolerance })
                    {
                        if (tDiff < tol & mDiff < massTolerance)
                        {
                            feature.IdIn2 = false;
                            feature.FoundIn2 = true;
                            feature.RT2 = rt2;
                            feature.Mass2 = mass2;
                            feature.Ms2Scan2 = peaks2[closestPeak2].Ms2Scan;
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        Features.Add(featureID, feature);
                        featureID++;
                        break;
                    }
                    else
                    {
                        closePeaks2.Remove(closestPeak2);
                    }

                    if (closePeaks2.Keys.Count() == 0)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    feature.IdIn2 = false;
                    feature.FoundIn2 = false;
                    Features.Add(featureID, feature);
                    featureID++;
                }
            }
            P.Done();

            // now look at file 2 psms

            var matchedAlready = (from f in Features select f.Value.Ms2Scan2).ToList();
            P = new ProgressIndicator(psms2.Count(), "Correlating features: second pass");
            P.Start();

            foreach(var psm2 in psms2)
            {
                P.Update();
                if (matchedAlready.Contains(psm2.Value.Scan)) continue;
                if (psm2.Value.PeakApexRT == 0) continue;

                MultiRunFeature feature = new MultiRunFeature();

                double rt2 = psm2.Value.PeakApexRT;
                double mass2 = masses2[psm2.Value.Scan].MonoisotopicMZ;
                bool found = false;

                feature.RT2 = rt2;
                feature.Mass2 = mass2;
                feature.Ms2Scan2 = psm2.Value.Scan;
                feature.IdIn2 = true;
                feature.FoundIn2 = true;
                

                // we don't need to look at file 1 psms, because they have already been matched to file 2 psms
                // so gp right on to file 1 peaks

                // looking for a matching ms2 scan
                var closePeaks1 = GetPeakBin(peaks1, rt2, 0.5);

                while (true)
                {
                    if (closePeaks1.Count() == 0) break;
                    int closestPeak1 = GetKeyOfClosestPeakRT(closePeaks1, rt2);

                    double rt1 = closePeaks1[closestPeak1].MaximumRetTime;
                    double mass1 = masses1[closePeaks1[closestPeak1].Ms2Scan].MonoisotopicMZ;

                    double tDiff = Math.Abs(rt1 - rt2) / (rt1 + rt2) * 2;
                    double mDiff = Math.Abs(mass1 - mass2) / (mass1 + mass2) * 2e6;

                    //foreach (var tol in new List<double> { 0.001, 0.002, 0.003, 0.004, 0.005, 0.006, 0.007 })
                    foreach (var tol in new List<double> { rtTolerance })
                    {
                        if (tDiff < tol & mDiff < massTolerance)
                        {
                            feature.IdIn1 = false;
                            feature.FoundIn1 = true;
                            feature.RT1 = rt1;
                            feature.Mass1 = mass1;
                            feature.Ms2Scan1 = peaks1[closestPeak1].Ms2Scan;
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        Features.Add(featureID, feature);
                        featureID++;
                        break;
                    }
                    else
                    {
                        closePeaks1.Remove(closestPeak1);
                    }

                    if (closePeaks1.Keys.Count() == 0)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    feature.IdIn1 = false;
                    feature.FoundIn1 = false;
                    Features.Add(featureID, feature);
                    featureID++;
                }
            }
            P.Done();
            return Features;
        }

        public static Ms1FeatureCollection AggregateMs1Features(PrecursorPeakCollection Peaks, PsmDataCollection PSMs, PrecursorMassCollection precursorMasses)
        {
            Ms1FeatureCollection features = new Ms1FeatureCollection();

            foreach (var peak in Peaks)
            {
                if (peak.Value.MaximumRetTime == 0) continue;

                Ms1Feature feature = new Ms1Feature();

                int ms2scan = peak.Value.Ms2Scan;

                feature.Peak = peak.Value;
                feature.MonoisotopicMZ = precursorMasses[ms2scan].MonoisotopicMZ;
                feature.Ms2Scan = ms2scan;

                if (PSMs.Keys.Contains(ms2scan))
                {
                    feature.PSM = PSMs[ms2scan];
                    feature.Identified = true;
                }

                features.Add(ms2scan, feature);
            }

            return features;
        }

        public static MultiRunFeatureCollection CorrelateFeatures2(Ms1FeatureCollection features1, Ms1FeatureCollection features2, double rtTolerance, double massTolerance)
        {
            MultiRunFeatureCollection MatchedFeatures = new MultiRunFeatureCollection();
            int featureID = 0;

            ProgressIndicator P = new ProgressIndicator(features1.Count(), "Correlating features, first pass");
            P.Start();

            var keys1 = features1.Keys.ToList();
            var file2ScansAlreadyMatched = new HashSet<int>();

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
                    closestFrom2 = closeFeaturesFrom2.FindClosest(rt1, mass1);
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
                }

                MatchedFeatures.Add(featureID++, multiFeature);

                //features1.Remove(key);
                P.Update();
            }
            P.Done();

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
