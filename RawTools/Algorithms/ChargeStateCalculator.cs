// Copyright 2018 Kevin Kovalchik & Christopher Hughes
// 
// Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// Kevin Kovalchik and Christopher Hughes do not claim copyright of
// any third-party libraries ditributed with RawTools. All third party
// licenses are provided in accompanying files as outline in the NOTICE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.Utilities.MathStats;
using RawTools.Algorithms;

namespace RawTools.Algorithms
{
    static class ChargeStateCalculator
    {
        public static List<int> GetChargeState(CentroidStreamData centroidData, double parentMass, int assignedCharge)
        {
            double[] masses;
            double[] intensities;
            int[] allowedCharges = new int[] {  2, 3, 4, 5 };
            List<int> possibleChargeStates = new List<int>();
            int parentIndex;
            double massDiff = 1.003356;

            (masses, intensities) = AdditionalMath.SubsetMsData(centroidData.Masses, centroidData.Intensities, parentMass - 0.1, parentMass + 1.2);

            if (masses.Length == 0)
            {
                possibleChargeStates.Add(assignedCharge);
                return possibleChargeStates;
            }

            int currentIndex;

            if (masses.withinTolerance(parentMass, 10))
            {
                parentIndex = masses.indexOfClosest(parentMass);
            }
            else
            {
                possibleChargeStates.Add(assignedCharge);
                return possibleChargeStates;
            }


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
