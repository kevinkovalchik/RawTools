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
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using RawTools.Data.Extraction;
using RawTools.Data.Processing;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;
using System.Xml.Serialization;


namespace RawTools.Utilities
{
    static class ConsoleUtils
    {
        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        public static void VoidBash(string cmd, string args)
        {

            // run a string as a process, return void
            // thanks to https://loune.net/2017/06/running-shell-bash-commands-in-net-core/ for this code.

            var escapedArgs = args.Replace("\"", "\\\"");
            Process process;
            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };

            //string result = string.Empty;
            /*
            process.Start();
            using (StreamReader reader = process.StandardOutput)
            {
                process.WaitForExit();
                string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                return result;
            }
            */
            process.Start();
            //string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return;
        }

        public static string Bash(string cmd, string args)
        {
            // run a string as a process
            // thanks to https://loune.net/2017/06/running-shell-bash-commands-in-net-core/ for this code.

            var escapedArgs = args.Replace("\"", "\\\"");
            Process process;
            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            
            //string result = string.Empty;
            /*
            process.Start();
            using (StreamReader reader = process.StandardOutput)
            {
                process.WaitForExit();
                string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                return result;
            }
            */
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }

    static class EM
    {
        public static int[] FindAllIndex1<T>(this T[] array, Predicate<T> match)
        {
            return array.Select((value, index) => match(value) ? index : -1)
                    .Where(index => index != -1).ToArray();
        }

        public static int[] FindAllIndex2<T>(this T[] a, Predicate<T> match)
        {
            T[] subArray = Array.FindAll<T>(a, match);
            return (from T item in subArray select Array.IndexOf(a, item)).ToArray();
        }
    }

    class ProgressIndicator
    {
        string message;
        int x = 0;
        int total_x;
        int writeNow;

        public ProgressIndicator(int total, string message)
        {
            total_x = total;
            if (total_x > 100)
            {
                writeNow = total_x / 100;
            }
            else
            {
                writeNow = total_x;
            }
            this.message = message;
        }

        public void Start()
        {
            Console.Write("{0}: 0%", message);
        }

        public void Update()
        {
            if (x % writeNow == 0)
            {
                ConsoleUtils.ClearLastLine();
                Console.Write("{0}: {1}%", message, ((x * 100) / total_x));
            }
            x += 1;
        }

        public void Done()
        {
            ConsoleUtils.ClearLastLine();
            Console.WriteLine("{0}: 100%", message);
        }
    }

