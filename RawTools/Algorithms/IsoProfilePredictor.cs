// This code is taken from from InformedProteomics from the compomics group. https://github.com/PNNL-Comp-Mass-Spec/Informed-Proteomics
// Informed Proteomics is licensed under the Apache License, Version 2.0
// Written by Sangtae Kim, Junkap Park, and Chris Wilkins for the Department of Energy (PNNL, Richland, WA) Copyright 2015, Battelle Memorial Institute. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawTools.Utilities;

namespace RawTools.Algorithms
{
    /// <summary>
    /// Isotopic profile predictor
    /// </summary>
    public class IsoProfilePredictor
    {
        /// <summary>
        /// Max number of isotopes to predict
        /// </summary>
        public int MaxNumIsotopes { get; set; }

        /// <summary>
        /// The relative intensity threshold
        /// </summary>
        public double IsotopeRelativeIntensityThreshold { get; set; }

        /// <summary>
        /// Get the Isotopomer envelope for the provided atom counts
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="n"></param>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IsotopomerEnvelope GetIsotopomerEnvelop(int c, int h, int n, int o, int s)
        {
            return Predictor.GetIsotopomerEnvelope(c, h, n, o, s);
        }

        /// <summary>
        /// Get the Isotopomer envelope for the provided atom counts
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="n"></param>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public IsotopomerEnvelope GetIsotopomerEnvelope(int c, int h, int n, int o, int s)
        {
            var dist = new double[MaxNumIsotopes];
            var means = new double[_possibleIsotopeCombinations[0][0].Length + 1];
            var exps = new double[means.Length];
            for (var i = 0; i < means.Length; i++) // precalculate means and thier exps
            {
                means[i] = c * ProbC[i] + h * ProbH[i] + n * ProbN[i] + o * ProbO[i] + s * ProbS[i];
                exps[i] = Math.Exp(means[i]);
            }

            // This assumes that the envelop is unimodal.
            var maxHeight = 0.0;
            var isotopeIndex = 0;
            var mostIntenseIsotopomerIndex = -1;
            for (; isotopeIndex < MaxNumIsotopes; isotopeIndex++)
            {
                foreach (var isopeCombinations in _possibleIsotopeCombinations[isotopeIndex])
                {
                    dist[isotopeIndex] += GetIsotopeProbability(isopeCombinations, means, exps);
                }
                if (Double.IsInfinity(dist[isotopeIndex]))
                {
                    throw new NotFiniteNumberException();
                }
                if (dist[isotopeIndex] > maxHeight)
                {
                    maxHeight = dist[isotopeIndex];
                    mostIntenseIsotopomerIndex = isotopeIndex;
                }
                else if (dist[isotopeIndex] < maxHeight * IsotopeRelativeIntensityThreshold)
                {
                    break;
                }
            }

            var truncatedDist = new double[isotopeIndex];
            for (var i = 0; i < isotopeIndex; i++)
            {
                truncatedDist[i] = dist[i] / maxHeight;
                //(Changed by Kevin)
                ///truncatedDist[i] = dist[i] / dist[0];
            }

            return new IsotopomerEnvelope(truncatedDist, mostIntenseIsotopomerIndex);
        }

        /// <summary>
        /// Default IsoProfilePredictor
        /// </summary>
        public static IsoProfilePredictor Predictor;

        static IsoProfilePredictor()
        {
            Predictor = new IsoProfilePredictor();
        }

        /// <summary>
        /// Constructor, uses default isotope probabilities
        /// </summary>
        /// <param name="relativeIntensityThreshold"></param>
        /// <param name="maxNumIsotopes"></param>
        public IsoProfilePredictor(double relativeIntensityThreshold = 0.1, int maxNumIsotopes = 100)
        {
            ProbC = DefaultProbC;
            ProbH = DefaultProbH;
            ProbN = DefaultProbN;
            ProbO = DefaultProbO;
            ProbS = DefaultProbS;

            this.IsotopeRelativeIntensityThreshold = relativeIntensityThreshold;
            this.MaxNumIsotopes = maxNumIsotopes;

            if (_possibleIsotopeCombinations == null)
            {
                ComputePossibleIsotopeCombinations(MaxNumIsotopes, ProbC.Length - 1);
            }
        }

