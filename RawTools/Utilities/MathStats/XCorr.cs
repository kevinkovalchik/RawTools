using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using RawTools.Utilities;

namespace RawTools.Utilities.MathStats
{
    static class SpectraCorrelation
    {
        public static List<double> YPrime(List<double> Intensities)
        {
            double tempSum;
            List<double> yOut = new List<double>();

            int maxIndex = Intensities.Count();

            for (int i = 0; i < Intensities.Count(); i++)
            {
                tempSum = 0;

                for (int j = -75; j < 0; j++)
                {
                    if (i + j < 0) continue;
                    else tempSum +=  Intensities[i + j];
                }

                for (int j = 1; j <= 75; j++)
                {
                    if (i + j >= maxIndex) continue;
                    else tempSum +=  Intensities[i + j];
                }

                yOut.Add(Intensities[i] - tempSum/150);
            }

            return yOut;
        }

        /// <summary>
        /// Bins intensity values by mass and returns the binned intensities.
        /// </summary>
        /// <param name="Masses"></param>
        /// <param name="Intensities"></param>
        /// <param name="BinWidth"></param>
        /// <param name="BinOffset"></param>
        /// <param name="LoMass"></param>
        /// <param name="HiMass"></param>
        /// <returns></returns>
        public static List<double> BinData(double[] Masses, double[] Intensities, double BinWidth,
            double BinOffset, double LoMass, double HiMass, bool SqrtIntensity = true)
        {
            List<double> binsOut = new List<double>();

            double currentLeft = LoMass - (BinWidth - BinOffset);
            double currentRight = currentLeft + BinWidth;
            int i = 0;
            int numBins = Convert.ToInt32(Math.Ceiling((HiMass - LoMass) / BinWidth) + 1);

            for (int j = 0; j < numBins; j++)
            {
                List<double> intensitiesIn = new List<double>();

                while (i < Masses.Length)
                {
                    if (Masses[i] >= currentLeft & Masses[i] < currentRight)
                    {
                        intensitiesIn.Add(Intensities[i]);
                    }
                    if (Masses[i] >= currentRight)
                    {
                        break;
                    }
                    i++;
                }

                if (intensitiesIn.Count() == 0)
                {
                    binsOut.Add(0);
                }
                else
                {
                    if (SqrtIntensity)
                    {
                        binsOut.Add(Math.Sqrt(intensitiesIn.Max()));
                    }
                    else
                    {
                        binsOut.Add(intensitiesIn.Max());
                    }
                }

                currentLeft += BinWidth;
                currentRight += BinWidth;
            }

            return binsOut;
        }

        public static List<double> BinData2(double[] Masses, double[] Intensities, double BinWidth,
            double BinOffset, double LoMass, double HiMass)
        {
            List<double> binsOut = new List<double>();
            List<double> currentBin = new List<double>();

            double currentLeft = LoMass - (BinWidth - BinOffset);
            double currentRight = currentLeft + BinWidth;
            int i = 0;
            int numBins = Convert.ToInt32(Math.Ceiling((HiMass - LoMass) / BinWidth) + 1);

            //for (int j = 0; j < 75; j++) binsOut.Add(0);
            
            while (i < Masses.Count())
            {
                if (Masses[i] < LoMass)
                {
                    i++;
                    continue;
                }
                if (Masses[i] > currentLeft & Masses[i] <= currentRight)
                {
                    currentBin.Add(Intensities[i]);
                    i++;
                    continue;
                }
                if (Masses[i] > HiMass)
                {
                    if (currentBin.Count() > 0)
                    { binsOut.Add(currentBin.Max()); }
                    else
                    { binsOut.Add(0); }

                    break;
                }

                if (currentBin.Count() > 0)
                { binsOut.Add(currentBin.Max()); }
                else
                { binsOut.Add(0); }
                currentLeft += BinWidth;
                currentRight += BinWidth;
                currentBin = new List<double>();
            }

            while (binsOut.Count() < numBins)
            {
                binsOut.Add(0);
            }

            for (int j = 0; j < 75; j++) binsOut.Add(0);
            for (int j = 0; j < 75; j++) binsOut.Insert(0, 0);

            return binsOut;
        }