    static class CheckIfDone
    {
        public static void Check(this RawDataCollection rawData, IRawDataPlus rawFile, List<Operations> operations)
        {
            if(operations.Contains(Operations.MethodData))
            {
                if(!rawData.Performed.Contains(Operations.MethodData))
                {
                    rawData.ExtractMethodData(rawFile);
                }
            }

            if (operations.Contains(Operations.PrecursorMasses))
            {
                if (!rawData.Performed.Contains(Operations.PrecursorMasses))
                {
                    rawData.ExtractPrecursorMasses(rawFile);
                }
            }

            if (operations.Contains(Operations.PrecursorScans))
            {
                if (!rawData.Performed.Contains(Operations.PrecursorScans))
                {
                    rawData.ExtractPrecursorScans(rawFile);
                }
            }

            if (operations.Contains(Operations.RetentionTimes))
            {
                if (!rawData.Performed.Contains(Operations.RetentionTimes))
                {
                    rawData.ExtractRetentionTimes(rawFile);
                }
            }

            if (operations.Contains(Operations.ScanIndex))
            {
                if (!rawData.Performed.Contains(Operations.ScanIndex))
                {
                    rawData.ExtractScanIndex(rawFile);
                }
            }

            if (operations.Contains(Operations.TrailerExtras))
            {
                if (!rawData.Performed.Contains(Operations.TrailerExtras))
                {
                    rawData.ExtractTrailerExtra(rawFile);
                }
            }

            if (operations.Contains(Operations.Ms1CentroidStreams))
            {
                if (!rawData.Performed.Contains(Operations.Ms1CentroidStreams))
                {
                    rawData.ExtractCentroidStreams(rawFile, MSOrder: MSOrderType.Ms);
                }
            }

            if (operations.Contains(Operations.Ms2CentroidStreams))
            {
                if (!rawData.Performed.Contains(Operations.Ms2CentroidStreams))
                {
                    rawData.ExtractCentroidStreams(rawFile, MSOrder: MSOrderType.Ms2);
                }
            }

            if (operations.Contains(Operations.Ms3CentroidStreams))
            {
                if (!rawData.Performed.Contains(Operations.Ms3CentroidStreams))
                {
                    rawData.ExtractCentroidStreams(rawFile, MSOrder: MSOrderType.Ms3);
                }
            }

            if (operations.Contains(Operations.Ms1SegmentedScans))
            {
                if (!rawData.Performed.Contains(Operations.Ms1SegmentedScans))
                {
                    rawData.ExtractSegmentScans(rawFile, MSOrder: MSOrderType.Ms);
                }
            }

            if (operations.Contains(Operations.Ms2SegmentedScans))
            {
                if (!rawData.Performed.Contains(Operations.Ms2SegmentedScans))
                {
                    rawData.ExtractSegmentScans(rawFile, MSOrder: MSOrderType.Ms2);
                }
            }

            if (operations.Contains(Operations.Ms3SegmentedScans))
            {
                if (!rawData.Performed.Contains(Operations.Ms3SegmentedScans))
                {
                    rawData.ExtractSegmentScans(rawFile, MSOrder: MSOrderType.Ms3);
                }
            }

            if (operations.Contains(Operations.MetaData))
            {
                if (!rawData.Performed.Contains(Operations.MetaData))
                {
                    rawData.metaData.AggregateMetaData(rawData, rawFile);
                }
            }

            if (operations.Contains(Operations.PeakRetAndInt))
            {
                if (!rawData.Performed.Contains(Operations.PeakRetAndInt))
                {
                    AnalyzePeaks.CalcPeakRetTimesAndInts(rawData, rawFile);
                }
            }

            if (operations.Contains(Operations.PeakShape))
            {
                if (!rawData.Performed.Contains(Operations.PeakShape))
                {
                    AnalyzePeaks.CalculatePeakShapes(rawData, rawFile);
                }
            }

            if (operations.Contains(Operations.PeakArea))
            {
                if (!rawData.Performed.Contains(Operations.PeakArea))
                {
                    AnalyzePeaks.QuantifyPrecursorPeaks(rawData, rawFile);
                }
            }
        }
        
    }

    static class AdditionalMath
    {
        public static double Percentile(this double[] Values, int percentile)
        {
            int end = Values.Length - 1;
            double endAsDouble = Convert.ToDouble(end);
            double[] sortedValues = (double[])Values.Clone();
            Array.Sort(sortedValues);

            if ((endAsDouble * percentile / 100) % 1 == 0)
            {
                return sortedValues[end * percentile / 100];
            }
            else
            {
                return (sortedValues[end * percentile / 100] + sortedValues[end * percentile / 100 + 1]) / 2;
            }
        }

        public static int[] SelectRandomScans(int[] scans, int num, bool fixedScans = false)
        {
            // convert scans to a list so we can use more methods on it
            List<int> scansIn = scans.ToList();
            int[] scansOut;
            Random numGen;
            if (fixedScans)
            {
                numGen = new Random(1);
            }
            else
            {
                numGen = new Random();
            }
            int newScan = 0;

            if (scans.Count() > num)
            {
                scansOut = new int[num];
                for (int x = 0; x < num; x++)
                {
                    // get a scan number from the pseudo-random number generator
                    while (!scansIn.Contains(newScan))
                    {
                        newScan = numGen.Next(scansIn.Min(), scansIn.Max());
                    }                    
                    // add it to the scansOut array
                    scansOut[x] = newScan;
                    // delete it from scansIn
                    scansIn.Remove(newScan);                    
                }
            }
            else
            {
                scansOut = scans;
            }

            Array.Sort(scansOut);
            return scansOut;
        }

