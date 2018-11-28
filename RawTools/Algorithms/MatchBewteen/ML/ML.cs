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
using RawTools.Algorithms.MatchBewteen;
using ThermoFisher.CommonCore.Data.FilterEnums;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Runtime.Learners;
using Microsoft.ML.Transforms.Conversions;

namespace RawTools.Algorithms.MatchBewteen.ML
{
    public class MatchInfo
    {
        //[Column(ordinal: "0", name: "Label")]
        public bool Match;

        //[Column(ordinal: "1")]
        public float SteinScott;
        //[Column(ordinal: "2")]
        public float XCorr;
        //[Column(ordinal: "3")]
        public float Pearsons;
        //[Column(ordinal: "4")]
        public float RtErr;
        //[Column(ordinal: "5")]
        public float MassErr;
        //[Column(ordinal: "6")]
        public float Bhattacharyya;
    }

    public class MatchPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }
    }

    class Run
    {
        public static (List<MatchInfo> Train, List<MatchInfo> Test) FormatData(GroupedFeatureCollection GroupedData)
        {
            List<MatchInfo> dataOut = new List<MatchInfo>();
            List<MatchInfo> trainingSet = new List<MatchInfo>();
            List<MatchInfo> testingSet = new List<MatchInfo>();

            ProgressIndicator P = new ProgressIndicator(GroupedData.Count(), "Building training set, first pass");
            P.Start();
            // first add the good matches
            foreach (var feat in GroupedData)
            {
                bool run1 = false;
                bool run2 = false;

                foreach (var scanevent in feat.Value)
                {
                    if (scanevent.Key.Run == "Run1") run1 = true;
                    if (scanevent.Key.Run == "Run2") run2 = true;
                }

                if (!(run1 & run2)) { P.Update(); continue; }

                if ((from x in feat.Value.Values where x.Identified select 1).Count() < 1) { P.Update(); continue; }

                string seq = string.Empty;

                foreach (var scan in feat.Value)
                {
                    if (!scan.Value.Identified) continue;

                    foreach (var otherScan in feat.Value)
                    {
                        if (!otherScan.Value.Identified) continue;

                        if ((scan.Key.Run == otherScan.Key.Run) & (scan.Key.Ms2Scan == otherScan.Key.Ms2Scan)) continue;

                        if (scan.Value.PSM.Seq == otherScan.Value.PSM.Seq)
                        {
                            var A = scan.Value;
                            var B = otherScan.Value;
                            var aArr = A.BinnedMs2Intensities.ToArray();
                            var bArr = B.BinnedMs2Intensities.ToArray();

                            MatchInfo newMatch = new MatchInfo();

                            newMatch.Match = true;
                            newMatch.MassErr = (float)(Math.Abs(A.MonoisotopicMZ - B.MonoisotopicMZ) / (A.MonoisotopicMZ + B.MonoisotopicMZ) * 2e6);
                            newMatch.RtErr = (float)(Math.Abs(A.Peak.MaximumRetTime - B.Peak.MaximumRetTime) / (A.Peak.MaximumRetTime - B.Peak.MaximumRetTime) * 2);
                            newMatch.SteinScott = (float)FitScores.ModifiedSteinScott(A, B);
                            newMatch.Pearsons = (float)FitScores.GetPearsonCorrelation(aArr, bArr);
                            newMatch.Bhattacharyya = (float)FitScores.GetBhattacharyyaDistance(aArr, bArr);
                            newMatch.XCorr = (float)SpectraCorrelation.CalcXCorr(SpectraCorrelation.Normalize(B.BinnedMs2Intensities), SpectraCorrelation.YPrime(SpectraCorrelation.Normalize(A.BinnedMs2Intensities)));
                            dataOut.Add(newMatch);
                        }
                    }
                }
                P.Update();
            }
            P.Done();
            int total = dataOut.Count();

            // and now the bad (random) matches
            Random rand = new Random();
            P = new ProgressIndicator(total, "Building training set, second pass");
            P.Start();
            for (int i = 0; i < total; i++)
            {
                while (true)
                {
                    int n1 = rand.Next(0, GroupedData.Count());
                    int n2 = rand.Next(0, GroupedData.ElementAt(n1).Value.Count());

                    if (!GroupedData.ElementAt(n1).Value.ElementAt(n2).Value.Identified) continue;

                    var closeMasses = MultiRunFeatureMatch.GetNearbyFeatures(GroupedData, GroupedData.ElementAt(n1).Value.ElementAt(n2).Value, 1, 10);
                    var closeTimes = MultiRunFeatureMatch.GetNearbyFeatures(GroupedData, GroupedData.ElementAt(n1).Value.ElementAt(n2).Value, 0.005, 1000000);
                    
                    var A = GroupedData.ElementAt(n1).Value.ElementAt(n2).Value;

                    foreach (var match in closeMasses)
                    {
                        var B = match;

                        if (!(A.Identified & B.Identified)) continue;
                        if (A.PSM.Seq == B.PSM.Seq) continue;
                        var aArr = A.BinnedMs2Intensities.ToArray();
                        var bArr = B.BinnedMs2Intensities.ToArray();

                        MatchInfo newMatch = new MatchInfo();

                        newMatch.Match = false;
                        newMatch.MassErr = (float)(Math.Abs(A.MonoisotopicMZ - B.MonoisotopicMZ) / (A.MonoisotopicMZ + B.MonoisotopicMZ) * 2e6);
                        newMatch.RtErr = (float)(Math.Abs(A.Peak.MaximumRetTime - B.Peak.MaximumRetTime) / (A.Peak.MaximumRetTime - B.Peak.MaximumRetTime) * 2);
                        newMatch.SteinScott = (float)FitScores.ModifiedSteinScott(A, B);
                        newMatch.Pearsons = (float)FitScores.GetPearsonCorrelation(aArr, bArr);
                        newMatch.Bhattacharyya = (float)FitScores.GetBhattacharyyaDistance(aArr, bArr);
                        newMatch.XCorr = (float)SpectraCorrelation.CalcXCorr(SpectraCorrelation.Normalize(B.BinnedMs2Intensities), SpectraCorrelation.YPrime(SpectraCorrelation.Normalize(A.BinnedMs2Intensities)));
                        dataOut.Add(newMatch);
                    }

                    foreach (var match in closeTimes)
                    {
                        var B = match;

                        if (!(A.Identified & B.Identified)) continue;
                        if (A.PSM.Seq == B.PSM.Seq) continue;
                        var aArr = A.BinnedMs2Intensities.ToArray();
                        var bArr = B.BinnedMs2Intensities.ToArray();

                        MatchInfo newMatch = new MatchInfo();

                        newMatch.Match = false;
                        newMatch.MassErr = (float)(Math.Abs(A.MonoisotopicMZ - B.MonoisotopicMZ) / (A.MonoisotopicMZ + B.MonoisotopicMZ) * 2e6);
                        newMatch.RtErr = (float)(Math.Abs(A.Peak.MaximumRetTime - B.Peak.MaximumRetTime) / (A.Peak.MaximumRetTime - B.Peak.MaximumRetTime) * 2);
                        newMatch.SteinScott = (float)FitScores.ModifiedSteinScott(A, B);
                        newMatch.Pearsons = (float)FitScores.GetPearsonCorrelation(aArr, bArr);
                        newMatch.Bhattacharyya = (float)FitScores.GetBhattacharyyaDistance(aArr, bArr);
                        newMatch.XCorr = (float)SpectraCorrelation.CalcXCorr(SpectraCorrelation.Normalize(B.BinnedMs2Intensities), SpectraCorrelation.YPrime(SpectraCorrelation.Normalize(A.BinnedMs2Intensities)));
                        dataOut.Add(newMatch);
                    }
                    
                    break;
                }
                P.Update();
            }
            P.Done();

            Console.WriteLine("==========================");
            Console.WriteLine("Training set:");
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Matches: {total}");
            Console.WriteLine($"Non-matches: {dataOut.Count() - total}");
            Console.WriteLine("==========================");

            // now split them up into training and testing sets
            int numTrainTest = (int)Math.Floor((double)total / 2);
            //int top = dataOut.Count();

            for (int i = 0; i < numTrainTest; i++)
            {
                int x = rand.Next(0, dataOut.Count());
                trainingSet.Add(dataOut[x]);
                dataOut.RemoveAt(x);

                x = rand.Next(0, dataOut.Count());
                testingSet.Add(dataOut[x]);
                dataOut.RemoveAt(x);
            }

            return (Train: trainingSet, Test: testingSet);
        }

        public static void Go(GroupedFeatureCollection GroupedData)
        {
            List<MatchInfo> train;
            List<MatchInfo> test;

            MLContext mlContext = new MLContext(seed: 0);

            (train, test) = FormatData(GroupedData);

            var trainData = mlContext.CreateDataView(train);
            var testData = mlContext.CreateDataView(test);

            var pipeline = mlContext.Transforms.Concatenate("Features", "SteinScott", "XCorr", "Pearsons", "Bhattacharyya", "RtErr", "MassErr")
                .Append(mlContext.BinaryClassification.Trainers.FastTree("Match", numLeaves: 50, numTrees: 50, minDatapointsInLeafs: 20));
            
            var model = pipeline.Fit(trainData);

            var predictions = model.Transform(testData);

            var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Match");

            Console.WriteLine();
            Console.WriteLine("=======================================================");
            Console.WriteLine("Model quality metrics evaluation:");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"Positive precision: {metrics.PositivePrecision}");
            Console.WriteLine($"Positive recall: {metrics.PositiveRecall}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
            Console.ReadKey();
        }

    }
}
