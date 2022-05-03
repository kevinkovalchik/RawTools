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
using System.IO;
using RawTools.Data.Containers;
using RawTools.Utilities;

namespace RawTools.WorkFlows
{
    class WorkflowParameters
    {
        public ExperimentType ExpType;
        public double MgfIntensityCutoff, MgfMassCutoff;
        public string RawFileDirectory, ImpurityTable;
        public bool IncludeSubdirectories, RefineMassCharge, LogDump;
        public (int Min, int Max) ConsideredChargeStates;
        public IEnumerable<string> InputFiles;
        public int MaxProcesses;

        public ParseWorkflowParameters ParseParams;
        public QcWorkflowParameters QcParams;

        public WorkflowParameters()
        {
            QcParams = new QcWorkflowParameters();
            ParseParams = new ParseWorkflowParameters();
        }

        public WorkflowParameters(Dictionary<string, object> Options)
        {

            ParseParams = new ParseWorkflowParameters();
            
            ExpType = ExperimentType.DDA;
            MgfIntensityCutoff = 0;
            MaxProcesses = Convert.ToInt32(TryGetElseDefault(Options, "MaxProcesses"));
            MgfMassCutoff = Convert.ToDouble(TryGetElseDefault(Options, "MgfMassCutOff"));
            InputFiles = (List<string>)TryGetElseDefault(Options,"RawFiles");
            RawFileDirectory = (string)TryGetElseDefault(Options,"RawFileDirectory");
            IncludeSubdirectories = (bool)TryGetElseDefault(Options,"SearchSubdirectories");
            RefineMassCharge = (bool)TryGetElseDefault(Options,"RefineMassCharge");
            ParseParams.Chromatogram = (string)TryGetElseDefault(Options, "Chromatogram");
            ParseParams.Xic = (string)TryGetElseDefault(Options, "Xic");
            ParseParams.WriteMgfLevels = (string)TryGetElseDefault(Options, "WriteMgfLevels");
            ParseParams.WriteFaimsMgf = (bool)TryGetElseDefault(Options, "FaimsMgf");
            ParseParams.LabelingReagents = (string)TryGetElseDefault(Options, "LabelingReagent");
            ParseParams.Metrics = (bool)TryGetElseDefault(Options, "Metrics");
            ParseParams.AllScanData = (bool)TryGetElseDefault(Options, "AllScanData");
            ParseParams.Parse = (bool)TryGetElseDefault(Options, "Parse");
            ParseParams.Quant = (bool)TryGetElseDefault(Options, "Quant");
            ParseParams.UnlabeledQuant = (bool)TryGetElseDefault(Options, "UnlabeledQuant");
            ParseParams.WriteMgf = (bool)TryGetElseDefault(Options, "WriteMGF");
            ConsideredChargeStates.Min = Convert.ToInt32(TryGetElseDefault(Options, "MinCharge"));
            ConsideredChargeStates.Max = Convert.ToInt32(TryGetElseDefault(Options, "MaxCharge"));
            LogDump = (bool)TryGetElseDefault(Options, "LogDump");
            ImpurityTable = (string)TryGetElseDefault(Options, "TmtImpurityTable");

            string output = (string)TryGetElseDefault(Options, "OutputDirectory");

            if (!String.IsNullOrEmpty(output))
            {
                if (!Path.IsPathRooted(output))
                {
                    output = Path.Combine(Directory.GetCurrentDirectory(), output);
                }
            }

            ParseParams.OutputDirectory = output;

            QcParams = new QcWorkflowParameters();

            QcParams.QcDirectory = (string)TryGetElseDefault(Options, "QcDirectory");
            QcParams.XTandemDirectory = (string)TryGetElseDefault(Options, "XTandemDirectory");

            if (!String.IsNullOrEmpty(QcParams.XTandemDirectory))
            {
                QcParams.SearchAlgorithm = SearchAlgorithm.XTandem;
            }

            ExpType = ExperimentType.DDA;
            
            QcParams.FastaDatabase = (string)TryGetElseDefault(Options, "FastaDB");
            QcParams.FixedMods = (string)TryGetElseDefault(Options, "FixedModifications");
            //QcParams.FixedScans = Options.FixedScans;
            QcParams.VariableMods = (string)TryGetElseDefault(Options, "VariableModifications");

            QcParams.NumberSpectra = Convert.ToInt32(TryGetElseDefault(Options, "NumberSpectra"));

            if (QcParams.SearchAlgorithm != SearchAlgorithm.None)
            {
                QcParams.PerformSearch = true;
            }

            if (QcParams.FastaDatabase != null)
            {
                if (!Path.IsPathRooted(QcParams.FastaDatabase))
                {
                    QcParams.FastaDatabase = Path.Combine(Directory.GetCurrentDirectory(), QcParams.FastaDatabase);
                }
            }

            if (QcParams.QcDirectory != null)
            {
                if (!Path.IsPathRooted(QcParams.QcDirectory))
                {
                    QcParams.QcDirectory = Path.Combine(Directory.GetCurrentDirectory(), QcParams.QcDirectory);
                }
            }

            if (QcParams.XTandemDirectory != null)
            {
                if (!Path.IsPathRooted(QcParams.XTandemDirectory))
                {
                    QcParams.XTandemDirectory = Path.Combine(Directory.GetCurrentDirectory(), QcParams.XTandemDirectory);
                }
            }
        }

        private V TryGetElseDefault<T, V>(Dictionary<T, V> parameters, T key)
        {
            if (parameters.ContainsKey(key))
            {
                return parameters[key];
            }
            else
            {
                return default(V);
            }
        }
    }

    public class ParseWorkflowParameters
    {
        public bool Parse, Quant, UnlabeledQuant, WriteMgf, WriteFaimsMgf, Metrics, AllScanData;
        public string LabelingReagents, OutputDirectory, Chromatogram, WriteMgfLevels, Xic;
    }

    public class QcWorkflowParameters
    {
        public bool PerformSearch, FixedScans;

        public int NumberSpectra;

        public SearchParameters SearchParameters;

        public SearchAlgorithm SearchAlgorithm;

        public string QcDirectory, FastaDatabase, FixedMods, VariableMods, XTandemDirectory;

        public string QcSearchDataDirectory { get { return Path.Combine(QcDirectory, "QcSearchData"); } }
    }
}
