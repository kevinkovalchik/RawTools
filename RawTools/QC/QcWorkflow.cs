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
using RawTools.Algorithms;
using RawTools.Algorithms.Analyze;
using RawTools.WorkFlows;
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
    
    static class QcWorkflow
    {
        public static void UpdateQcCollection(QcDataCollectionDDA qcDataCollection, SearchMetricsContainer newSearchMetrics, RawMetricsDataDDA newRawMetrics, MethodDataContainer methodData, string rawFileName)
        {
            qcDataCollection.QcData.Add(methodData.CreationDate, (newRawMetrics ,newSearchMetrics));
            qcDataCollection.ProcessedRawFiles.Add(Path.GetFileName(rawFileName));
            qcDataCollection.WriteQcToTable();
            Console.WriteLine("QC data written to csv file.");
            
            try
            {
                XmlSerialization.WriteToXmlFile<QcDataCollectionDDA>(qcDataCollection.QcFile, qcDataCollection);
                Log.Information("QC file saved successfully");
                Console.WriteLine("QC file saved successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed during serialization of QC data");
                Console.WriteLine("ERROR: failure during serialization of QC data.");
                Environment.Exit(1);
            }
        }

        public static void UpdateQcCollection(QcDataCollectionDIA qcDataCollection, RawMetricsDataDIA newRawMetrics, MethodDataContainer methodData, string rawFileName)
        {
            qcDataCollection.QcData.Add(methodData.CreationDate, newRawMetrics);
            qcDataCollection.ProcessedRawFiles.Add(Path.GetFileName(rawFileName));
            qcDataCollection.WriteQcToTable();
            Console.WriteLine("QC data written to csv file.");

            try
            {
                XmlSerialization.WriteToXmlFile<QcDataCollectionDIA>(qcDataCollection.QcFile, qcDataCollection);
                Log.Information("QC file saved successfully");
                Console.WriteLine("QC file saved successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed during serialization of QC data");
                Console.WriteLine("ERROR: failure during serialization of QC data.");
                Environment.Exit(1);
            }
        }

        public static void DoQc(WorkflowParameters parameters)
        {
            QcDataCollectionDDA qcDataCollection;
            string dataDirectory = parameters.RawFileDirectory;
            string qcDirectory = parameters.QcParams.QcDirectory;
            string qcSearchDataDirecotry = parameters.QcParams.QcSearchDataDirectory;
            List<string> fileList = new List<string>();

            string qcFile = Path.Combine(qcDirectory, "QC.xml");

            (fileList, qcDataCollection) = GetFileListAndQcFile(parameters);

            foreach (string fileName in fileList)
            {
                Console.WriteLine("Processing {0}", fileName);

                if (!RawFileInfo.CheckIfValid(fileName))
                {
                    continue;
                }
                // okay, it is probably a real raw file, let's do the QC

                // check if the raw file already exists in the QC data with a different name
                if (CheckIfFilePresentInQcCollection(fileName, qcDataCollection))
                {
                    Log.Information("A file with the same creation date and time as {File} already exists in the QC data", fileName);

                    Console.WriteLine("A file with the same creation date and time as {File} already exists in the QC data. Skipping to next file.",
                        fileName);

                    continue;
                }

                using (IRawDataPlus rawFile = RawFileReaderFactory.ReadFile(fileName))
                {
                    rawFile.SelectInstrument(Device.MS, 1);

                    WorkFlowsDDA.QcDDA(rawFile, parameters);
                }

                Log.Information("QC finished: {File}", fileName);
            }

            Log.Information("QC of all files completed");
            Console.WriteLine("QC of all files completed!");
        }

        public static QcDataCollectionDDA LoadOrCreateQcCollection(WorkflowParameters parameters)
        {
            QcDataCollectionDDA qcDataCollection;
            string qcFile = Path.Combine(parameters.QcParams.QcDirectory, "QC.xml");

            // see if the file exists
            if (File.Exists(qcFile))
            {
                // if so, open it
                try
                {
                    qcDataCollection = XmlSerialization.ReadFromXmlFile<QcDataCollectionDDA>(qcFile);
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
                if (!Directory.Exists(parameters.QcParams.QcDirectory))
                {
                    Directory.CreateDirectory(parameters.QcParams.QcDirectory);
                }

                qcDataCollection = new QcDataCollectionDDA(parameters.RawFileDirectory, parameters.QcParams.QcDirectory);
                Log.Information("Appears to be a new QC directory. New QC data collection created.");
            }

            return qcDataCollection;

        }

        private static (List<string> fileList, QcDataCollectionDDA qcDataCollection) GetFileListAndQcFile(WorkflowParameters parameters)
        {
            QcDataCollectionDDA qcDataCollection;
            string dataDirectory = parameters.RawFileDirectory;
            string qcDirectory = parameters.QcParams.QcDirectory;
            string qcSearchDataDirecotry = parameters.QcParams.QcSearchDataDirectory;
            SearchParameters searchParameters = parameters.QcParams.SearchParameters;

            // our qc file
            string qcFile = Path.Combine(qcDirectory, "QC.xml");

            // see if the file exists
            if (File.Exists(qcFile))
            {
                // if so, open it
                try
                {
                    qcDataCollection = XmlSerialization.ReadFromXmlFile<QcDataCollectionDDA>(qcFile);
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

                qcDataCollection = new QcDataCollectionDDA(dataDirectory, qcDirectory);
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

            return (fileList, qcDataCollection);
        }

        private static bool CheckIfFilePresentInQcCollection(string fileName, QcDataCollectionDDA qcDataCollection)
        {
            IFileHeader rawHeader = FileHeaderReaderFactory.ReadFile(fileName);

            if (qcDataCollection.QcData.Keys.Contains(rawHeader.CreationDate))
            {
                return true;
            }
            else
            {
                return false;
            }
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
        public double MedianMs1IsolationInterference;
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
        public SearchData SearchData;

        public Distribution Ms1FillTimeDistribution, Ms2FillTimeDistribution, Ms3FillTimeDistribution;

        public QuantMetaData QuantMeta;

        public QcDataContainer()
        { }

        public QcDataContainer(string rawFile, DateTime dateAquired, RawMetricsDataDDA metricsData)
        {
            RawFile = rawFile;
            DateAcquired = dateAquired;
            Ms1FillTimeDistribution = Ms2FillTimeDistribution = Ms3FillTimeDistribution = new Distribution();
            LabelingEfficiencyAtX = LabelingEfficiencyAtNTerm = LabelingEfficiencyAtK = -1;
            DigestionEfficiency = IdentificationRate = MissedCleavageRate = -1;
            ChargeRatio3to2 = ChargeRatio4to2 = -1;
            Ms3ScanRate = -1;
            MedianMassDrift = -1;
            SearchData = new SearchData();

            TotalScans = metricsData.TotalScans;
            NumMs1Scans = metricsData.MS1Scans;
            NumMs2Scans = metricsData.MS2Scans;
            NumMs3Scans = metricsData.MS3Scans;
            Ms1ScanRate = metricsData.MS1ScanRate;
            Ms2ScanRate = metricsData.MS2ScanRate;
            MeanDutyCycle = metricsData.MeanDutyCycle;
            MeanTopN = metricsData.MeanTopN;
            MedianPrecursorIntensity = metricsData.MedianPrecursorIntensity;
            MedianSummedMs2Intensity = metricsData.MedianSummedMS2Intensity;
            MedianMs1IsolationInterference = metricsData.MedianMs1IsolationInterference;
            MedianMs2FractionConsumingTop80PercentTotalIntensity = metricsData.MedianMs2FractionConsumingTop80PercentTotalIntensity;
            NumEsiStabilityFlags = metricsData.NumberOfEsiFlags;
            QuantMeta = metricsData.QuantMeta;
            GradientTime = metricsData.Gradient;
            ColumnPeakCapacity = metricsData.PeakCapacity;
            TimeBeforeFirstScanToExceedPoint1MaxIntensity = metricsData.TimeBeforeFirstScanToExceedPoint1MaxIntensity;
            TimeAfterLastScanToExceedPoint1MaxIntensity = metricsData.TimeAfterLastScanToExceedPoint1MaxIntensity;
            FractionOfRunAbovePoint1MaxIntensity = metricsData.FractionOfRunAbovePoint1MaxIntensity;
            PeakShape.Asymmetry.P10 = metricsData.PeakShape.Asymmetry.P10;
            PeakShape.Asymmetry.P50 = metricsData.PeakShape.Asymmetry.P50;
            PeakShape.Width.P10 = metricsData.PeakShape.Width.P10;
            PeakShape.Width.P50 = metricsData.PeakShape.Width.P50;
            MedianSummedMs1Intensity = metricsData.MedianSummedMS1Intensity;
            Ms1FillTimeDistribution = metricsData.Ms1FillTimeDistribution;
            Ms2FillTimeDistribution = metricsData.Ms2FillTimeDistribution;
            Ms3FillTimeDistribution = metricsData.Ms3FillTimeDistribution;
        }
    }

    //[Serializable]
    public class QcDataCollectionDDA
    {
        public SerializableDictionary<DateTime, (RawMetricsDataDDA RawMetrics,SearchMetricsContainer SearchMetrics)> QcData;
        public string DirectoryToWatch, QcDirectory, QcFile;
        //public StringBuilder FileOut;
        public List<string> ProcessedRawFiles;

        public QcDataCollectionDDA()
        {
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, (RawMetricsDataDDA RawMetrics, SearchMetricsContainer SearchMetrics)>();
        }

        public QcDataCollectionDDA(string directoryToWatch, string qcDirectory)
        {
            DirectoryToWatch = directoryToWatch;
            QcDirectory = qcDirectory;
            QcFile = Path.Combine(new string[] { qcDirectory, "QC.xml" });
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, (RawMetricsDataDDA RawMetrics, SearchMetricsContainer SearchMetrics)>();
        }
    }

    //[Serializable]
    public class QcDataCollectionDIA
    {
        public SerializableDictionary<DateTime, RawMetricsDataDIA> QcData;
        public string DirectoryToWatch, QcDirectory, QcFile;
        //public StringBuilder FileOut;
        public List<string> ProcessedRawFiles;

        public QcDataCollectionDIA()
        {
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, RawMetricsDataDIA>();
        }

        public QcDataCollectionDIA(string directoryToWatch, string qcDirectory)
        {
            DirectoryToWatch = directoryToWatch;
            QcDirectory = qcDirectory;
            QcFile = Path.Combine(new string[] { qcDirectory, "QC.xml" });
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, RawMetricsDataDIA>();
        }
    }
}
