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

namespace RawTools.WorkFlows
{
    class WorkflowParameters
    {
        public ExperimentType ExpType;
        public double MgfIntensityCutoff, MgfMassCutoff;
        public string RawFileDirectory;
        public bool RefineMassCharge;
        public IEnumerable<string> InputFiles;

        public ParseWorkflowParameters ParseParams;
        public QcWorkflowParameters QcParams;

        public WorkflowParameters()
        {
            QcParams = new QcWorkflowParameters();
            ParseParams = new ParseWorkflowParameters();
        }

        public WorkflowParameters(ArgumentParser.ParseOptions parseOptions)
        {
            ParseParams = new ParseWorkflowParameters();

            ExpType = ExperimentType.DDA;
            MgfIntensityCutoff = parseOptions.IntensityCutoff;
            MgfMassCutoff = parseOptions.MassCutOff;
            InputFiles = parseOptions.InputFiles;
            RawFileDirectory = parseOptions.InputDirectory;
            RefineMassCharge = parseOptions.RefineMassCharge;
            ParseParams.Chromatogram = parseOptions.Chromatogram;
            ParseParams.LabelingReagents = String.Empty;
            ParseParams.LabelingReagents = parseOptions.LabelingReagents;
            ParseParams.Metrics = parseOptions.Metrics;
            ParseParams.OutputDirectory = parseOptions.OutputDirectory;
            ParseParams.Parse = parseOptions.ParseData;
            ParseParams.Quant = parseOptions.Quant;
            ParseParams.UnlabeledQuant = parseOptions.UnlabeledQuant;
            ParseParams.WriteMgf = parseOptions.WriteMGF;
        }

        public WorkflowParameters(ArgumentParser.QcOptions qcOptions)
        {
            QcParams = new QcWorkflowParameters();
            Enum.TryParse(qcOptions.SearchAlgorithm, true, out QcParams.SearchAlgorithm);

            ExpType = ExperimentType.DDA;

            MgfIntensityCutoff = qcOptions.IntensityCutoff;
            MgfMassCutoff = qcOptions.MassCutOff;
            RawFileDirectory = qcOptions.DirectoryToQc;
            RefineMassCharge = qcOptions.RefineMassCharge;

            QcParams.FastaDatabase = qcOptions.FastaDatabase;
            QcParams.FixedMods = qcOptions.FixedMods;
            QcParams.FixedScans = qcOptions.FixedScans;
            QcParams.IdentipyScript = qcOptions.IdentipyScript;
            QcParams.KMod = qcOptions.VariableKMod;
            QcParams.NMod = qcOptions.VariableNMod;
            QcParams.XMod = qcOptions.VariableXMod;
            QcParams.NumberSpectra = qcOptions.NumberSpectra;
            QcParams.PythonExecutable = qcOptions.PythonExecutable;
            QcParams.QcDirectory = qcOptions.QcDirectory;
            QcParams.XTandemDirectory = qcOptions.XTandemDirectory;

            if (QcParams.SearchAlgorithm != SearchAlgorithm.None)
            {
                QcParams.PerformSearch = true;
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

        public string QcDirectory, FastaDatabase, FixedMods,
            NMod, KMod, XMod, PythonExecutable, IdentipyScript, XTandemDirectory;

        public string QcSearchDataDirectory { get { return Path.Combine(QcDirectory, "QcSearchData"); } }
    }
}
