using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawTools.Utilities.MathStats
{
    public static class ToleranceCalculator
    {
        public static bool withinTolerance(this IReadOnlyList<double> isotopes, double ion, double ppmTolerance)
        {
            double[] ppm = new double[isotopes.Count()];

            for (int i = 0; i < ppm.Length; i++)
            {
                ppm[i] = Math.Abs(ion - isotopes[i]) / isotopes[i] * 1e6;
            }

            return ppm.Min() < ppmTolerance;
        }

        public static int indexOfClosest(this IList<double> values, double target)
        {
            int index = 0;
            double diff = 1e6;
            for (int i = 0; i < values.Count(); i++)
            {
                if (Math.Abs(target - values[i]) < diff)
                {
                    diff = Math.Abs(target - values[i]);
                    index = i;
                }
            }
            return index;
        }
    }
}