        /// <summary>
        /// Constructor, uses provided isotope probabilities
        /// </summary>
        /// <param name="probC"></param>
        /// <param name="probH"></param>
        /// <param name="probN"></param>
        /// <param name="probO"></param>
        /// <param name="probS"></param>
        /// <param name="relativeIntensityThreshold"></param>
        /// <param name="maxNumIsotopes"></param>
        public IsoProfilePredictor(
                                   double[] probC,
                                   double[] probH,
                                   double[] probN,
                                   double[] probO,
                                   double[] probS,
                                   double relativeIntensityThreshold = 0.1,
                                   int maxNumIsotopes = 100)
        {
            ProbC = probC;
            ProbH = probH;
            ProbN = probN;
            ProbO = probO;
            ProbS = probS;

            this.IsotopeRelativeIntensityThreshold = relativeIntensityThreshold;
            this.MaxNumIsotopes = maxNumIsotopes;

            if (_possibleIsotopeCombinations == null)
            {
                ComputePossibleIsotopeCombinations(MaxNumIsotopes, ProbC.Length - 1);
            }
        }

        /// <summary>
        /// Default isotope probabilities for Carbon
        /// </summary>
        public static readonly double[] DefaultProbC = { .9893, 0.0107, 0, 0 };

        /// <summary>
        /// Default isotope probabilities for Hydrogen
        /// </summary>
        public static readonly double[] DefaultProbH = { .999885, .000115, 0, 0 };

        /// <summary>
        /// Default isotope probabilities for Nitrogen
        /// </summary>
        public static readonly double[] DefaultProbN = { 0.99632, 0.00368, 0, 0 };

        /// <summary>
        /// Default isotope probabilities for Oxygen
        /// </summary>
        public static readonly double[] DefaultProbO = { 0.99757, 0.00038, 0.00205, 0 };

        /// <summary>
        /// Default isotope probabilities for Sulfur
        /// </summary>
        public static readonly double[] DefaultProbS = { 0.9493, 0.0076, 0.0429, 0.0002 };

        /// <summary>
        /// Isotope probabilities for Carbon used by this instance
        /// </summary>
        public double[] ProbC { get; }

        /// <summary>
        /// Isotope probabilities for Hydrogen used by this instance
        /// </summary>
        public double[] ProbH { get; }

        /// <summary>
        /// Isotope probabilities for Nitrogen used by this instance
        /// </summary>
        public double[] ProbN { get; }

        /// <summary>
        /// Isotope probabilities for Oxygen used by this instance
        /// </summary>
        public double[] ProbO { get; }

        /// <summary>
        /// Isotope probabilities for Sulfur used by this instance
        /// </summary>
        public double[] ProbS { get; }

        private static int[][][] _possibleIsotopeCombinations;

        private static void ComputePossibleIsotopeCombinations(int max, int maxIsotopeNumberInElement = 3) // called just once.
        {
            var comb = new List<int[]>[max + 1];
            comb[0] = new List<int[]> { (new int[maxIsotopeNumberInElement]) };

            for (var n = 1; n <= max; n++)
            {
                comb[n] = new List<int[]>();
                for (var j = 1; j <= maxIsotopeNumberInElement; j++)
                {
                    var index = n - j;
                    if (index < 0) continue;
                    foreach (var t in comb[index])
                    {
                        var add = new int[maxIsotopeNumberInElement];
                        add[j - 1]++;
                        for (var k = 0; k < t.Length; k++)
                            add[k] += t[k];
                        var toAdd = comb[n].Select(v => !v.Where((t1, p) => t1 != add[p]).Any()).All(equal => !equal);
                        if (toAdd) comb[n].Add(add);
                    }
                }
            }
            var possibleIsotopeCombinations = new int[max][][];
            for (var i = 0; i < possibleIsotopeCombinations.Length; i++)
            {
                possibleIsotopeCombinations[i] = new int[comb[i].Count][];
                var j = 0;
                foreach (var t in comb[i])
                {
                    possibleIsotopeCombinations[i][j++] = t;
                }
            }
            _possibleIsotopeCombinations = possibleIsotopeCombinations;
        }

        private double GetIsotopeProbability(int[] number, double[] means, double[] exps)
        {
            var prob = 1.0;
            for (var i = 1; i <= Math.Min(ProbC.Length - 1, number.Length); i++)
            {
                var mean = means[i];
                var exp = exps[i];
                if (number[i - 1] == 0) prob *= exp;
                else
                    prob *=
                        (Math.Pow(mean, number[i - 1]) * exp / AdditionalMath.Factorial(number[i - 1]));
            }
            return prob;
        }
    }
}
