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
        //public static QcDataContainer ProcessQcData(this QcDataCollection Data, RawDataCollection rawData, IRawDataPlus rawFile, string qcDirectory, string fastaDB = null)
        public static QcDataContainer ParseQcData(QcWorkflowParameters parameters, MetricsData metricsData, MethodDataContainer methodData)
        {
            DateTime dateAcquired = methodData.CreationDate;

            QcDataContainer qcData = new QcDataContainer(metricsData.RawFileName, dateAcquired, metricsData);

            qcData.Instrument = methodData.Instrument;
            qcData.ExperimentMsOrder = methodData.AnalysisOrder;
            qcData.Ms1Analyzer = methodData.MassAnalyzers[MSOrderType.Ms].ToString();
            qcData.Ms2Analyzer = methodData.MassAnalyzers[MSOrderType.Ms2].ToString();

            if (qcData.ExperimentMsOrder == MSOrderType.Ms3)
            {
                qcData.Ms3Analyzer = methodData.MassAnalyzers[MSOrderType.Ms3].ToString();
            }
            else
            {
                qcData.Ms3Analyzer = "None";
            }

            return qcData;
        }

        public static void DoQc(WorkflowParameters parameters)
        {
            QcDataCollection qcDataCollection;
            string dataDirectory = parameters.RawFileDirectory;
            string qcDirectory = parameters.QcParams.QcDirectory;
            string qcSearchDataDirecotry = parameters.QcParams.QcSearchDataDirectory;
            SearchParameters searchParameters = parameters.QcParams.SearchParameters;
            List<string> fileList = new List<string>();

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

                    Console.WriteLine("{0} appears to already exist in the QC data with the name {1}. Skipping to next file.",
                        fileName, qcDataCollection.QcData[FileHeaderReaderFactory.ReadFile(fileName).CreationDate].RawFile);

                    continue;
                }

                using (IRawDataPlus rawFile = RawFileReaderFactory.ReadFile(fileName))
                {
                    rawFile.SelectInstrument(Device.MS, 1);

                    QcDataContainer newQcData = DataWorkFlows.QcDDA(rawFile, parameters);

                    if (searchParameters != null)
                    {
                        Search.RunSearch(qcParameters, rawData, rawFile);
                        newQcData.ParseSearchResults(rawData, rawFile, qcParameters);
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

        private static (List<string> fileList, QcDataCollection qcDataCollection) GetFileListAndQcFile(WorkflowParameters parameters)
        {
            QcDataCollection qcDataCollection;
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

            return (fileList, qcDataCollection);
        }

        private static bool CheckIfFilePresentInQcCollection(string fileName, QcDataCollection qcDataCollection)
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

        public QcDataContainer(string rawFile, DateTime dateAquired, MetricsData metricsData)
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
