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
        public static void UpdateQcCollection(QcDataCollection qcDataCollection, QcDataContainer newQcData, MethodDataContainer methodData, string rawFileName)
        {
            qcDataCollection.QcData.Add(methodData.CreationDate, newQcData);
            qcDataCollection.ProcessedRawFiles.Add(Path.GetFileName(rawFileName));
            qcDataCollection.WriteQcToTable();
            Console.WriteLine("QC data written to csv file.");
            
            try
            {
                XmlSerialization.WriteToXmlFile<QcDataCollection>(qcDataCollection.QcFile, qcDataCollection);
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

        public static QcDataCollection LoadOrCreateQcCollection(WorkflowParameters parameters)
        {
            QcDataCollection qcDataCollection;
            string qcFile = Path.Combine(parameters.QcParams.QcDirectory, "QC.xml");

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
                if (!Directory.Exists(parameters.QcParams.QcDirectory))
                {
                    Directory.CreateDirectory(parameters.QcParams.QcDirectory);
                }

                qcDataCollection = new QcDataCollection(parameters.RawFileDirectory, parameters.QcParams.QcDirectory, parameters.ExpType);
                Log.Information("Appears to be a new QC directory. New QC data collection created.");
            }
            return qcDataCollection;
        }
        
        public static (List<string> fileList, QcDataCollection qcDataCollection) GetFileListAndQcFile(WorkflowParameters parameters, bool subdirectoriesIncluded)
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

                qcDataCollection = new QcDataCollection(dataDirectory, qcDirectory, parameters.ExpType);
                Log.Information("Appears to be a new QC directory. New QC data collection created.");
            }

            // get our list of new raw files. it is every raw file in the directory that is not listed in the qc data
            List<string> fileList;
            if (subdirectoriesIncluded)
            {
                fileList = Directory.GetFiles(dataDirectory, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                fileList = Directory.GetFiles(dataDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();
            }
            

            // make sure we've got absolute paths to the files
            fileList.EnsureAbsolutePaths();

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

        public static bool CheckIfFilePresentInQcCollection(string fileName, QcDataCollection qcDataCollection)
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

    [Serializable]
    public class QcDataContainer
    {
        public RawMetricsDataDDA DDA;
        public RawMetricsDataDIA DIA;
        public SearchMetricsContainer SearchMetrics;

        public QcDataContainer()
        { }
    }


    [Serializable]
    public class QcDataCollection
    {
        public SerializableDictionary<DateTime, QcDataContainer> QcData;
        public string DirectoryToWatch, QcDirectory, QcFile;
        //public StringBuilder FileOut;
        public List<string> ProcessedRawFiles;
        public ExperimentType DataType;

        public QcDataCollection()
        {
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, QcDataContainer>();
        }

        public QcDataCollection(string directoryToWatch, string qcDirectory, ExperimentType experimentType)
        {
            DirectoryToWatch = directoryToWatch;
            QcDirectory = qcDirectory;
            DataType = experimentType;
            QcFile = Path.Combine(new string[] { qcDirectory, "QC.xml" });
            ProcessedRawFiles = new List<string>();
            QcData = new SerializableDictionary<DateTime, QcDataContainer>();
        }
    }
}
