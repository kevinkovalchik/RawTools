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
using System.IO;
using RawTools.Data.Containers;

namespace RawTools.WorkFlows
{
    class WorkflowParameters
    {
        public ExperimentType ExpType;
        public double MgfIntensityCutoff, MgfMassCutoff;
        public string RawFileName, RawFileDirectory;
        public bool RefineMassCharge;

        public ParseWorkflowParameters ParseParams;
        public QcWorkflowParameters QcParams;

        public WorkflowParameters(ArgumentParser.ParseOptions parseOptions)
        {
            QcParams = null;
            Enum.TryParse(parseOptions.ExperimentType, out ExpType);

            // place holder
        }

        public WorkflowParameters(ArgumentParser.QcOptions qcOptions)
        {
            ParseParams = null;
            Enum.TryParse(qcOptions.ExperimentType, out ExpType);
            // place holder
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
