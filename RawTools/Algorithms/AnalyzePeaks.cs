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
    static class AnalyzePeaks
    {
        private static PrecursorPeakData OnePeak(CentroidStreamCollection centroids, RetentionTimeCollection retentionTimes,double monoIsoMass, int parentScan, int ddScan, ScanIndex index)
        {
            PrecursorPeakData peak = new PrecursorPeakData();

            int firstScan = parentScan,
                lastScan = parentScan,
                maxScan = parentScan,
                currentScan = parentScan,
                previousMS1scan, nextMS1scan;

            bool containsFirstMS1Scan = false,
                containsLastMS1Scan = false;

            int[] MS1Scans = index.ScanEnumerators[MSOrderType.Ms];

            double minMassDiff, maxIntensity, parentIntensity;

            List<int> scans = new List<int>();
            List<double> profileTimes = new List<double>();
            List<double> profileIntensities = new List<double>();

            double[] masses, intensities, massDiff;

            Dictionary<int, double> indexedIntensities = new Dictionary<int, double>();

            // first take care of the parent scan data. In QE data sometimes the parent mass is missing from the parent spectrum, so we need to deal with that.

            masses = centroids[currentScan].Masses;//.Where(i => (i > parentMass - 1 & i < parentMass + 1)).ToArray();
            //masses = (from mass in rawData.centroidStreams[currentScan].Masses where mass > parentMass - 1 & mass < parentMass + 1 select mass).ToArray();
            //masses = masses.Where(i => (i > parentMass - 1 & i < parentMass + 1)).ToArray();

            if (masses.Length == 0)
            {
                peak.PeakFound = false;
                return peak;
            }

            massDiff = new double[masses.Length];

            for (int i = 0; i < masses.Length; i++)
            {
                massDiff[i] = Math.Abs(masses[i] - monoIsoMass);
            }

            minMassDiff = massDiff.Min();

            if (minMassDiff / monoIsoMass * 1e6 < 4)
            {
                peak.PeakFound = true;
            }
            else
            {
                peak.PeakFound = false;
                return peak;
            }

            int scanIndex = Array.IndexOf(MS1Scans, parentScan);

            // now find the first ms1 scan of the peak, just follow the mass (within tolerance) accross scans until it goes to baseline
            while (true)
            {
                currentScan = MS1Scans[scanIndex];
                masses = centroids[currentScan].Masses;
                intensities = centroids[currentScan].Intensities;

                massDiff = new double[masses.Length];

                for (int i = 0; i < masses.Length; i++)
                {
                    massDiff[i] = Math.Abs(masses[i] - monoIsoMass);
                }

                minMassDiff = massDiff.Min();

                if (minMassDiff / monoIsoMass * 1e6 < 4)
                {
                    scans.Add(currentScan);
                    scanIndex -= 1;
                    indexedIntensities.Add(currentScan, intensities[Array.IndexOf(massDiff, minMassDiff)]);
                    if (scanIndex < 0)
                    {
                        previousMS1scan = currentScan;
                        break;
                    }
                }
                else
                {
                    if (scanIndex == 0)
                    {
                        previousMS1scan = currentScan;
                    }
                    else
                    {
                        previousMS1scan = MS1Scans[scanIndex - 1];
                    }
                    break;
                }
            }

            // now find the last ms1 scan of the peak
            scanIndex = Array.IndexOf(MS1Scans, parentScan) + 1; // reset the ms1 scan indexer, add 1 so we don't replicate the parent scan

            while (true)
            {
                // Check to make sure the ms1 scan isn't the last one....
                if (scanIndex >= MS1Scans.Length)
                {
                    nextMS1scan = currentScan;
                    break;
                }

                currentScan = MS1Scans[scanIndex];
                masses = centroids[currentScan].Masses;
                intensities = centroids[currentScan].Intensities;

                massDiff = new double[masses.Length];

                for (int i = 0; i < masses.Length; i++)
                {
                    massDiff[i] = Math.Abs(masses[i] - monoIsoMass);
                }

                minMassDiff = massDiff.Min();

                if (minMassDiff / monoIsoMass * 1e6 < 4)
                {
                    scans.Add(currentScan);
                    scanIndex += 1;
                    indexedIntensities.Add(currentScan, intensities[Array.IndexOf(massDiff, minMassDiff)]);
                    if (scanIndex >= MS1Scans.Length)
                    {
                        nextMS1scan = currentScan;
                        break;
                    }
                }
                else
                {
                    if (scanIndex == MS1Scans.Length - 1)
                    {
                        nextMS1scan = currentScan;
                    }
                    else
                    {
                        nextMS1scan = MS1Scans[scanIndex + 1];
                    }
                    break;
                }
            }
            // We need to add an index and intensity for the scans before and after the peak. Otherwise fitting and other calculations later will be a huge pain.
            // We make note of the peaks which contain the first or last MS1 scans. This edge cases will probably need special treatment.

            if (previousMS1scan != scans.Min())
            {
                scans.Add(previousMS1scan);
                indexedIntensities.Add(previousMS1scan, 0);
            }
            else
            {
                containsFirstMS1Scan = true;
            }
            if (nextMS1scan != scans.Max())
            {
                scans.Add(nextMS1scan);
                indexedIntensities.Add(nextMS1scan, 0);
            }
            else
            {
                containsLastMS1Scan = true;
            }

            scans.Sort();
            firstScan = scans.First();
            lastScan = scans.Last();


            // add the retention times and intensities

            foreach (int scan in scans)
            {
                profileTimes.Add(retentionTimes[scan]);
                profileIntensities.Add(indexedIntensities[scan]);
            }

            maxIntensity = profileIntensities.Max();
            parentIntensity = indexedIntensities[parentScan];

            maxScan = scans[profileIntensities.IndexOf(maxIntensity)];

            peak.FirstScan = firstScan;
            peak.LastScan = lastScan;
            peak.MaxScan = maxScan;
            //peak.PreviousScan = previousMS1scan;
            //peak.NextScan = nextMS1scan;
            peak.ParentScan = parentScan;
            peak.NScans = scans.Count();
            peak.Scans = scans.ToArray();
            peak.ContainsFirstMS1Scan = containsFirstMS1Scan;
            peak.ContainsLastMS1Scan = containsLastMS1Scan;

            peak.ParentIntensity = parentIntensity;
            peak.MaximumIntensity = maxIntensity;

            peak.MaximumRetTime = retentionTimes[maxScan];
            peak.ParentRetTime = retentionTimes[parentScan];

            peak.BaselineWidth = profileTimes.Last() - profileTimes.First();

            peak.Intensities = profileIntensities.ToArray();
            peak.RetTimes = profileTimes.ToArray();

            return peak;
        }

        public static PrecursorPeakCollection AnalyzeAllPeaks(CentroidStreamCollection centroids, RetentionTimeCollection retentionTimes,
            PrecursorMassCollection precursorMasses, PrecursorScanCollection precursorScans, ScanIndex index)
        {
            PrecursorPeakCollection peaks = new PrecursorPeakCollection();
            PrecursorPeakData peak = new PrecursorPeakData();
            DistributionMultiple allPeaksAsymmetry = new DistributionMultiple();
            DistributionMultiple allPeaksWidths = new DistributionMultiple();

            int[] scans = index.ScanEnumerators[MSOrderType.Ms2];

            ProgressIndicator P = new ProgressIndicator(total: scans.Length, message: "Analyzing precursor peaks");

            foreach (int scan in scans)
            {
                peak = OnePeak(centroids, retentionTimes, precursorMasses[scan].MonoisotopicMZ, precursorScans[scan].MasterScan, ddScan: scan, index: index);

                if (peak.NScans < 5 | peak.PeakFound == false |
                    peak.ContainsFirstMS1Scan | peak.ContainsLastMS1Scan)
                {
                    peak.PeakShape = null;
                }
                else
                {
                    peak.PeakShape = GetPeakShape(peak);
                    allPeaksAsymmetry.Add(peak.PeakShape.Asymmetry);
                    allPeaksWidths.Add(peak.PeakShape.Width);
                }
                
                peak.Area = CalculatePeakArea(peak);

                peaks.Add(scan, peak);

                P.Update();
            }
            P.Done();

            if (allPeaksWidths.P50.Count() == 0)
            {
                peaks.PeakShapeMedians = new Data.Containers.PeakShape(width: new Width(), asymmetry: new Asymmetry(), peakMax: 0);
            }
            else
            {
                peaks.PeakShapeMedians = new Data.Containers.PeakShape(width: allPeaksWidths.GetMedians(), asymmetry: allPeaksAsymmetry.GetMedians(), peakMax: 0);
            }

            return peaks;
        }

        private static PrecursorPeakCollection CalcPeakRetTimesAndInts(CentroidStreamCollection centroids, IRawDataPlus rawFile, RetentionTimeCollection retentionTimes,
            PrecursorMassCollection precursorMasses, PrecursorScanCollection precursorScans, ScanIndex index)
        {
            int[] scans = index.ScanEnumerators[MSOrderType.Ms2];

            PrecursorPeakCollection peaks = new PrecursorPeakCollection();

            ProgressIndicator P = new ProgressIndicator(total: scans.Length, message: "Analyzing precursor peaks");

            foreach (int scan in scans)
            {
                peaks.Add(scan, OnePeak(centroids, retentionTimes, precursorMasses[scan].MonoisotopicMZ, precursorScans[scan].MasterScan, ddScan: scan, index: index));
                P.Update();
            }
            P.Done();

            return peaks;
        }

        private static double InterpolateLinear((double x, double y) point0, (double x, double y) point1, double y)
        {
            return point0.x + (y - point0.y) * (point1.x - point0.x) / (point1.y - point0.y);
        }

        private static Data.Containers.PeakShape GetPeakShape(PrecursorPeakData precursorPeak)
        {
            double[] Intensities, RetTimes;
            int maxIndex, currentIndex;
            Dictionary<int, double> a, b;
            Asymmetry asymmetry = new Asymmetry();
            Width width = new Width();
            a = new Dictionary<int, double>();
            b = new Dictionary<int, double>();

            Intensities = precursorPeak.Intensities;
            RetTimes = precursorPeak.RetTimes;
            maxIndex = Array.FindIndex(Intensities, x => { return x == Intensities.Max(); });


            foreach (int percent in new int[4] { 10, 25, 50, 75 })
            {
                currentIndex = maxIndex;
                // first we find the asymmetery parameter from the left side of the peak
                while (true)
                {
                    if (Intensities[currentIndex] < Intensities[maxIndex] * percent / 100)
                    {
                        a.Add(percent, RetTimes[maxIndex] - InterpolateLinear(point0: (RetTimes[currentIndex], Intensities[currentIndex]),
                            point1: (RetTimes[currentIndex + 1], Intensities[currentIndex + 1]), y: Intensities[maxIndex] * percent / 100));
                        break;
                    }
                    else
                    {
                        currentIndex -= 1;

                        /*
                         * Now the peak MUST contain intensities of 0 at the edges, so this case shouldn't come up anymore
                        if (currentIndex == -1)
                        {                            
                            double newTime = rawData.retentionTimes[rawData.peakData[scan].PreviousScan];
                            a.Add(percent, RetTimes[maxIndex] - InterpolateLinear(point0: (newTime, 0),
                                point1: (RetTimes.First(), Intensities.First()), y: Intensities[maxIndex] * percent / 100));
                            break;
                        }
                        */
                    }
                }

                currentIndex = maxIndex;
                // and now the right side of the peak
                while (true)
                {
                    if (Intensities[currentIndex] < Intensities[maxIndex] * percent / 100)
                    {
                        b.Add(percent, InterpolateLinear(point0: (RetTimes[currentIndex - 1], Intensities[currentIndex - 1]),
                            point1: (RetTimes[currentIndex], Intensities[currentIndex]), y: Intensities[maxIndex] * percent / 100) - RetTimes[maxIndex]);
                        break;
                    }
                    else
                    {
                        currentIndex += 1;
                        /*
                         * Now the peak MUST contain intensities of 0 at the edges, so this case shouldn't come up anymore
                        if (currentIndex == RetTimes.Count())
                        {
                            double newTime = rawData.retentionTimes[rawData.peakData[scan].NextScan];
                            b.Add(percent, InterpolateLinear(point0: (RetTimes.Last(), Intensities.Last()), point1: (newTime, 0),
                                y: Intensities[maxIndex] * percent / 100) - RetTimes[maxIndex]);
                            break;
                        }
                        */
                    }
                }
            }
            asymmetry.P10 = b[10] / a[10];
            asymmetry.P25 = b[25] / a[25];
            asymmetry.P50 = b[50] / a[50];
            asymmetry.P75 = b[75] / a[75];
            width.P10 = a[10] + b[10];
            width.P25 = a[25] + b[25];
            width.P50 = a[50] + b[50];
            width.P75 = a[75] + b[75];

            Data.Containers.PeakShape peak = new Data.Containers.PeakShape(asymmetry: asymmetry, width: width, peakMax: RetTimes[maxIndex]);

            return peak;
        }
        
        private static double CalculatePeakArea(PrecursorPeakData peak)
        {
            double area = 0;

            for (int i = 1; i < peak.NScans; i++)
            {
                area += (peak.Intensities[i - 1] + peak.Intensities[i]) * (peak.RetTimes[i] - peak.RetTimes[i - 1]) / 2;
            }

            return area;
        }
    }
}
