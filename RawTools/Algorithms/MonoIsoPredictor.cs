using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.Utilities.MathStats;
using RawTools.Constants;
using RawTools.Algorithms;

namespace RawTools.Algorithms
{
    public static class MonoIsoPredictor
    {
        /// <summary>
        /// Predict precursor monoisotopic mass
        /// </summary>
        /// <param name="centroidData">MS1 centroid data</param>
        /// <param name="parentMZ">Parent ion m/z</param>
        /// <param name="assignedCharge">Charge state assigned in the raw file</param>
        /// <returns></returns>
        public static (int charge, double mass) GetMonoIsotopicMassCharge(CentroidStreamData centroidData, double parentMZ, int assignedCharge)
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
        public static (int charge, double mass) GetMonoIsotopicMassCharge(CentroidStreamData centroidData, double parentMZ, List<int> possibleChargeStates, int assignedChargeState)
        {
            Dictionary<(int charge, double monoIsoMass), (double distance, double correlation)> scores =
                new Dictionary<(int charge, double monoIsoMass), (double distance, double correlation)>(); ;
            List<double> possibleMonoIsoMasses;
            Averagine averagine = new Averagine();
            double[] masses, intensities;
            (masses, intensities) = AdditionalMath.SubsetMsData(centroidData.Masses, centroidData.Intensities, parentMZ - 2.2, parentMZ + 5);

            if (masses.Length == 0)
            {
                Console.WriteLine("BLAMO!");
                return (assignedChargeState, parentMZ);
            }

            foreach (int charge in possibleChargeStates)
            {
                // populate a list of possible monoisotopic masses
                possibleMonoIsoMasses = new List<double>() { parentMZ };
                for (int i = 1; i < 3; i++)
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
                    double monoIsotopeMass = Ion.GetMonoIsotopicMassFromMZ(monoisoMZ, charge);
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
            int chargeOut = -1;

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
