using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawTools.Algorithms
{
    class Ms2NoiseModel
    {
        public double CutOff(double[] intensities)
        {
            double sum = intensities.Sum();
            double len = intensities.Length;

            return Math.Sqrt((sum * sum) / (len * len));
        }
    }
}
