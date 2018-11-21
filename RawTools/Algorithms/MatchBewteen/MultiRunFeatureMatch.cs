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
    static class MultiRunFeatureMatch
    {
        public static MultiRunFeatureCollection MatchFeatures(Dictionary<string, Ms1FeatureCollection> SingleRunFeatures, double massTolerance, double rtTolerance)
        {
            MultiRunFeatureCollection features = new MultiRunFeatureCollection();
            List<string> runNames = SingleRunFeatures.Keys.ToList();

            Dictionary<string, int> runIDs = new Dictionary<string, int>();
            for (int i = 0; i < runNames.Count(); i++) runIDs.Add(runNames[i], i);

            string self;
            double selfMass, otherMass, selfRT, otherRT;
            HashSet<string> others;
            SingleFeatureMatchData featureMatch;
            //HashSet<int> selfsAlreadyMatched;
            HashSet<int> othersAlreadyMatched;

            foreach (var run in runNames)
            {
                // set the name of this run
                self = run;

                // set the names of the other runs, ensuring self is not in the list
                others = new HashSet<string>(runNames);
                others.Remove(self);

                // begin feature matching with self
                ProgressIndicator P = new ProgressIndicator(SingleRunFeatures[self].Count() - 1, String.Format("Self-matching {0}", self));
                P.Start();
                //selfsAlreadyMatched = new HashSet<int>();

                foreach (var thisFeature in SingleRunFeatures[self].Values)
                {
                    featureMatch = new SingleFeatureMatchData();
                    selfMass = thisFeature.MonoisotopicMZ;
                    selfRT = thisFeature.Peak.MaximumRetTime;

                    featureMatch = thisFeature.MatchFeature(SingleRunFeatures[self], rtTolerance, massTolerance);
                    P.Update();
                }
                P.Done();
            }

            return features;
        }

        public static void AddFirstFeature(this GroupedFeatureCollection Features, Ms1Feature Feature, string Run)
        {
            if (Features.Count() != 0)
            {
                return;
            }

            // create new feature group
            GroupedMs1Feature featureGroup = new GroupedMs1Feature();

            // add the feature to it
            featureGroup.Add((Run: Run, Ms2Scan: Feature.Ms2Scan), Feature);

            // add the group to the collection
            Features.Add(new Guid(), featureGroup);
        }

        public static void AddAdditionalFeature(this GroupedFeatureCollection Features, Ms1Feature Feature, string Run)
        {

        }

        public static GroupedFeatureCollection GroupSameRunFeatures(Ms1FeatureCollection SingleRunFeatures, string Run, double massTolerance, double rtTolerance)
        {
            GroupedFeatureCollection features = new GroupedFeatureCollection();

            HashSet<int> keys = new HashSet<int>(SingleRunFeatures.Keys);

            // add the first feature to the grouped features collection
            features.AddFirstFeature(SingleRunFeatures.First().Value, Run);

            // delete it from the list of keys
            keys.Remove(SingleRunFeatures.First().Key);



            return features;
        }

        public static SingleFeatureMatchData MatchFeature(this Ms1Feature self, Ms1FeatureCollection others, double rtTolerance, double massTolerance)
        {
            double score;
            int closestOtherScan;

            SingleFeatureMatchData featureMatch = new SingleFeatureMatchData();

            double mass1 = self.MonoisotopicMZ;
            double rt1 = self.Peak.MaximumRetTime;
            featureMatch.PickedInSelf = true;
            featureMatch.IdInSelf = self.Identified;

            featureMatch.RtSelf = rt1;
            featureMatch.MassSelf = mass1;
            featureMatch.Ms2ScanSelf = self.Ms2Scan;

            Ms1FeatureCollection closeFromOthers = MatchBetween.GetFeatureBin(others, rt1, mass1, rtTolerance, massTolerance);
            // if matched to self, remove self
            if (closeFromOthers.Keys.Contains(self.Ms2Scan)) closeFromOthers.Remove(self.Ms2Scan);

            Ms1Feature closestFromOther = null;

            // see if there is one or more matches
            if (closeFromOthers.Count() > 0)
            {
                foreach (var close in closeFromOthers.Values)
                {
                    score = FitScores.ModifiedSteinScott(self, close);

                    featureMatch.AllScores.Add((featureMatch.Ms2ScanSelf, close.Ms2Scan), score);
                }
            }
            // if not, return now
            else
            {
                return featureMatch;
            }

            // get the high score and the corresponding scan
            if (featureMatch.AllScores.Count() > 1)
            {
                score = -100;
                foreach (var item in featureMatch.AllScores)
                {
                    if (item.Value > score)
                    {
                        score = item.Value;
                        closestOtherScan = item.Key.scanOther;
                        closestFromOther = closeFromOthers[closestOtherScan];
                    }
                }
            }
            else
            {
                score = featureMatch.AllScores.First().Value;
                closestOtherScan = featureMatch.AllScores.First().Key.scanOther;
                closestFromOther = closeFromOthers[closestOtherScan];
            }

            featureMatch.Score = score;

            featureMatch.PickedInOther = true;
            featureMatch.IdInOther = closestFromOther.Identified;

            featureMatch.RtOther = closestFromOther.Peak.MaximumRetTime;
            featureMatch.MassOther = closestFromOther.MonoisotopicMZ;
            featureMatch.Ms2ScanOther = closestFromOther.Ms2Scan;

            //features2.Remove(closestFrom2.Ms2Scan);

            if (self.Identified == true & closestFromOther.Identified == true) 
            {
                if (self.PSM.Seq == closestFromOther.PSM.Seq) featureMatch.ConfirmSeqMatch = true;
            }

            return featureMatch;
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

                Ms1FeatureCollection closeFeaturesFrom2 = MatchBetween.GetFeatureBin(features2, rt1, mass1, rtTolerance, massTolerance);
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

                    multiFeature.XCorr = multiFeature.AllScores.Values.Max();

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