        public static double FractionOfScansConsumingTotalIntensity(this double[] Values, int percent)
        {
            double summed = Values.Sum();
            var valuesOut = (double[])Values.Clone();
            Array.Sort(valuesOut);

            double total = 0;

            for (int i = 0; i < valuesOut.Length; i++)
            {
                int j = valuesOut.Length - 1 - i;
                total += valuesOut[j];
                if (total > summed * percent / 100)
                {
                    return i / (double)valuesOut.Length;
                }
            }

            return 1;

        }

        public static double[] SliceArray(this double[] Values, int first, int last)
        {
            double[] slice = new double[last - first];

            int j = 0;
            for (int i = first; i < last; i++)
            {
                slice[j++] = Values.ElementAt(i);
            }
            return slice;
        }

        public static double Mean(this double[] Values)
        {
            int num = Values.Length;
            double sum = Values.Sum();

            return sum / num;
        }

        public static double MovingAverage(this double[] Values, int index, int window)
        {
            double[] valueArray;

            if (index >= 0 & index < window/2)
            {
                valueArray = Values.SliceArray(0, window);
                return valueArray.Mean();
            }
            if (index > window/2 & index < Values.Length - (window - window/2))
            {
                valueArray = Values.SliceArray(index - window/2, index + (window - window/2));
                return valueArray.Mean();
            }
            if (index > Values.Length - (window - window / 2) & index < Values.Length)
            {
                valueArray = Values.SliceArray(Values.Length - window, Values.Length);
                return valueArray.Mean();
            }
            else
            {
                return 0;
            }
        }

        public static (double[], double[]) SubsetMsData(double[] masses, double[] intensities, double lowMass, double hiMass)
        {
            List<double> massesOut = new List<double>();
            List<double> intensitiesOut = new List<double>();

            for (int i = 0; i < masses.Length; i++)
            {
                if (masses[i] > lowMass & masses[i] < hiMass)
                {
                    massesOut.Add(masses[i]);
                    intensitiesOut.Add(intensities[i]);
                }
            }

            return (massesOut.ToArray(), intensitiesOut.ToArray());
        }

        /// <summary>
        /// Returns the factorial of the input as a double
        /// </summary>
        /// <param name="value">The number of interest</param>
        public static double Factorial(int value)
        {
            double valueOut = 1;
            for (int i = value; i > 0; i--)
            {
                valueOut *= i;
            }

            return valueOut;
        }
    }

    static class ReadWrite
    {
        public static string GetPathToFile(string outputDirectory, string fileName, string suffix)
        {
            string newFileName = string.Empty;
            
            if (outputDirectory != null)
            {
                // check if the output directory is rooted
                if (!Path.IsPathRooted(outputDirectory))
                {
                    // if it isn't then add it to the path root of the raw file
                    outputDirectory = Path.Combine(Path.GetDirectoryName(fileName), outputDirectory);
                }

                newFileName = Path.Combine(outputDirectory, Path.GetFileName(fileName) + suffix);

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
            }
            else
            {
                newFileName = fileName + suffix;
            }

            return newFileName;
        }

        public static bool IsFileLocked(string fileName)
        {
            FileStream stream = null;

            try
            {
                FileInfo file = new FileInfo(fileName);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void AwaitFileAccessibility(string fileName)
        {
            //Your File
            var fileInfo = new FileInfo(fileName);

            //While File is not accesable because of writing process
            while (IsFileLocked(fileName)) { }

            //File is available here
        }

        public static void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }

    /// <summary>
    /// Functions for performing common binary Serialization operations.
    /// <para>All properties and variables will be serialized.</para>
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    public static class BinarySerialization
    {
        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }

    /// <summary>
    /// Functions for performing common XML Serialization operations.
    /// <para>Only public properties and variables will be serialized.</para>
    /// <para>Use the [XmlIgnore] attribute to prevent a property/variable from being serialized.</para>
    /// <para>Object to be serialized must have a parameterless constructor.</para>
    /// </summary>
    public static class XmlSerialization
    {
        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }

    public static class AppInformation
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }    
}