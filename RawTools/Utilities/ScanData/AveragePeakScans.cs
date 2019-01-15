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
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;

namespace RawTools.Utilities.ScanData
{
    static class AveragePeakScans
    {
        /* Need to look at this to see how and if to adapt from RawDataCollection format data
        public static CentroidStreamData GetAverageScan(this RawDataCollection rawData, IRawDataPlus rawFile, int scan)
        {
            if (rawData.peakData[scan].PeakFound == false)
            {
                return new CentroidStreamData(rawFile.GetCentroidStream(scan, false));
            }
            int maxScan = rawData.peakData[scan].MaxScan;
            int[] scans = rawData.peakData[scan].Scans;
            int[] topScans;

            // this is here so it doesn't throw an error during compilation while I work on something else
            //return (rawData.centroidStreams[scans[0]].Masses, rawData.centroidStreams[scans[0]].Intensities);

            if (scans.Length < 4)
            {
                if (scans.Length <= 1)
                {
                    return new CentroidStreamData(rawFile.GetCentroidStream(scan, false));
                }
                else
                {
                    topScans = scans;
                }
            }
            else
            {
                int maxIndex = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms].FindAllIndex1(x => x == maxScan).First();
                int previous = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms][maxIndex - 1];
                if (maxIndex < rawData.scanIndex.ScanEnumerators[MSOrderType.Ms].Length - 1)
                {
                    int next = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms][maxIndex + 1];
                    topScans = new int[] { previous, maxIndex, next };
                }
                else
                {
                    topScans = new int[] { previous, maxIndex };
                }
                
            }

            scans = topScans;
            //for (int x = 0; x < scans.Length; x++) Console.Write(scans[x].ToString() + " ");

            var average = rawFile.AverageScans(scans.ToList()).CentroidScan;

            return new CentroidStreamData(average);

            IEnumerable<(double[] masses, double[] intensities)> scanData = from x in scans
                                                                 select (rawData.centroidStreams[x].Masses, rawData.centroidStreams[x].Intensities);
            int lenOut = (from x in scanData select x.masses.Length).Max();

            (double[] masses, double[] intensities) averageData;
            averageData.masses = new double[lenOut];
            averageData.intensities = new double[lenOut];

            for (int i = 0; i < lenOut; i++)
            {
                
            }
        }
        */
    }
}
