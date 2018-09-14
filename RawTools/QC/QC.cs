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
using System.IO;
using System.Xml.Serialization;
using System.Threading.Tasks;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools.Data.Processing;
using RawTools.Data.Extraction;
using RawTools.Data.IO;
using RawTools.Utilities;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Business;
using Serilog;
using RawTools;


namespace RawTools.QC
{
    
    static class QC
    {
        public static QcDataContainer ProcessQcData(this QcDataCollection Data, RawDataCollection rawData, IRawDataPlus rawFile, string qcDirectory, string fastaDB = null)
        {
            DateTime dateAcquired = rawFile.CreationDate;
            //RawDataCollection rawData = new RawDataCollection(rawFile);

            MetricsData metricsData = new MetricsData();
            metricsData.GetMetricsData(metaData: rawData.metaData, rawData: rawData, rawFile: rawFile);

            QcDataContainer qcData = new QcDataContainer(rawData.rawFileName, dateAcquired);

            qcData.Instrument = rawData.instrument;
            qcData.ExperimentMsOrder = rawData.methodData.AnalysisOrder;
            qcData.Ms1Analyzer = rawData.methodData.MassAnalyzers[MSOrderType.Ms].ToString();
            qcData.Ms2Analyzer = rawData.methodData.MassAnalyzers[MSOrderType.Ms2].ToString();

            if (qcData.ExperimentMsOrder == MSOrderType.Ms3)
            {
                qcData.Ms3Analyzer = rawData.methodData.MassAnalyzers[MSOrderType.Ms3].ToString();
            }
            else
            {
                qcData.Ms3Analyzer = "None";
            }

            qcData.TotalScans = metricsData.TotalScans;
            qcData.NumMs1Scans = metricsData.MS1Scans;
            qcData.NumMs2Scans = metricsData.MS2Scans;
            qcData.NumMs3Scans = metricsData.MS3Scans;

            qcData.Ms1ScanRate = metricsData.MS1ScanRate;
            qcData.Ms2ScanRate = metricsData.MS2ScanRate;

            qcData.MeanDutyCycle = metricsData.MeanDutyCycle;

            qcData.MeanTopN = metricsData.MeanTopN;

            qcData.MedianPrecursorIntensity = metricsData.MedianPrecursorIntensity;
            qcData.MedianSummedMs2Intensity = metricsData.MedianSummedMS2Intensity;
            qcData.MedianMs2FractionConsumingTop80PercentTotalIntensity = metricsData.MedianMs2FractionConsumingTop80PercentTotalIntensity;

            qcData.NumEsiStabilityFlags = NumberOfEsiFlags(rawData);

            qcData.QuantMeta = metricsData.QuantMeta;

            qcData.GradientTime = metricsData.Gradient;
            qcData.ColumnPeakCapacity = metricsData.PeakCapacity;
            qcData.ChromIntMetrics(rawData, metricsData);

            if (!rawData.isBoxCar)
            {
                qcData.PeakShape.Asymmetry.P10 = rawData.peakData.PeakShapeMedians.Asymmetry.P10;
                qcData.PeakShape.Asymmetry.P50 = rawData.peakData.PeakShapeMedians.Asymmetry.P50;

                qcData.PeakShape.Width.P10 = rawData.peakData.PeakShapeMedians.Width.P10;
                qcData.PeakShape.Width.P50 = rawData.peakData.PeakShapeMedians.Width.P50;
            }



            // add the signal-to-noise distribution to the QC data. These are presented as "median of the ith percentile", so for example we take all the 10th percentile values of
            // the S2N and put them in a list, then report the median of that list
                
            qcData.MedianSummedMs1Intensity = (from x in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms] select rawData.centroidStreams[x].Intensities.Sum()).ToArray().Percentile(50);

