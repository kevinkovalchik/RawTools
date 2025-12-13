// This code is adapted from InformedProteomics from the compomics group. https://github.com/PNNL-Comp-Mass-Spec/Informed-Proteomics
// Informed Proteomics is licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawTools.Algorithms
{
    public class IsotopomerEnvelope
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="mostAbundantIsotopeIndex"></param>
        public IsotopomerEnvelope(double[] envelope, int mostAbundantIsotopeIndex)
        {
            Envelope = envelope;
            MostAbundantIsotopeIndex = mostAbundantIsotopeIndex;
        }

        /// <summary>
        /// The Isotopomer envelope
        /// </summary>
        public double[] Envelope { get; }

        /// <summary>
        /// Index in <see cref="Envelope"/> of the most abundant isotope
        /// </summary>
        public int MostAbundantIsotopeIndex { get; }
    }
}
