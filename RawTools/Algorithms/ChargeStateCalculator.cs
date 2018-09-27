using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawTools.Data.Containers;
using RawTools.Utilities;

namespace RawTools.Algorithms
{
    static class ChargeStateCalculator
    {
        public static List<int> GetChargeState(CentroidStreamData centroidData, double parentMass, int assignedCharge)
        {
            double[] masses;
            double[] intensities;
            int[] allowedCharges = new int[] { 2, 3, 4 };
            List<int> possibleChargeStates = new List<int>();
            int parentIndex;
            double massDiff = 1.003356;

            (masses, intensities) = AdditionalMath.SubsetMsData(centroidData.Masses, centroidData.Intensities, parentMass - 0.1, parentMass + 1.2);

            parentIndex = masses.ToList().IndexOf(parentMass);
            int currentIndex;

            // needs more work. can try matching multiple isotopes, then if charge state is ambiguous take the one with more matches
            foreach (int charge in allowedCharges)
            {
                currentIndex = parentIndex + 1;
                double isotopeMass = parentMass + massDiff / charge;

                for (int i = currentIndex; i < masses.Length; i++)
                {
                    if (Math.Abs(masses[i] - isotopeMass) / isotopeMass * 1e6 < 6)
                    {
                        double ratio = intensities[i] / intensities[parentIndex];
                        if (ratio > 10 | ratio < 0.1)
                        {
                            // the peak is really big or small compared to the parent peak. it probably is not actually an isotope of it
                            continue;
                        }
                        possibleChargeStates.Add(charge);
                        break;
                    }
                }
            }

            if (possibleChargeStates.Count() == 0)
            {
                possibleChargeStates.Add(assignedCharge);
            }

            return possibleChargeStates;
        }
    }
}