            // add the fill-time distribution to the QC data. This is more straightforward. Just put all the fill times in an array and use it to instantiate a new distribution.
            qcData.Ms1FillTime = new Distribution((from x in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms] select rawData.trailerExtras[x].InjectionTime).ToArray());
            qcData.Ms2FillTime = new Distribution((from x in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2] select rawData.trailerExtras[x].InjectionTime).ToArray());
            qcData.Ms3FillTime = new Distribution((from x in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms3] select rawData.trailerExtras[x].InjectionTime).ToArray());
            
            //Data.QcData.Add(dateAcquired, newData);
            //Data.ProcessedRawFiles.Add(Path.GetFileName(rawData.rawFileName));

            return qcData;
        }

        public static void DoQc(QcParameters qcParameters)
        {
            QcDataCollection qcDataCollection;
            string dataDirectory = qcParameters.RawFileDirectory;
            string qcDirectory = qcParameters.QcDirectory;
            string qcSearchDataDirecotry = qcParameters.QcSearchDataDirectory;
            SearchParameters searchParameters = qcParameters.searchParameters;

            // our qc file
            string qcFile = Path.Combine(qcDirectory, "QC.xml");

            // see if the file exists
            if (File.Exists(qcFile))
            {
                // if so, open it
                try
                {
                    qcDataCollection = XmlSerialization.ReadFromXmlFile<QcDataCollection>(qcFile);
                    Log.Information("QC data file loaded successfully");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed while loading QC data");
                    throw e;
                }
            }
            else
            {
                // if not, check if the directory exists
                if (!Directory.Exists(qcDirectory))
                {
                    Directory.CreateDirectory(qcDirectory);
                }

                qcDataCollection = new QcDataCollection(dataDirectory, qcDirectory);
                Log.Information("Appears to be a new QC directory. New QC data collection created.");
            }

            // get our list of new raw files. it is every raw file in the directory that is not listed in the qc data
            var fileList = Directory.GetFiles(dataDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();

            if (fileList.Count() == 0)
            {
                Log.Error("No raw files found in {Directory}", dataDirectory);
                Console.WriteLine("{0} contains no raw files!", dataDirectory);
                Environment.Exit(1);
            }

            fileList.RemoveAll(s => qcDataCollection.ProcessedRawFiles.Contains(Path.GetFileName(s)));

            Log.Information("Raw files in QC queue: {Files}", fileList);

            if (fileList.Count() == 0)
            {
                Log.Information("No new files to QC");
                Console.WriteLine("No new files in the directory to QC!");
                Environment.Exit(0);
            }

            Console.WriteLine("{0} file(s) to process", fileList.Count());

            foreach (string fileName in fileList)
            {
                Console.WriteLine("Processing {0}", fileName);

                IFileHeader rawHeader;

                // try to open the raw file header
                try
                {
                    rawHeader = FileHeaderReaderFactory.ReadFile(fileName); ;
                }
                catch (Exception)
                {
                    Log.Information("{File} is not a valid raw file", fileName);
                    Console.WriteLine("{0} is not a valid raw file, continuing to next file.", fileName);
                    continue;
                }

                // is it a real raw file?
                if (rawHeader.FileType == FileType.RawFile)
                {
                    Log.Information("{File} is a valid raw file", fileName);
                    Log.Information("Creation date: {Date}", rawHeader.CreationDate);
                    Log.Information("File description: {Description}", rawHeader.FileDescription);
                }
                else
                {
                    Log.Information("{File} is not a valid raw file", fileName);
                    Console.WriteLine("{0} is not a valid raw file, continuing to next file.", fileName);
                    continue;
                }
                // okay, it is probably a real raw file, let's do the QC

                // check if the raw file already exists in the QC data with a different name
                if (qcDataCollection.QcData.Keys.Contains(rawHeader.CreationDate))
                {
                    Log.Information("A file with the same creation date and time as {File} already exists in the QC data", fileName);
                    Console.WriteLine("{0} appears to already exist in the QC data with the name {1}. Skipping to next file.",
                        fileName, qcDataCollection.QcData[rawHeader.CreationDate].RawFile);
                    continue;
                }

                using (IRawDataPlus rawFile = RawFileReaderFactory.ReadFile(fileName))
                {
                    rawFile.SelectInstrument(Device.MS, 1);
                    RawDataCollection rawData = new RawDataCollection(rawFile);
                    rawData.ExtractAll(rawFile);

                    /*
                    if (idpyPars?.QuantMods != null)
                    {
                        rawData.quantData.Quantify(rawData, rawFile, )
                    }
                    */

                    QcDataContainer newQcData = ProcessQcData(Data: qcDataCollection, rawData: rawData, rawFile: rawFile, qcDirectory: qcDirectory);

                    if (searchParameters != null)
                    {
                        Search.WriteSearchMGF(qcParameters, rawData, rawFile);
                        Search.RunSearch(qcParameters, rawData, rawFile);
                        SearchQC.RunQC(newQcData, rawData, rawFile, qcParameters);
                        newQcData.IdentipyParameters = String.Format("\"fmods: {0}; nmod: {1}; kmod: {2}; xmod: {3}; fastaDB: {4}; pythonExecutable: {5}; identipyScript: {6}\"",
                            searchParameters.FixedMods, searchParameters.NMod, searchParameters.KMod, searchParameters.XMod, searchParameters.FastaDatabase, searchParameters.PythonExecutable, searchParameters.IdentipyScript);
                    }

                    qcDataCollection.QcData.Add(rawFile.CreationDate, newQcData);
                    qcDataCollection.ProcessedRawFiles.Add(Path.GetFileName(rawData.rawFileName));
                    qcDataCollection.WriteQcToTable();
                }

                Log.Information("QC finished: {File}", fileName);
            }

            Log.Information("QC of all files completed");
            Console.WriteLine("QC of all files completed!");

            try
            {
                XmlSerialization.WriteToXmlFile<QcDataCollection>(qcFile, qcDataCollection);
                Log.Information("QC file saved successfully");
                Console.WriteLine("QC file saved successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed during serialization of QC data");
                throw e;
            }
        }

        public static int NumberOfEsiFlags(RawDataCollection rawData)
        {
            int flags = 0;

            int[] scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms];

            for (int i = 2; i < scans.Length; i++)
            {
                if (rawData.metaData[scans[i]].SummedIntensity / rawData.metaData[scans[i - 1]].SummedIntensity < 0.1)
                {
                    flags += 1;
                }
                if (rawData.metaData[scans[i]].SummedIntensity / rawData.metaData[scans[i - 1]].SummedIntensity > 10)
                {
                    flags += 1;
                }
            }
            return flags;
        }

        public static void ChromIntMetrics(this QcDataContainer qcData, RawDataCollection rawData, MetricsData metrics)
        {
            double firstRtToExceed10 = 0;
            double lastRtToExceed10 = 0;
            double proportionCovered;
            var scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms];
            var reversedScans = scans.Reverse();
            var totalIntList = (from x in scans select rawData.metaData[x].SummedIntensity).ToArray();

            // get Q1 of total intensity from all scans
            double threshold = totalIntList.Max()/10;

            // get first RT which exceeds Q1
            for (int i = 0; i < scans.Length; i++)
            {
                int scan = scans[i];
                if (totalIntList.MovingAverage(i, 20) > threshold)
                {
                    firstRtToExceed10 = rawData.retentionTimes[scan];
                    break;
                }
            }

            for (int i = scans.Length - 1; i >= 0; i--)
            {
                int scan = scans[i];
                if (totalIntList.MovingAverage(i, 20) > threshold)
                {
                    lastRtToExceed10 = rawData.retentionTimes[scan];
                    break;
                }
            }

            // get proportion of run encompassed by these times
            //proportionCovered = (lastRtToExceedQ1 - firstRtToExceedQ1) / metrics.TotalAnalysisTime;
            proportionCovered = (lastRtToExceed10 - firstRtToExceed10) / rawData.retentionTimes[rawData.scanIndex.ScanEnumerators[MSOrderType.Ms].Last()];

            qcData.TimeBeforeFirstScanToExceedPoint1MaxIntensity = firstRtToExceed10;// - rawData.retentionTimes[1];
            qcData.TimeAfterLastScanToExceedPoint1MaxIntensity = rawData.retentionTimes[rawData.scanIndex.ScanEnumerators[MSOrderType.Ms].Last()] - lastRtToExceed10;
            qcData.FractionOfRunAbovePoint1MaxIntensity = proportionCovered;
        }
    }
    
    //[Serializable]
    public class QcDataContainer
    {
        public string RawFile, Instrument;
        public MSOrderType ExperimentMsOrder;
        public string Ms1Analyzer, Ms2Analyzer, Ms3Analyzer;
        public DateTime DateAcquired;
        public int TotalScans, NumMs1Scans, NumMs2Scans, NumMs3Scans;
        public double Ms1ScanRate, Ms2ScanRate, Ms3ScanRate;
        public double MeanDutyCycle;
        public double MeanTopN;
        public double MedianSummedMs2Intensity;
        public double MedianSummedMs1Intensity;
        public double MedianMs2FractionConsumingTop80PercentTotalIntensity;
        public double MedianPrecursorIntensity;
        public double LabelingEfficiencyAtX, LabelingEfficiencyAtNTerm, LabelingEfficiencyAtK;
        public string LabelX;
        public double ColumnPeakCapacity, GradientTime, IdentificationRate, MissedCleavageRate, DigestionEfficiency, ChargeRatio3to2, ChargeRatio4to2;
        public double MedianMassDrift;
        public ((double P10, double P50) Asymmetry, (double P10, double P50) Width) PeakShape;
        public double TimeBeforeFirstScanToExceedPoint1MaxIntensity;
        public double TimeAfterLastScanToExceedPoint1MaxIntensity;
        public double FractionOfRunAbovePoint1MaxIntensity;
        public string IdentipyParameters = "None";
        public int NumEsiStabilityFlags;

        public Distribution Ms1FillTime, Ms2FillTime, Ms3FillTime;

        public QuantMetaData QuantMeta;

        public QcDataContainer()
        { }

        public QcDataContainer(string rawFile, DateTime dateAquired)
        {
            RawFile = rawFile;
            DateAcquired = dateAquired;
            Ms1FillTime = Ms2FillTime = Ms3FillTime = new Distribution();
            LabelingEfficiencyAtX = LabelingEfficiencyAtNTerm = LabelingEfficiencyAtK = -1;
            DigestionEfficiency = IdentificationRate = MissedCleavageRate = -1;
            ChargeRatio3to2 = ChargeRatio4to2 = -1;
            Ms3ScanRate = -1;
            MedianMassDrift = -1;
        }
    }

    //[Serializable]
    public class QcDataCollection
    {
        public SerializableDictionary<DateTime, QcDataContainer> QcData;
        public string DirectoryToWatch, QcDirectory, QcFile;
        //public StringBuilder FileOut;
        public List<string> ProcessedRawFiles;

        public QcDataCollection()
        {
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, QcDataContainer>();
        }

        public QcDataCollection(string directoryToWatch, string qcDirectory)
        {
            DirectoryToWatch = directoryToWatch;
            QcDirectory = qcDirectory;
            QcFile = Path.Combine(new string[] { qcDirectory, "QC.xml" });
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, QcDataContainer>();
        }
    }
}
