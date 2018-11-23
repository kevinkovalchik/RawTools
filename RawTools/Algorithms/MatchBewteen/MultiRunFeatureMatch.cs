using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        public static void AddNewFeature(this GroupedFeatureCollection Features, Ms1Feature Feature, string Run)
        {
            // create new feature group
            GroupedMs1Feature featureGroup = new GroupedMs1Feature();

            // add the feature to it
            featureGroup.Add((Run: Run, Ms2Scan: Feature.Ms2Scan), Feature);

            // update average mass and RT
            featureGroup.UpdateAverageMassAndRT();

            // add the group to the collection
            Features.Add(Guid.NewGuid(), featureGroup);
        }

        public static void AddToExistingGroup(this GroupedFeatureCollection Features, Ms1Feature Feature, Guid Key, string Run)
        {
            Features[Key].Add((Run: Run, Ms2Scan: Feature.Ms2Scan), Feature);
            Features[Key].UpdateAverageMassAndRT();
        }

        public static void AddAdditionalFeature(this GroupedFeatureCollection Features, Ms1Feature Feature, string Run, double RtTol, double MassTol)
        {
            Guid featureKey = Features.GetClosestFeatureGroupIndex(Feature, RtTol, MassTol);

            if (featureKey == Guid.Empty)
            {
                Features.AddNewFeature(Feature, Run);
            }
            else
            {
                Features.AddToExistingGroup(Feature, featureKey, Run);
            }
        }

        public static Guid GetClosestFeatureGroupIndex(this GroupedFeatureCollection GroupedFeatures, Ms1Feature Feature, double RtTol, double MassTol)
        {
            //List<Guid> closeFeatures = new List<Guid>();
            ConcurrentDictionary<Guid, double> RtErrors = new ConcurrentDictionary<Guid, double>();
            ConcurrentDictionary<Guid, double> MassErrors = new ConcurrentDictionary<Guid, double>();

            //Dictionary<Guid, double> RtErrors = new Dictionary<Guid, double>();
            //Dictionary<Guid, double> MassErrors = new Dictionary<Guid, double>();
            double mass, rt, massErr, rtErr, topScore, currentScore;
            Guid index = Guid.Empty;

            // find the "nearby" features
            /*
            foreach (var feature in GroupedFeatures)
            {
                mass = feature.Value.AverageMonoIsoMZ;
                rt = feature.Value.AverageRT;
                massErr = Math.Abs(Feature.MonoisotopicMZ - mass) / mass * 1e6;
                rtErr = Math.Abs(Feature.RT - rt) / rt;

                if ((massErr < MassTol) & (rtErr < RtTol))
                {
                    //closeFeatures.Add(feature.Key);
                    RtErrors.Add(feature.Key, rtErr);
                    MassErrors.Add(feature.Key, massErr);
                }
            }*/
            
            Parallel.ForEach(GroupedFeatures, (feature) =>
            {
                mass = feature.Value.AverageMonoIsoMZ;
                rt = feature.Value.AverageRT;
                massErr = Math.Abs(Feature.MonoisotopicMZ - mass) / mass * 1e6;
                rtErr = Math.Abs(Feature.RT - rt) / rt;

                if ((massErr < MassTol) & (rtErr < RtTol))
                {
                    //closeFeatures.Add(feature.Key);
                    RtErrors.TryAdd(feature.Key, rtErr);
                    MassErrors.TryAdd(feature.Key, massErr);
                }
            }
            );

            // check if it is empty
            if (RtErrors.Count() == 0)
            {
                return index;
            }

            // check if there is only one candidate
            if (RtErrors.Count() == 1)
            {
                return RtErrors.Keys.First();
            }

            // if more than one, find the "closest" feature
            topScore = 0;
            currentScore = 0;
            double rtErrMax = RtErrors.Values.Max();
            double massErrMax = MassErrors.Values.Max();

            foreach (var guid in RtErrors.Keys)
            {
                currentScore = RtErrors[guid] / rtErrMax + MassErrors[guid] / massErrMax;

                if (currentScore > topScore)
                {
                    topScore = currentScore;
                    index = guid;
                }
            }

            return index;
        }

        public static GroupedFeatureCollection GroupSameRunFeatures(Ms1FeatureCollection SingleRunFeatures, string Run, double massTolerance, double rtTolerance)
        {
            GroupedFeatureCollection features = new GroupedFeatureCollection();

            HashSet<int> keys = new HashSet<int>(SingleRunFeatures.Keys);

            // add the first feature to the grouped features collection
            features.AddNewFeature(SingleRunFeatures.First().Value, Run);

            // delete it from the list of keys
            keys.Remove(SingleRunFeatures.First().Key);

            ProgressIndicator P = new ProgressIndicator(keys.Count(), "Grouping Ms2 scans into Ms1 features");
            P.Start();
            foreach (var key in keys)
            {
                features.AddAdditionalFeature(SingleRunFeatures[key], Run, rtTolerance, massTolerance);
                P.Update();
            }
            P.Done();

            return features;
        }
        
        public static Guid GetClosestFeatureGroupIndex(this GroupedFeatureCollection GroupedFeatures, GroupedMs1Feature Feature, double RtTol, double MassTol)
        {
            ConcurrentDictionary<Guid, double> RtErrors = new ConcurrentDictionary<Guid, double>();
            ConcurrentDictionary<Guid, double> MassErrors = new ConcurrentDictionary<Guid, double>();
            
            double mass, rt, massErr, rtErr, topScore, currentScore;
            Guid index = Guid.Empty;

            // find the "nearby" features
            Parallel.ForEach(GroupedFeatures, (candidateFeature) =>
            {
                mass = candidateFeature.Value.AverageMonoIsoMZ;
                rt = candidateFeature.Value.AverageRT;
                massErr = Math.Abs(Feature.AverageMonoIsoMZ - mass) / mass * 1e6;
                rtErr = Math.Abs(Feature.AverageRT - rt) / rt;

                if ((massErr < MassTol) & (rtErr < RtTol))
                {
                    //closeFeatures.Add(feature.Key);
                    RtErrors.TryAdd(candidateFeature.Key, rtErr);
                    MassErrors.TryAdd(candidateFeature.Key, massErr);
                }
            }
            );

            // check if it is empty
            if (RtErrors.Count() == 0)
            {
                return index;
            }

            // check if there is only one candidate
            if (RtErrors.Count() == 1)
            {
                return RtErrors.Keys.First();
            }

            // if more than one, find the "closest" feature
            topScore = 0;
            currentScore = 0;
            double rtErrMax = RtErrors.Values.Max();
            double massErrMax = MassErrors.Values.Max();

            foreach (var guid in RtErrors.Keys)
            {
                currentScore = RtErrors[guid] / rtErrMax + MassErrors[guid] / massErrMax;

                if (currentScore > topScore)
                {
                    topScore = currentScore;
                    index = guid;
                }
            }

            return index;
        }

        public static void CombineGroups(this GroupedMs1Feature GroupA, GroupedMs1Feature GroupB)
        {
            foreach (var group in GroupB)
            {
                GroupA.Add(group.Key, group.Value);
            }
        }

        public static void AddNewGroup(this GroupedFeatureCollection Features, KeyValuePair<Guid, GroupedMs1Feature> Group)
        {
            Features.Add(Group.Key, Group.Value);
        }

        public static void AddToExistingGroup(this GroupedFeatureCollection Features, GroupedMs1Feature NewGroup, Guid Key)
        {
            Features[Key].CombineGroups(NewGroup);
        }

        public static void GroupMultipleRunFeatures(this GroupedFeatureCollection RunA, GroupedFeatureCollection RunB, double RtTol, double MassTol)
        {
            ProgressIndicator P = new ProgressIndicator(RunB.Count(), "Grouping multiple run features");
            P.Start();
            foreach (var group in RunB)
            {
                Guid index = GetClosestFeatureGroupIndex(RunA, group.Value, RtTol, MassTol);

                if (index == Guid.Empty)
                {
                    RunA.AddNewGroup(group);
                }
                else
                {
                    RunA.AddToExistingGroup(group.Value, index);
                }

                P.Update();
            }
            P.Done();
        }

        public static GroupedFeatureCollection ProcessAll(List<Ms1FeatureCollection> Runs, List<string> RunNames, double RtTol, double MassTol)
        {
            var groupedFeatures = GroupSameRunFeatures(Runs[0], RunNames[0], MassTol, RtTol);

            for (int i = 1; i < Runs.Count(); i++)
            {
                groupedFeatures.GroupMultipleRunFeatures(GroupSameRunFeatures(Runs[i], RunNames[i], MassTol, RtTol), RtTol, MassTol);
            }

            return groupedFeatures;
        }
    }
}
