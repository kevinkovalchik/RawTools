using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawTools.QC;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.WorkFlows;

namespace RawTools.Algorithms.MatchBewteen
{
    static class AlignRetentionTimes
    {
        public static void AlignRT(Ms1FeatureCollection Features1, Ms1FeatureCollection Features2, SegmentScanCollection scans1, SegmentScanCollection scans2, double ExpectationValueCutoff)
        {
            var features1 = from x in Features1
                            where x.Value.Identified
                            select x;

            features1 = from x in features1
                        where x.Value.PSM.ExpectationValue < ExpectationValueCutoff
                        select x;

            var features2 = from x in Features2
                            where x.Value.Identified
                            select x;

            features2 = from x in features2
                        where x.Value.PSM.ExpectationValue < ExpectationValueCutoff
                        select x;

            Ms1FeatureCollection anchors1 = new Ms1FeatureCollection();
            Ms1FeatureCollection anchors2 = new Ms1FeatureCollection();

            foreach (var feature in features1) anchors1.Add(feature.Key, feature.Value);
            foreach (var feature in features2) anchors2.Add(feature.Key, feature.Value);

            MultiRunFeatureCollection matchedAnchors = MatchBetween.CorrelateFeatures2(anchors1, anchors2, scans1, scans2, 0.05, 20);

            var filteredAnchors = from x in matchedAnchors
                                  where x.Value.ConfirmSeqMatch
                                  select x;

            double[] t1 = (from x in filteredAnchors select x.Value.RT1).ToArray();
            double[] t2 = (from x in filteredAnchors select x.Value.RT2).ToArray();
            double[] tDiff = new double[t1.Length];

            for (int i = 0; i < t1.Length; i++) tDiff[i] = t2[i] - t1[i];

            Console.WriteLine("Number of trainers: {0}", t1.Length);
            //StreamWriter f = new StreamWriter("Training set.csv");
            //f.WriteLine("RT1,RT2");
            //for (int i = 0; i < t1.Length; i++)
            //{
            //    f.WriteLine("{0},{1}", t1[i], t2[i]);
            //}
            //f.Close();

            ProgressIndicator P = new ProgressIndicator(features2.Count(), "Aligning retention times");
            P.Start();

            int order;

            if (t1.Length > 100) order = t1.Length / 30;
            else order = 1;

            //var pars = MathNet.Numerics.Fit.Polynomial(t1, t2, order);
            var pars = MathNet.Numerics.Fit.Polynomial(t2, tDiff, order);

            //double[] alignedT1 = new double[t1.Length];
            //double[] alignedT2 = new double[t2.Length];

            //for (int i = 0; i < t1.Length; i++)
            //{
            //    t1[i] = MathNet.Numerics.Polynomial.Evaluate(t1[i], pars);
            //    alignedT2[i] = t2[i];
            //}

            foreach (var feature in Features2)
            {
                //feature.Value.Peak.MaximumRetTime = MathNet.Numerics.Polynomial.Evaluate(feature.Value.Peak.MaximumRetTime, pars);
                feature.Value.Peak.MaximumRetTime = feature.Value.Peak.MaximumRetTime - MathNet.Numerics.Polynomial.Evaluate(feature.Value.Peak.MaximumRetTime, pars);
                P.Update();
            }
            P.Done();
        }
    }
}
