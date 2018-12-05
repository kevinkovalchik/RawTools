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
using System.Linq;
using RawTools.Data.Containers;
using RawTools.Utilities;

namespace RawTools.Algorithms
{
    static class QuantifyReporters
    {

        // first instance method is for FTMS data
        static public QuantData QuantifyOneScan(CentroidStreamData centroidStream, string labelingReagent)
        {
            (string[] Labels, double[] Masses) reporters = new LabelingReagents().Reagents[labelingReagent];
            QuantData quantData = new QuantData();
            int index;
            double mass;
            double[] massDiff;

            for (int i = 0; i < reporters.Masses.Length; i++)
            {
                mass = reporters.Masses[i];
                int[] indices = centroidStream.Masses.FindAllIndex1((x) =>
                {
                    if (Math.Abs(x - mass) / mass * 1e6 < 10)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (indices.Length > 1)
                {
                    massDiff = new double[indices.Length];
                    for (int ix = 0; ix < indices.Length; ix++)
                    {
                        massDiff[ix] = Math.Abs(centroidStream.Masses[indices[ix]] - mass);
                    }

                    index = -1;
                    for (int j = 0; j < indices.Length; j++)
                    {
                        if (massDiff[j] == massDiff.Min())
                        {
                            index = indices[j];
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //index = Array.Find(indices, x => centroidStream.Masses[indices[x]] - mass == massDiff.Min());

                    if (index != -1)
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(centroidStream.Masses[index], centroidStream.Intensities[index],
                                                                   centroidStream.Noises[index], centroidStream.Resolutions[index],
                                                                   centroidStream.Baselines[index], centroidStream.SignalToNoise[index],
                                                                   massDiff[index] / mass * 1e6));
                    }
                    else
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(0, 0, 0, 0, 0, 0, 0));
                    }
                }
                else
                {
                    if (indices.Length == 1)
                    {
                        index = indices.Single();
                        var diff = Math.Abs(centroidStream.Masses[index] - mass);
                        quantData.Add(reporters.Labels[i], new ReporterIon(centroidStream.Masses[index], centroidStream.Intensities[index],
                                                                   centroidStream.Noises[index], centroidStream.Resolutions[index],
                                                                   centroidStream.Baselines[index], centroidStream.SignalToNoise[index],
                                                                   diff / mass * 1e6));
                    }
                    else
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(0, 0, 0, 0, 0, 0, 0));
                    }
                }
            }

            return quantData;
        }

        // second instance method is for ITMS data
        public static QuantData QuantifyOneScan(SegmentedScanData segmentedScan, string labelingReagent)
        {
            (string[] Labels, double[] Masses) reporters = new LabelingReagents().Reagents[labelingReagent];
            QuantData quantData = new QuantData();

            int index;

            for (int i = 0; i < reporters.Masses.Length; i++)
            {
                double mass = reporters.Masses[i];
                int[] indices = segmentedScan.Positions.FindAllIndex1((x) =>
                {
                    if (Math.Abs(x - mass) / mass * 1e6 < 10)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (indices.Length > 1)
                {
                    double[] massDiff = new double[indices.Length];
                    for (int ix = 0; ix < indices.Length; ix++)
                    {
                        massDiff[ix] = Math.Abs(segmentedScan.Positions[indices[ix]] - mass);
                    }

                    index = Array.Find(indices, (x) => { return massDiff[x] == massDiff.Min(); });

                    quantData.Add(reporters.Labels[i], new ReporterIon(segmentedScan.Positions[index],
                        segmentedScan.Intensities[index], massDiff[index] / mass * 1e6));

                }
                else
                {
                    if (indices.Length == 1)
                    {
                        index = indices[0];
                        var diff = Math.Abs(segmentedScan.Positions[index] - mass);
                        quantData.Add(reporters.Labels[i], new ReporterIon(segmentedScan.Positions[index],
                            segmentedScan.Intensities[index], diff / mass * 1e6));
                    }
                    else
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(0, 0, 0));
                    }

                }


            }
            return quantData;
        }
    }
}
