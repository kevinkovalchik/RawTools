// This code is adapted from InformedProteomics from the compomics group. https://github.com/PNNL-Comp-Mass-Spec/Informed-Proteomics
// Informed Proteomics is licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using RawTools.Constants;
using RawTools.Utilities;
using RawTools.Data.Containers;

namespace RawTools.Algorithms
{
    /// <summary>
    /// Averagine algorithm - creates isotopic envelopes based on an averaged elemental composition
    /// </summary>
    public class Averagine
    {
        // NominalMass -> Isotope Envelop (Changed to ConcurrentDictionary by Chris)
        private readonly ConcurrentDictionary<int, IsotopomerEnvelope> IsotopeEnvelopMap;

        /// <summary>
        /// Constructor
        /// </summary>
        public Averagine()
        {
            this.IsotopeEnvelopMap = new ConcurrentDictionary<int, IsotopomerEnvelope>();
        }

        /// <summary>
        /// Get the Isotopomer envelope for <paramref name="monoIsotopeMass"/>
        /// </summary>
        /// <param name="monoIsotopeMass"></param>
        /// <returns></returns>
        public static IsotopomerEnvelope GetIsotopomerEnvelope(double monoIsotopeMass)
        {
            return DefaultAveragine.GetIsotopomerEnvelopeInst(monoIsotopeMass);
        }

        /// <summary>
        /// Get the Isotopomer envelope for <paramref name="monoIsotopeMass"/> using <paramref name="isoProfilePredictor"/>
        /// </summary>
        /// <param name="monoIsotopeMass"></param>
        /// <param name="isoProfilePredictor"></param>
        /// <returns></returns>
        public IsotopomerEnvelope GetIsotopomerEnvelopeInst(double monoIsotopeMass, IsoProfilePredictor isoProfilePredictor = null)
        {
            var nominalMass = (int)Math.Round(monoIsotopeMass * 0.9995);
            return GetIsotopomerEnvelopeFromNominalMassInst(nominalMass, isoProfilePredictor);
        }

        /// <summary>
        /// Get the Isotopomer envelope for the nominal mass <paramref name="nominalMass"/>
        /// </summary>
        /// <param name="nominalMass"></param>
        /// <returns></returns>
        public static IsotopomerEnvelope GetIsotopomerEnvelopeFromNominalMass(int nominalMass)
        {
            return DefaultAveragine.GetIsotopomerEnvelopeFromNominalMassInst(nominalMass);
        }

        /// <summary>
        /// Get the Isotopomer envelope for the nominal mass <paramref name="nominalMass"/> using <paramref name="isoProfilePredictor"/>
        /// </summary>
        /// <param name="nominalMass"></param>
        /// <param name="isoProfilePredictor"></param>
        /// <returns></returns>
        public IsotopomerEnvelope GetIsotopomerEnvelopeFromNominalMassInst(int nominalMass, IsoProfilePredictor isoProfilePredictor = null)
        {
            var nominalMassFound = IsotopeEnvelopMap.TryGetValue(nominalMass, out var envelope);
            if (nominalMassFound) return envelope;

            var mass = nominalMass / 0.9995;
            envelope = ComputeIsotopomerEnvelope(mass, isoProfilePredictor);
            IsotopeEnvelopMap.AddOrUpdate(nominalMass, envelope, (key, value) => value);

            return envelope;
        }

        private const double C = 4.9384;
        private const double H = 7.7583;
        private const double N = 1.3577;
        private const double O = 1.4773;
        private const double S = 0.0417;
        private const double AveragineMass = C * Atom.C + H * Atom.H + N * Atom.N + O * Atom.O + S * Atom.S;

        /// <summary>
        /// Default averagine formula
        /// </summary>
        public static Averagine DefaultAveragine;

        static Averagine()
        {
            DefaultAveragine = new Averagine();
        }

        private IsotopomerEnvelope ComputeIsotopomerEnvelope(double mass, IsoProfilePredictor isoProfilePredictor = null)
        {
            var numAveragines = mass / AveragineMass;
            var numC = (int)Math.Round(C * numAveragines);
            var numH = (int)Math.Round(H * numAveragines);
            var numN = (int)Math.Round(N * numAveragines);
            var numO = (int)Math.Round(O * numAveragines);
            var numS = (int)Math.Round(S * numAveragines);

            if (numH == 0) numH = 1;

            isoProfilePredictor = isoProfilePredictor ?? IsoProfilePredictor.Predictor;
            return isoProfilePredictor.GetIsotopomerEnvelope(numC, numH, numN, numO, numS);
        }
    }

    /// <summary>
    /// Atom class - elements of the periodic table
    /// </summary>
    public class Atom
    {
        public const double C = 12.0;

        /// <summary>
        /// Monoisotopic mass of Hydrogen
        /// </summary>
        public const double H = 1.007825035;

        /// <summary>
        /// Monoisotopic mass of Nitrogen
        /// </summary>
        public const double N = 14.003074;

        /// <summary>
        /// Monoisotopic mass of Oxygen
        /// </summary>
        public const double O = 15.99491463;

        /// <summary>
        /// Monoisotopic mass of Sulfur
        /// </summary>
        public const double S = 31.9720707;
    }
}
