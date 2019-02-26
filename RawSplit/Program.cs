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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.Interfaces;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.Algorithms.ExtractData;

namespace RawSplit
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            if (Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Console.Out.NewLine = "\n\n";
            }

            var FileList = new List<string>();

            if (args[0] == "-f")
            {
                FileList.Add(args[1]);
            }
            else if (args[0] == "-d")
            {
                FileList = Directory.GetFiles(args[1], "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                Console.WriteLine("Unknown command. Please use this interface: \"RawSplit.exe {-f, -d} X\", where X is" +
                    "a file (-f) or a directory of files (-d) to process.");
            }

            CentroidStreamCollection centroids;
            SegmentScanCollection segments;
            RetentionTimeCollection retentionTimes;
            ScanIndex Index;
            CentroidStreamData centroid;
            SegmentedScanData segment;
            Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();
            string mode;
            ProgressIndicator P;

            writers.Add("P", null);
            writers.Add("N", null);

            foreach (string file in FileList)
            {
                Console.WriteLine("\nSplitting: {0}", Path.GetFileName(file));
                using (var rawFile = RawFileReaderFactory.ReadFile(file))
                {
                    rawFile.SelectMsData();

                    writers["P"] = new StreamWriter(file + "_Postitive.mgf", append: false);
                    writers["N"]= new StreamWriter(file + "_Negative.mgf", append: false);

                    Index = Extract.ScanIndices(rawFile);

                    retentionTimes = Extract.RetentionTimes(rawFile, Index);

                    var events = new Dictionary<int, IScanEvent>();

                    foreach (int scan in Index.ScanEnumerators[MSOrderType.Ms])
                    {
                        events.Add(scan, rawFile.GetScanEventForScanNumber(scan));
                    }

                    (centroids, segments) = Extract.MsData(rawFile, Index);

                    P = new ProgressIndicator(Index.ScanEnumerators[MSOrderType.Ms].Length, String.Format("Splitting {0}", Path.GetFileName(file)));
                    P.Start();

                    foreach (var scan in Index.ScanEnumerators[MSOrderType.Ms])
                    {
                        if (events[scan].Polarity == PolarityType.Positive) mode = "P";
                        else mode = "N";

                        writers[mode].WriteLine("BEGIN IONS");
                        writers[mode].WriteLine("TITLE=Spectrum_{0}", scan);
                        writers[mode].WriteLine("RTINSECONDS={0}", retentionTimes[scan] * 60);
                        writers[mode].WriteLine("SCAN={0}", scan);
                        writers[mode].WriteLine("RAWFILE={0}", file);

                        if (Index.allScans[scan].MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                        {
                            centroid = centroids[scan];

                            for (int j = 0; j < centroid.Masses.Length; j++)
                            {
                                writers[mode].WriteLine("{0} {1}", Math.Round(centroid.Masses[j], 5), Math.Round(centroid.Intensities[j], 4));
                            }
                        }
                        else
                        {
                            segment = segments[scan];

                            for (int j = 0; j < segment.Positions.Length; j++)
                            {
                                writers[mode].WriteLine("{0} {1}", Math.Round(segment.Positions[j], 5), Math.Round(segment.Intensities[j], 4));
                            }
                        }
                        writers[mode].WriteLine("END IONS\n");
                        P.Update();
                    }
                    P.Done();
                }
                writers["P"].Close();
                writers["N"].Close();
            }
        }
    }
}

