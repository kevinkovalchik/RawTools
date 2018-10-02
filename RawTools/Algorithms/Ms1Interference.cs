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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using System.Collections;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools.Data.Extraction;
using RawTools.Utilities;
using RawTools.Algorithms;

namespace RawTools.Algorithms
{
    static class Ms1Interference
    {
        static (double[], double[]) SubsetCentroidScan(this CentroidStreamData Ms1Scan, double parentMass, double isoWindow)
        {
            List<double> masses = new List<double>();
            List<double> intensities = new List<double>();

            double lower = parentMass - 0.5 * isoWindow;
            double upper = parentMass + 0.5 * isoWindow;

            for (int i = 0; i < Ms1Scan.Masses.Length; i++)
            {
                if (Ms1Scan.Masses[i] > lower & Ms1Scan.Masses[i] < upper)
                {
                    masses.Add(Ms1Scan.Masses[i]);
                    intensities.Add(Ms1Scan.Intensities[i]);
                }
            }

            return (masses.ToArray(), intensities.ToArray());
        }

        static bool withinTolerance(this List<double> isotopes, double ion, double ppmTolerance)
        {
            double[] ppm = new double[isotopes.Count()];

            for (int i = 0; i < ppm.Length; i++)
            {
                ppm[i] = Math.Abs(ion - isotopes[i]) / isotopes[i] * 1e6;
            }

            return ppm.Min() < ppmTolerance;
        }

        public static double CalculateForOneScan(CentroidStreamData Ms1Scan, double monoIsoMass, double isoWindow, int charge, double ppm = 4)
        {
            double[] masses;
            double[] intensities;
            double[] interferences;
            double currentIsotope;
            List<double> isotopes = new List<double>();

            // subset the ms1 scan to be the isolation window
            (masses, intensities) = SubsetCentroidScan(Ms1Scan, monoIsoMass, isoWindow);

            // make a copy of the intensities array to represent the interference intensities
            interferences = (double[])intensities.Clone();

            // make a list of the possible isotopes
            currentIsotope = monoIsoMass;
            while (currentIsotope < monoIsoMass + 0.5 * isoWindow)
            {
                isotopes.Add(currentIsotope);
                currentIsotope += 1.003356 / charge;
            }

            for (int i = 0; i < interferences.Length; i++)
            {
                double ion = masses[i];

                if (isotopes.withinTolerance(ion, 10))
                {
                    interferences[i] = 0;
                }
            }

            return interferences.Sum() / intensities.Sum();
        }
    }
}
