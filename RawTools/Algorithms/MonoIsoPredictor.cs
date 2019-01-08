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
using RawTools.Data.Collections;
using RawTools.Utilities;
using RawTools.Utilities.MathStats;
using RawTools.Constants;
using RawTools.Algorithms;

namespace RawTools.Algorithms
{
    static class MonoIsoPredictor
    {
        public static void RefineMonoIsoMassChargeValues(CentroidStreamCollection centroids, PrecursorMassCollection precursorMasses, TrailerExtraCollection trailerExtras, PrecursorPeakCollection precursorPeaks, PrecursorScanCollection precursorScans)
        {
            int ms2Scan, ms1Scan, refinedCharge;
            double refinedMass;

            ProgressIndicator P = new ProgressIndicator(precursorPeaks.Count(), "Refining precursor charge and monoisotopic mass");
            P.Start();

            foreach (var peak in precursorPeaks)
            {
                ms2Scan = peak.Value.Ms2Scan;

                if (peak.Value.PeakFound)
                {
                    ms1Scan = peak.Value.MaxScan;
                }
                else
                {
                    ms1Scan = precursorScans[ms2Scan].MasterScan;
                }

                (refinedCharge, refinedMass) = GetMonoIsotopicMassCharge(centroids[ms1Scan], precursorMasses[ms2Scan].ParentMZ, trailerExtras[ms2Scan].ChargeState);

                precursorMasses[ms2Scan].MonoisotopicMZ = refinedMass;
                trailerExtras[ms2Scan].MonoisotopicMZ = refinedMass;
                trailerExtras[ms2Scan].ChargeState = refinedCharge;

                P.Update();
            }
            P.Done();
        }

        /// <summary>
        /// Predict precursor monoisotopic mass
        /// </summary>
        /// <param name="centroidData">MS1 centroid data</param>
        /// <param name="parentMZ">Parent ion m/z</param>
        /// <param name="assignedCharge">Charge state assigned in the raw file</param>
        /// <returns></returns>
        static (int charge, double mass) GetMonoIsotopicMassCharge(CentroidStreamData centroidData, double parentMZ, int assignedCharge)
        {
            List<int> possibleChargeStates = ChargeStateCalculator.GetChargeState(centroidData, parentMZ, assignedCharge);
            return GetMonoIsotopicMassCharge(centroidData, parentMZ, possibleChargeStates, assignedCharge);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centroidData"></param>
        /// <param name="parentMZ"></param>
        /// <param name="possibleChargeStates"></param>
        /// <param name="assignedChargeState"></param>
        /// <returns></returns>
        static (int charge, double mass) GetMonoIsotopicMassCharge(CentroidStreamData centroidData, double parentMZ, List<int> possibleChargeStates, int assignedChargeState)
        {
            Dictionary<(int charge, double monoIsoMass), (double distance, double correlation)> scores =
                new Dictionary<(int charge, double monoIsoMass), (double distance, double correlation)>(); ;
            List<double> possibleMonoIsoMasses;
            Averagine averagine = new Averagine();
            double[] masses, intensities;
            (masses, intensities) = AdditionalMath.SubsetMsData(centroidData.Masses, centroidData.Intensities, parentMZ - 2.2, parentMZ + 5);

            if (masses.Length == 0)
            {
                return (assignedChargeState, parentMZ);
            }

            foreach (int charge in possibleChargeStates)
            {
                // populate a list of possible monoisotopic masses
                possibleMonoIsoMasses = new List<double>() { parentMZ };
                for (int i = 1; i < 4; i++)
                {
                    double isotope = parentMZ - i * Masses.C13MinusC12 / charge;
                    if (masses.withinTolerance(isotope, 10))
                    {
                        possibleMonoIsoMasses.Add(isotope);
                    }
                    else
                    {
                        break;
                    }
                }

                if (possibleChargeStates.Count() == 1 & possibleMonoIsoMasses.Count() == 1)
                {
                    // only one possible monoisotopic mass was found, so just return that
                    return (possibleChargeStates.First(), parentMZ);
                }

                // now score 
                foreach (double monoisoMZ in possibleMonoIsoMasses)
                {
                    // calculate the actual mass and intensity envelope of the potential monoisotopicMZ
                    double monoIsotopeMass = (monoisoMZ - Masses.Proton) * charge;
                    double[] isotopomerEnvelope = Averagine.GetIsotopomerEnvelope(monoIsotopeMass).Envelope;

                    // get out the observed intensities corresponding to the isotope envelope
                    List<double> observedIntensities = new List<double>();
                    int currentIndex = masses.ToList().IndexOf(monoisoMZ);

                    for (int i = 0; i < isotopomerEnvelope.Count(); i++)
                    {
                        if ((masses.withinTolerance(monoisoMZ + i * Masses.C13MinusC12/charge, 10)))
                        {
                            if (currentIndex < intensities.Count())
                            {
                                int index = masses.indexOfClosest(monoisoMZ + i * Masses.C13MinusC12 / charge);
                                observedIntensities.Add(intensities[index]);
                            }
                            else
                            {
                                // we've run out of ions
                                double[] temp = new double[observedIntensities.Count()];
                                for (int j = 0; j < temp.Length; j++)
                                {
                                    temp[j] = isotopomerEnvelope[j];
                                }
                                isotopomerEnvelope = temp;
                            }
                        }
                        else
                        {
                            observedIntensities.Add(0);
                        }
                        currentIndex++;
                    }

                    // normalize the observed intensities
                    double maxInt = observedIntensities.Max();
                    for (int i = 0; i < observedIntensities.Count(); i++)
                    {
                        observedIntensities[i] = observedIntensities[i] / maxInt;
                    }

                    // calculate distance and correlation
                    double distance, correlation;
                    (distance, correlation) = FitScores.GetDistanceAndCorrelation(observedIntensities.ToArray(), isotopomerEnvelope);
                    if (distance < 0.3)
                    {
                        scores.Add((charge, monoisoMZ), (distance, correlation));
                    }
                }
            }

            double smallestDistance = 1;
            double massOut = 0;
            int chargeOut = assignedChargeState;

            if (scores.Keys.Count == 0)
            {
                return (chargeOut, parentMZ);
            }

            foreach (var key in scores?.Keys)
            {
                if (scores[key].distance < smallestDistance)
                {
                    smallestDistance = scores[key].distance;
                    massOut = key.monoIsoMass;
                    chargeOut = key.charge;
                }
            }

            return (chargeOut, massOut);
        }
    }
}
