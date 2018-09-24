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
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using RawTools.Data.Collections;
using RawTools.Data.IO;
using RawTools.Utilities;
using RawTools.Data.Containers;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;
using Serilog;

namespace RawTools.QC
{
    static class Identipy
    {
        public static string GetPythonDir()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // the OS is linux
                if (File.Exists("/usr/bin/python2.7"))
                {
                    return "/usr/bin";
                }
                else
                {
                    if (File.Exists("/bin/python2.7"))
                    {
                        return "/bin";
                    }
                    else
                    {
                        Console.WriteLine("Python not automatically detected. Please provide the python installation directory using the -p argument.");
                        Environment.Exit(1);
                        return null;
                    }
                }
            }

            else
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    //The OS is windows NT or later
                    try
                    {
                        return ConsoleUtils.Bash("python", "-c \"import sys; print(sys.exec_prefix)\"").Replace("\r", "").Replace("\n", "");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Python not automatically detected. Please provide the python installation directory using the -p argument.");
                        Environment.Exit(1);
                        return null;
                    }
                }
                else
                {
                    //The OS is, hopefully, MacOS
                    if (File.Exists("/Library/Frameworks/Python.framework/Versions/2.7/bin/python"))
                    {
                        return "/Library/Frameworks/Python.framework/Versions/2.7/bin/";
                    }
                    else
                    {
                        Console.WriteLine("Python not automatically detected. Please provide the python installation directory using the -p argument.");
                        Environment.Exit(1);
                        return null;
                    }
                }
            }
        }

        public static string GetPythonExec(string pythonDir = null)
        {

            pythonDir = GetPythonDir();

            if (Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return Path.Combine(pythonDir, "python");
            }
            else
            {
                return Path.Combine(pythonDir, "python.exe");
            }
        }

        public static string GetPythonVersion(string pythonExec)
        {
            return ConsoleUtils.Bash(pythonExec, "-c \"import sys; print(sys.version.split(' ')[0])\"").Replace("\r", "").Replace("\n", "");
        }

        public static void CheckIdentipyDependencies(SearchParameters idpyPars)
        {
            string pyExec;
            // Check if python is installed
            Console.WriteLine("Checking Identipy dependencies");
            Log.Information("Checking Identipy dependencies");
            //try
            //{
                if (idpyPars.PythonExecutable == null)
                {
                    pyExec = GetPythonExec(GetPythonDir());
                }
                else
                {
                    pyExec = idpyPars.PythonExecutable;
                }
                string pyV = GetPythonVersion(pyExec);

                // check if it is python 2.7
                if (!pyV.StartsWith("2.7"))
                {
                    Log.Error("Python {Version} detected, but Python 2.7 is requried");
                    Console.WriteLine("ERROR: {0} detected, but python 2.7 is requried. If Python 2.7 is installed, please provide " +
                        "the path to the executable using the -p argument.", pyV);
                    Environment.Exit(1);
                }

                // if all is okay, proceed
                Log.Information("Python {Version} detected, using this to run identipy", pyV);

            // check if identipy is installed
            try
            {
                ConsoleUtils.Bash("python", "-c \"import identipy\"");
            }
            catch (Exception e)
            {
                Log.Error(e, "Identipy is not installed");
                Console.WriteLine("ERROR: Identipy is not installed. See identipy installation instructions at https://bitbucket.org/levitsky/identipy/overview");
                Environment.Exit(1);
            }
            Console.WriteLine("All Identipy dependencies satisfied!");
        }

        public static string GetIdentipyExec(string pythonDir)
        {
            string idpyFile;
            if (Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                idpyFile = Path.Combine(pythonDir, "identipy");
            }
            else
            {
                idpyFile = Path.Combine(pythonDir, "Scripts", "identipy-script.py");
            }

            if (File.Exists(idpyFile))
            {
                return idpyFile;
            }
            else
            {
                Console.WriteLine("Identipy start script not found. Is Identipy installed? If so, please provide the path the the " +
                    "identipy start script using the -I argument. See help for more details.");
                Log.Error("Unable to automatically find identipy script");
                Environment.Exit(1);
                return null;
            }
        }

        public static void RunIdentipy(RawDataCollection rawData, IRawDataPlus rawFile, string QcDataDirectory, SearchParameters idpyPars, bool writeMGF = true)
        {
            
            int[] scans;
            string pyExec, idpyScript, mgfFile;

            // get the path to the python executable and identipy start script if they have not been specified
            if (idpyPars.PythonExecutable == null & idpyPars.IdentipyScript == null)
            {
                string pyDir = GetPythonDir();
                pyExec = GetPythonExec(pyDir);
                idpyScript = GetIdentipyExec(pyDir);
                Log.Information("Found python directory: {PyDir}", pyDir);
                Log.Information("Found python executable: {PyExec}", pyExec);
                Log.Information("Found identipy start script: {IdpyScript}", idpyScript);
            }
            else
            {
                pyExec = idpyPars.PythonExecutable;
                idpyScript = idpyPars.IdentipyScript;
            }

            if (writeMGF)
            {
                // get a random subset of scans
                scans = AdditionalMath.SelectRandomScans(scans: rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2], num: idpyPars.NumSpectra);

                // write them to a mgf file
                MGF.WriteMGF(rawData, rawFile, QcDataDirectory, scans: scans, cutoff: idpyPars.MgfMassCutoff, intensityCutoff: idpyPars.MgfIntensityCutoff);
                
            }

            // recreate the path to the mgf
            mgfFile = Path.Combine(QcDataDirectory, Path.GetFileName(rawData.rawFileName) + ".mgf");

            // run identipy, supresseing RunTimeWarnings
            //IdentipyExecute(idpyExec, idpyPars.FastaDatabase, idpyPars.FixedMods, idpyPars.VariableMods, mgfFile);
            Console.WriteLine("Starting Identipy");
            Log.Information("Starting Identipy");
            IdentipyExecute(pyExec, idpyScript, mgfFile, idpyPars, rawData.methodData.MassAnalyzers[MSOrderType.Ms2]);

        }

        public static void IdentipyExecute(string pyExec, string idpyScript, string mgfFile, SearchParameters identipyParameters, MassAnalyzerType Ms2Analyzer)
        {
            string fastaDB = identipyParameters.FastaDatabase;
            string fmods = identipyParameters.FixedMods;
            string vmods = null;
            
            var tempMods = from x in (new string[] { identipyParameters.NMod, identipyParameters.KMod, identipyParameters.XMod })
                    where x != null
                    select x;

            if (tempMods.Count() > 0)
            {
                vmods = tempMods.Aggregate((i, j) => i + "," + j);
            }
            
            string appDir = AppInformation.AssemblyDirectory;
            string N;
            string parameters = String.Empty;
            string idpyConfig;
            //string constants = "-ad -method reverse -prefix DECOY_ -at";
            string constants = "";

            // get the appropriate identipy config file
            if (Ms2Analyzer == MassAnalyzerType.MassAnalyzerFTMS)
            {
                idpyConfig = Path.Combine(appDir, "orbitrap_config.cfg");
            }
            else
            {
                idpyConfig = Path.Combine(appDir, "iontrap_config.cfg");
            }

            // N is number of processors to use. Must be 1 on windows, but otherwise Identipy can decide how many to use
            // I don't really know about on MacOS, so I'm going to leave it as 1 for that one for now
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                N = "0";
            }
            else
            {
                N = "1";
            }

            if (fmods != null & vmods != null)
            {
                parameters = string.Format("-W ignore " + idpyScript + " -cfg {0} {1} -nproc {2} -db {3} -fmods {4} -vmods {5} {6}", idpyConfig, constants, N, fastaDB, fmods, vmods, mgfFile);
            }

            if (fmods == null & vmods != null)
            {
                parameters = string.Format("-W ignore " + idpyScript + " -cfg {0} {1} -nproc {2} -db {3} -vmods {4} {5}", idpyConfig, constants, N, fastaDB, vmods, mgfFile);
            }

            if (fmods != null & vmods == null)
            {
                parameters = string.Format("-W ignore " + idpyScript + " -cfg {0} {1} -nproc {2} -db {3} -fmods {4} {5}", idpyConfig, constants, N, fastaDB, fmods, mgfFile);
            }

            if (fmods == null & vmods == null)
            {
                parameters = string.Format("-W ignore " + idpyScript + " -cfg {0} {1} -nproc {2} -db {3} {4}", idpyConfig, constants, N, fastaDB, mgfFile);
            }

            ConsoleUtils.VoidBash(pyExec, parameters);
        }
    }
}