        public static List<double> BinData2(List<double> Masses, List<double> Intensities, double BinWidth,
            double BinOffset, double LoMass, double HiMass)
        {
            List<double> binsOut = new List<double>();
            List<double> currentBin = new List<double>();

            double currentLeft = LoMass - (BinWidth - BinOffset);
            double currentRight = currentLeft + BinWidth;
            int i = 0;
            int numBins = Convert.ToInt32(Math.Ceiling((HiMass - LoMass) / BinWidth) + 1);

            //for (int j = 0; j < 75; j++) binsOut.Add(0);

            while (i < Masses.Count())
            {
                if (Masses[i] < LoMass)
                {
                    i++;
                    continue;
                }
                if (Masses[i] > currentLeft & Masses[i] <= currentRight)
                {
                    currentBin.Add(Intensities[i]);
                    i++;
                    continue;
                }
                if (Masses[i] > HiMass)
                {
                    if (currentBin.Count() > 0)
                    { binsOut.Add(currentBin.Max()); }
                    else
                    { binsOut.Add(0); }

                    break;
                }

                if (currentBin.Count() > 0)
                { binsOut.Add(currentBin.Max()); }
                else
                { binsOut.Add(0); }
                currentLeft += BinWidth;
                currentRight += BinWidth;
                currentBin = new List<double>();
            }

            while (binsOut.Count() < numBins)
            {
                binsOut.Add(0);
            }

            for (int j = 0; j < 75; j++) binsOut.Add(0);
            for (int j = 0; j < 75; j++) binsOut.Insert(0, 0);

            return binsOut;
        }

        public static double CalcXCorr(List<double> x0, List<double> yPrime)
        {
            double score = x0.Zip(yPrime, (a, b) => a * b).Sum();

            return score;
        }

        /// <summary>
        /// Break intensity array into N portions normalized to a uniform maximum. Expects to recieve a binned intensity array.
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="N"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        public static List<double> Normalize(this List<double> Values, int N = 10, int Max = 50)
        {
            double normFactor;
            int len = Values.Count() / N;

            List<double> normalized = new List<double>();

            for (int i = 0; i < N; i++)
            {
                List<double> bin = new List<double>();

                for (int j = 0; j < len; j++)
                {
                    bin.Add(Values[i * len + j]);
                }

                if (bin.Max() > 0)
                {
                    normFactor = 50 / bin.Max();
                }
                else
                {
                    normFactor = 0;
                }

                for (int j = 0; j < len; j++)
                {
                    normalized.Add(bin[j] * normFactor);
                }
            }

            int numLeft = Values.Count() - len * N;

            if (numLeft == 0)
            {
                return normalized;
            }
            else
            {
                List<double> bin = new List<double>();

                for (int j = 0; j < numLeft; j++)
                {
                    bin.Add(Values[N * len + j]);
                }

                if (bin.Max() > 0)
                {
                    normFactor = 50 / bin.Max();
                }
                else
                {
                    normFactor = 0;
                }

                for (int j = 0; j < numLeft; j++)
                {
                    normalized.Add(bin[j] * normFactor);
                }

                return normalized;
            }
        }

        public static List<double> MedianNormalize(this List<double> Values)
        {
            double median = Values.Percentile(50);

            List<double> normalized = new List<double>();

            for (int i = 0; i < Values.Count(); i++)
            {
                normalized.Add(Values[i] / median);
            }

            return normalized;
        }

        public static double ScoreMultiRunSpectra(SimpleCentroid scan1, SimpleCentroid scan2)
        {
            var y = BinData2(scan1.Masses.ToArray(), scan1.Intensities.ToArray(), 1.005, 0.4, 132, 1000);
            var x = BinData2(scan2.Masses.ToArray(), scan2.Intensities.ToArray(), 1.005, 0.4, 132, 1000);

            y = y.Normalize();
            x = x.Normalize();

            var yPrime = YPrime(y);

            return CalcXCorr(x, yPrime);
        }

        public static double ScoreMultiRunSpectra(SegmentedScanData scan1, SegmentedScanData scan2)
        {
            var y = BinData2(scan1.Positions.ToArray(), scan1.Intensities.ToArray(), 1.005, 0.4, 132, 1000);
            var x = BinData2(scan2.Positions.ToArray(), scan2.Intensities.ToArray(), 1.005, 0.4, 132, 1000);

            //y = y.Normalize();
            //x = x.Normalize();

            //y = y.MedianNormalize();
            //x = x.MedianNormalize();

            //var yPrime = YPrime(y);

            //return CalcXCorr(x, yPrime);

            //return MathStats.FitScores.GetBhattacharyyaDistance(x.ToArray(), y.ToArray());
            return MathStats.FitScores.ModifiedSteinScott(x.ToArray(), y.ToArray());
            //return MathStats.FitScores.GetCosine(x.ToArray(), y.ToArray());
            //return MathStats.FitScores.GetPearsonCorrelation(x.ToArray(), y.ToArray());
        }

        public static void ScoreMultiRunSpectra(MultiRunFeatureCollection features, SegmentScanCollection scans1, SegmentScanCollection scans2)
        {
            ProgressIndicator P = new ProgressIndicator(features.Count(), "Calculating MS2 spectra correlation");
            P.Start();

            foreach (var feature in features)
            {
                if (! (feature.Value.FoundIn1 & feature.Value.FoundIn2))
                {
                    feature.Value.XCorr = 0;
                    P.Update();
                    continue;
                }
                var scan1 = scans1[feature.Value.Ms2Scan1];
                var scan2 = scans2[feature.Value.Ms2Scan2];

                feature.Value.XCorr = ScoreMultiRunSpectra(scan1, scan2);
                P.Update();
            }
            P.Done();
        }
    }
}
