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
        public string RawFileDirectory;
        public bool IncludeSubdirectories, RefineMassCharge, LogDump;
        public (int Min, int Max) ConsideredChargeStates;
        public IEnumerable<string> InputFiles;

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

            MgfMassCutoff = (double)Options["MgfMassCutOff"];
            InputFiles = (List<string>)Options["RawFiles"];
            RawFileDirectory = (string)Options["RawFileDirectory"];
            IncludeSubdirectories = (bool)Options["SearchSubdirectories"];
            RefineMassCharge = (bool)Options["RefineMassCharge"];
            ParseParams.Chromatogram = (string)Options["Chromatogram"];
            ParseParams.LabelingReagents = (string)Options["LabelingReagents"];
            ParseParams.Metrics = (bool)Options["Metrics"];
            ParseParams.Parse = (bool)Options["Parse"];
            ParseParams.Quant = (bool)Options["Quant"];
            ParseParams.UnlabeledQuant = (bool)Options["UnlabeledQuant"];
            ParseParams.WriteMgf = (bool)Options["WriteMGF"];
            ConsideredChargeStates.Min = (int)Options["MinCharge"];
            ConsideredChargeStates.Max = (int)Options["MaxCharge"];
            LogDump = (bool)Options["LogDump"];

            string output = (string)Options["OutputDirectory"];

            if (!String.IsNullOrEmpty(output))
            {
                if (!Path.IsPathRooted(output))
                {
                    output = Path.Combine(Directory.GetCurrentDirectory(), output);
                }
            }

            ParseParams.OutputDirectory = output;

            QcParams = new QcWorkflowParameters();

            QcParams.QcDirectory = (string)Options["QcDirectory"];
            QcParams.XTandemDirectory = (string)Options["XTandemDirectory"];

            if (!String.IsNullOrEmpty(QcParams.XTandemDirectory))
            {
                QcParams.SearchAlgorithm = SearchAlgorithm.XTandem;
            }

            ExpType = ExperimentType.DDA;
            
            QcParams.FastaDatabase = (string)Options["FastaDatabase"];
            QcParams.FixedMods = (string)Options["FixedMods"];
            //QcParams.FixedScans = Options.FixedScans;
            QcParams.VariableMods = (string)Options["VariableKMods"];
            QcParams.FreqMods = (string)Options["XKMods"];

            QcParams.NumberSpectra = (int)Options["NumberSpectra"];

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
    }

    public class ParseWorkflowParameters
    {
        public bool Parse, Quant, UnlabeledQuant, WriteMgf, Metrics;
        public string LabelingReagents, OutputDirectory, Chromatogram;
    }

    public class QcWorkflowParameters
    {
        public bool PerformSearch, FixedScans;

        public int NumberSpectra;

        public SearchParameters SearchParameters;

        public SearchAlgorithm SearchAlgorithm;

        public string QcDirectory, FastaDatabase, FixedMods, VariableMods, FreqMods,
            NMod, KMod, XMod, PythonExecutable, IdentipyScript, XTandemDirectory;

        public string QcSearchDataDirectory { get { return Path.Combine(QcDirectory, "QcSearchData"); } }
    }
}
