// This code is adapted from InformedProteomics from the compomics group. Need to add some license info etc here

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
        /// Get the theoretical Isotope profile for <paramref name="monoIsotopeMass"/> at charge <paramref name="charge"/>
        /// </summary>
        /// <param name="monoIsotopeMass"></param>
        /// <param name="charge"></param>
        /// <param name="relativeIntensityThreshold"></param>
        /// <returns></returns>
        public static SimpleCentroid GetTheoreticalIsotopeProfile(double monoIsotopeMass, int charge, double relativeIntensityThreshold = 0.1)
        {
            return DefaultAveragine.GetTheoreticalIsotopeProfileInst(monoIsotopeMass, charge, relativeIntensityThreshold);
        }

        /// <summary>
        /// Get the theoretical Isotope profile for <paramref name="monoIsotopeMass"/> at charge <paramref name="charge"/> using <paramref name="isoProfilePredictor"/>
        /// </summary>
        /// <param name="monoIsotopeMass"></param>
        /// <param name="charge"></param>
        /// <param name="relativeIntensityThreshold"></param>
        /// <param name="isoProfilePredictor"></param>
        /// <returns></returns>
        public SimpleCentroid GetTheoreticalIsotopeProfileInst(double monoIsotopeMass, int charge, double relativeIntensityThreshold = 0.1, IsoProfilePredictor isoProfilePredictor = null)
        {
            var peakList = new SimpleCentroid();
            var envelope = GetIsotopomerEnvelopeInst(monoIsotopeMass, isoProfilePredictor);
            for (var isotopeIndex = 0; isotopeIndex < envelope.Envelope.Length; isotopeIndex++)
            {
                var intensity = envelope.Envelope[isotopeIndex];
                if (intensity < relativeIntensityThreshold) continue;
                var mz = Ion.GetIsotopeMz(monoIsotopeMass, charge, isotopeIndex);
                peakList.Masses.Add(mz);
                peakList.Intensities.Add(intensity);
            }
            return peakList;
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

    public class Ion
    { 
        /// <summary>
        /// Get the m/z of the specified isotope
        /// </summary>
        /// <param name="monoIsotopicMass"></param>
        /// <param name="charge"></param>
        /// <param name="isotopeIndex"></param>
        /// <returns></returns>
        public static double GetIsotopeMz(double monoIsotopicMass, int charge, int isotopeIndex)
        {
            var isotopeMass = monoIsotopicMass + isotopeIndex * Masses.C13MinusC12;
            return isotopeMass / charge + Masses.Proton;
        }

        /// <summary>
        /// Get the monoisotopic mass of the specified isotope
        /// </summary>
        /// <param name="isotopeMz"></param>
        /// <param name="charge"></param>
        /// <param name="isotopeIndex"></param>
        /// <returns></returns>
        public static double GetMonoIsotopicMass(double isotopeMz, int charge, int isotopeIndex)
        {
            var isotopeMass = (isotopeMz - Masses.Proton) * charge;
            var monoIsotopeMass = isotopeMass - isotopeIndex * Masses.C13MinusC12;
            return monoIsotopeMass;
        }

        /// <summary>
        /// Get the monoisotopic mass of a given m/z
        /// </summary>
        /// <param name="isotopeMz"></param>
        /// <param name="charge"></param>
        /// <param name="isotopeIndex"></param>
        /// <returns></returns>
        public static double GetMonoIsotopicMassFromMZ(double isotopeMz, int charge)
        {
            return (isotopeMz - Masses.Proton) * charge;
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
