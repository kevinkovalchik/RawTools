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

        public WorkflowParameters(ArgumentParser.Options Options)
        {

            ParseParams = new ParseWorkflowParameters();
            
            ExpType = ExperimentType.DDA;
            MgfIntensityCutoff = 0;
            MaxProcesses = Options.MaxProcesses;
            MgfMassCutoff = Options.MgfMassCutOff;
            InputFiles = Options.RawFiles;
            RawFileDirectory = Options.RawFileDirectory;
            IncludeSubdirectories = Options.SearchSubdirectories;
            RefineMassCharge = Options.RefineMassCharge;
            ParseParams.Chromatogram = Options.Chromatogram;
            ParseParams.Xic = Options.Xic;
            ParseParams.WriteMgfLevels = Options.WriteMgfLevels;
            ParseParams.WriteFaimsMgf = Options.FaimsMgf;
            ParseParams.LabelingReagents = Options.LabelingReagent;
            ParseParams.Metrics = Options.Metrics;
            ParseParams.AllScanData = Options.AllScanData;
            ParseParams.Parse = Options.Parse;
            ParseParams.Quant = Options.Quant;
            ParseParams.UnlabeledQuant = Options.UnlabeledQuant;
            ParseParams.WriteMgf = Options.WriteMGF;
            ConsideredChargeStates.Min = Options.MinCharge;
            ConsideredChargeStates.Max = Options.MaxCharge;
            LogDump = Options.LogDump;
            ImpurityTable = Options.TmtImpurityTable;

            string output = Options.OutputDirectory;

            if (!String.IsNullOrEmpty(output))
            {
                if (!Path.IsPathRooted(output))
                {
                    output = Path.Combine(Directory.GetCurrentDirectory(), output);
                }
            }

            ParseParams.OutputDirectory = output;

            QcParams = new QcWorkflowParameters();

            QcParams.QcDirectory = Options.QcDirectory;
            QcParams.XTandemDirectory = Options.XTandemDirectory;

            if (!String.IsNullOrEmpty(QcParams.XTandemDirectory))
            {
                QcParams.SearchAlgorithm = SearchAlgorithm.XTandem;
            }

            ExpType = ExperimentType.DDA;
            
            QcParams.FastaDatabase = Options.FastaDB;
            QcParams.FixedMods = Options.FixedModifications;
            //QcParams.FixedScans = Options.FixedScans;
            QcParams.VariableMods = Options.VariableModifications;

            QcParams.NumberSpectra = Options.NumberSpectra;

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
