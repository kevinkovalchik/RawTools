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
        public static PrecursorPeakData OnePeak(RawDataCollection rawData, double monoIsoMass, int parentScan, int ddScan)
        {
            PrecursorPeakData peak = new PrecursorPeakData();

            int firstScan = parentScan,
                lastScan = parentScan,
                maxScan = parentScan,
                currentScan = parentScan,
                previousMS1scan, nextMS1scan;

            bool containsFirstMS1Scan = false,
                containsLastMS1Scan = false;

            int[] MS1Scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms];

            double minMassDiff, maxIntensity, parentIntensity;

            List<int> scans = new List<int>();
            List<double> profileTimes = new List<double>();
            List<double> profileIntensities = new List<double>();

            double[] masses, intensities, massDiff;

            Dictionary<int, double> indexedIntensities = new Dictionary<int, double>();

            // first take care of the parent scan data. In QE data sometimes the parent mass is missing from the parent spectrum, so we need to deal with that.

            masses = rawData.centroidStreams[currentScan].Masses;//.Where(i => (i > parentMass - 1 & i < parentMass + 1)).ToArray();
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
                masses = rawData.centroidStreams[currentScan].Masses;
                intensities = rawData.centroidStreams[currentScan].Intensities;

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
                masses = rawData.centroidStreams[currentScan].Masses;
                intensities = rawData.centroidStreams[currentScan].Intensities;

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
                profileTimes.Add(rawData.retentionTimes[scan]);
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

            peak.MaximumRetTime = rawData.retentionTimes[maxScan];
            peak.ParentRetTime = rawData.retentionTimes[parentScan];

            peak.BaselineWidth = profileTimes.Last() - profileTimes.First();

            peak.Intensities = profileIntensities.ToArray();
            peak.RetTimes = profileTimes.ToArray();

            return peak;
        }

        public static void CalcPeakRetTimesAndInts(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            CheckIfDone.Check(rawData, rawFile, new List<Operations> { Operations.ScanIndex, Operations.PrecursorMasses, Operations.PrecursorScans, Operations.Ms1CentroidStreams, Operations.RetentionTimes });

            if (rawData.Performed.Contains(Operations.PeakRetAndInt))
            {
                return;
            }

            int[] scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2];

            PrecursorPeakDataCollection peaks = new PrecursorPeakDataCollection();

            ProgressIndicator P = new ProgressIndicator(total: scans.Length, message: "Analyzing precursor peaks");

            foreach (int scan in scans)
            {
                peaks.Add(scan, OnePeak(rawData: rawData, monoIsoMass: rawData.precursorMasses[scan].MonoisotopicMZ, parentScan: rawData.precursorScans[scan].MasterScan, ddScan: scan));
                P.Update();
            }
            P.Done();

            rawData.peakData = peaks;
            rawData.Performed.Add(Operations.PeakRetAndInt);
            rawData.Performed.RemoveWhere(x => x == Operations.PeakArea);
        }

        public static double InterpolateLinear((double x, double y) point0, (double x, double y) point1, double y)
        {
            return point0.x + (y - point0.y) * (point1.x - point0.x) / (point1.y - point0.y);
        }

        public static Data.Containers.PeakShape GetPeakShape(RawDataCollection rawData, int scan)
        {
            double[] Intensities, RetTimes;
            int maxIndex, currentIndex;
            Dictionary<int, double> a, b;
            Asymmetry asymmetry = new Asymmetry();
            Width width = new Width();
            a = new Dictionary<int, double>();
            b = new Dictionary<int, double>();

            Intensities = rawData.peakData[scan].Intensities;
            RetTimes = rawData.peakData[scan].RetTimes;
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

        public static void CalculatePeakShapes(this RawDataCollection rawData, IRawDataPlus rawFile, int percentile = 90)
        {
            List<Operations> list = new List<Operations>() { Operations.PeakRetAndInt };
            CheckIfDone.Check(rawData, rawFile, list);

            // in this first step we chose scans which are in the ith percentile
            List<double> Intensities = (from x in rawData.peakData.Keys.ToArray() select rawData.peakData[x].MaximumIntensity).ToList();
            Intensities.Sort();
            Intensities.Reverse();
            Intensities = Intensities.GetRange(0, Intensities.Count() / (100 - percentile));

            int[] scans = (from x in rawData.peakData.Keys.ToArray() where Intensities.Contains(rawData.peakData[x].MaximumIntensity) select x).ToArray();
            DistributionMultiple allPeaksAsymmetry = new DistributionMultiple();
            DistributionMultiple allPeaksWidths = new DistributionMultiple();
            ProgressIndicator P = new ProgressIndicator(scans.Length, "Calculating peak symmetries");

            for (int i = 0; i < scans.Count(); i++)
            {
                if (rawData.peakData[scans[i]].NScans < 5 | rawData.peakData[scans[i]].PeakFound == false |
                    rawData.peakData[scans[i]].ContainsFirstMS1Scan | rawData.peakData[scans[i]].ContainsLastMS1Scan)
                {
                    rawData.peakData[scans[i]].PeakShape = null;
                    P.Update();
                    continue;
                }
                rawData.peakData[scans[i]].PeakShape = GetPeakShape(rawData, scans[i]);

                allPeaksAsymmetry.Add(rawData.peakData[scans[i]].PeakShape.Asymmetry);
                allPeaksWidths.Add(rawData.peakData[scans[i]].PeakShape.Width);
                P.Update();
            }
            P.Done();

            rawData.Performed.Add(Operations.PeakShape);
            if (allPeaksWidths.P50.Count() == 0)
            {
                rawData.peakData.PeakShapeMedians = new Data.Containers.PeakShape(width: new Width(), asymmetry: new Asymmetry(), peakMax: 0);
            }
            else
            {
                rawData.peakData.PeakShapeMedians = new Data.Containers.PeakShape(width: allPeaksWidths.GetMedians(), asymmetry: allPeaksAsymmetry.GetMedians(), peakMax: 0);
            }
        }
        /*
        static double FindMaxRetTime(PrecursorPeakData peakData, alglib.spline1dinterpolant splineInterpolant)
        {
            double[] rets = new double[10];
            double[] ints = new double[10];
            double firstRet = peakData.RetTimes.First();
            double lastRet = peakData.RetTimes.Last();
            int maxIndex = 0;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    rets[i] = firstRet + (lastRet - firstRet) * (i / 9);
                    ints[i] = alglib.spline1dcalc(splineInterpolant, rets[i]);
                }

                maxIndex = Array.FindIndex(ints, x => x == ints.Max());
                if (maxIndex == 0)
                {
                    return firstRet;
                }
                
                firstRet = rets[maxIndex - 1];
                lastRet = rets[maxIndex + 1];
            }

            return rets[maxIndex];
        }
        
        static (double a, double b) FindShoulderRetTimes(PrecursorPeakData peakData, double centerRetTime, alglib.spline1dinterpolant splineInterpolant, int percentHeight)
        {
            double[] rets = new double[10];
            double[] ints = new double[10];
            double[] diffs = new double[10];
            double firstRet = peakData.RetTimes.First();
            double lastRet = peakData.RetTimes.Last();
            int leftIndex = 0,
                rightIndex = 0;
            double a, b;
            double maxIntensity = alglib.spline1dcalc(splineInterpolant, centerRetTime);
            double findIntensity = maxIntensity * percentHeight / 100;

            // FindLeftShoulder
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    rets[i] = firstRet + (centerRetTime - firstRet) * (i / 9);
                    ints[i] = alglib.spline1dcalc(splineInterpolant, rets[i]);
                    diffs[i] = Math.Abs(ints[i] - findIntensity);
                }

                leftIndex = Array.FindIndex(diffs, x => x == diffs.Min());
                if (leftIndex == 0)
                {
                    break;
                }
                firstRet = rets[leftIndex - 1];
                lastRet = rets[leftIndex + 1];
            }
            a = rets[leftIndex];

            // FindRightShoulder
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    rets[i] = centerRetTime + (lastRet - centerRetTime) * (i / 9);
                    ints[i] = alglib.spline1dcalc(splineInterpolant, rets[i]);
                    diffs[i] = Math.Abs(ints[i] - findIntensity);
                }

                rightIndex = Array.FindIndex(diffs, x => x == diffs.Min());
                if (rightIndex == rets.Length - 1 | rightIndex == 0)
                {
                    break;
                }
                firstRet = rets[rightIndex - 1];
                lastRet = rets[rightIndex + 1];
            }
            b = rets[rightIndex];

            return (a : rets[leftIndex], b : rets[rightIndex]);
        }
        /*
        static alglib.spline1dinterpolant FitPeak(RawDataCollection rawData, int scan)
        {
            int info;
            double v;
            alglib.spline1dinterpolant s;
            alglib.spline1dfitreport rep;
            double rho = 2;
            alglib.spline1dfitpenalized(rawData.peakData[scan].RetTimes, rawData.peakData[scan].Intensities, 10, rho, out info, out s, out rep);

            return s;
        }
        
        public static (Distribution Asymmetry, Distribution Width, double PeakMaxRetTime) SplinePeakShape(RawDataCollection rawData, int scan)
        {
            alglib.spline1dinterpolant fit = FitPeak(rawData, scan);
            Dictionary<int, (double a, double b)> PeakShoulders = new Dictionary<int, (double a, double b)>();
            Distribution Asymmetry = new Distribution();
            Distribution Width = new Distribution();

            double PeakMaxRetTime = FindMaxRetTime(rawData.peakData[scan], fit);

            foreach (int percent in new List<int> { 16, 25, 50, 75, 84 })
            {
                PeakShoulders.Add(percent, FindShoulderRetTimes(rawData.peakData[scan], PeakMaxRetTime, fit, percent));
            }

            Asymmetry.P16 = PeakShoulders[16].b / PeakShoulders[16].a;
            Asymmetry.P25 = PeakShoulders[25].b / PeakShoulders[25].a;
            Asymmetry.P50 = PeakShoulders[50].b / PeakShoulders[50].a;
            Asymmetry.P75 = PeakShoulders[75].b / PeakShoulders[75].a;
            Asymmetry.P84 = PeakShoulders[84].b / PeakShoulders[84].a;

            Width.P16 = PeakShoulders[16].b + PeakShoulders[16].a;
            Width.P25 = PeakShoulders[25].b + PeakShoulders[25].a;
            Width.P50 = PeakShoulders[50].b + PeakShoulders[50].a;
            Width.P75 = PeakShoulders[75].b + PeakShoulders[75].a;
            Width.P84 = PeakShoulders[84].b + PeakShoulders[84].a;

            return (Asymmetry: Asymmetry, Width: Width, PeakMaxRetTime: PeakMaxRetTime);
        }

        public static void AddPeakShapesToPeakData(RawDataCollection rawData)
        {
            // Not implemented. This method is for using a spline fitting, which we are not doing right now.
            (Distribution Asymmetry, Distribution Width, double PeakMaxRetTime) peak;
            ProgressIndicator P = new ProgressIndicator(rawData.peakData.Keys.Count, "Analyzing peak shapes");
            Containers.PeakShape PeakShape = new Containers.PeakShape();

            foreach (int scan in rawData.peakData.Keys)
            {
                if (!rawData.peakData[scan].PeakFound | rawData.peakData[scan].NScans < 3)
                {
                    continue;
                }
                peak = SplinePeakShape(rawData, scan);

                PeakShape.Asymmetry = peak.Asymmetry;
                PeakShape.MaxRetTime = peak.PeakMaxRetTime;
                PeakShape.Width = peak.Width;

                rawData.peakData[scan].PeakShape = PeakShape;

                P.Update();
            }
            P.Done();
        }
        */
        public static double CalculatePeakArea(PrecursorPeakData peak)
        {
            double area = 0;

            for (int i = 1; i < peak.NScans; i++)
            {
                area += (peak.Intensities[i - 1] + peak.Intensities[i]) * (peak.RetTimes[i] - peak.RetTimes[i - 1]) / 2;
            }

            return area;
        }

        public static void QuantifyPrecursorPeaks(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            List<Operations> list = new List<Operations>() { Operations.PeakRetAndInt };
            CheckIfDone.Check(rawData, rawFile, list);
            int[] scans = rawData.peakData.Keys.ToArray();
            ProgressIndicator P = new ProgressIndicator(scans.Length, "Integrating precursor peaks");

            for (int i = 0; i < scans.Count(); i++)
            {
                if (rawData.peakData[scans[i]].PeakFound == false)
                {
                    P.Update();
                    continue;
                }

                rawData.peakData[scans[i]].Area = CalculatePeakArea(rawData.peakData[scans[i]]);
                P.Update();
            }
            P.Done();

            rawData.Performed.Add(Operations.PeakArea);
        }
    }
}
