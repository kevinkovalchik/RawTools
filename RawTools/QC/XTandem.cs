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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools;
using RawTools.Utilities;
using ThermoFisher.CommonCore.Data.FilterEnums;

namespace RawTools.QC
{
    static class XTandem
    {
        static void AddNoteToXTandemParameters(this XElement parameters, string type, string label, string value)
        {
            if (value != null)
            {
                XElement note = new XElement("note");
                note.SetAttributeValue("type", type);
                note.SetAttributeValue("label", label);
                note.SetValue(value);
                parameters.Element("bioml").Add(note);
            }
        }

        public static void UpdateCustomXParameters(this XElement customParameters, SearchParameters parameters, RawDataCollection rawData, string mgfFile, string outputFile)
        {
            // add fixed modifications
            customParameters.AddNoteToXTandemParameters(type: "input", label: "residue, modification mass", value: parameters.FixedMods);

            // add the variable modifications
            var tempMods = from x in (new string[] { parameters.NMod, parameters.KMod, parameters.XMod })
                           where x != null
                           select x;

            string vmods = tempMods.Aggregate((i, j) => i + "," + j);
            customParameters.AddNoteToXTandemParameters(type: "input", label: "residue, potential modification mass", value: vmods);

            // add the parent and fragment mass errors
            // we assume the parent scan is in the FTMS
            customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, parent monoisotopic mass error plus", value: "10");
            customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, parent monoisotopic mass error minus", value: "10");
            customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, parent monoisotopic mass isotope error", value: "yes");
            customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, parent monoisotopic mass error units", value: "ppm");

            // need to check where the fragment scan is happening to assign mass error
            if (rawData.methodData.MassAnalyzers[MSOrderType.Ms2] == MassAnalyzerType.MassAnalyzerFTMS)
            {
                customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, fragment monoisotopic mass error", value: "0.5");
            }
            else
            {
                customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, fragment monoisotopic mass error", value: "0.05");
            }
            customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, fragment monoisotopic mass error units", value: "Daltons");

            // add default parameter file
            customParameters.AddNoteToXTandemParameters(type: "input", label: "list path, default parameters",
                value: Path.Combine(parameters.XTandemDirectory, "XTandem_default_config.xml"));

            // add taxonomy file
            customParameters.AddNoteToXTandemParameters(type: "input", label: "list path, taxonomy information",
                value: Path.Combine(parameters.XTandemDirectory, "XTandem_taxonomy.xml"));

            // add input and output
            customParameters.AddNoteToXTandemParameters(type: "input", label: "spectrum, path", value: mgfFile);
            customParameters.AddNoteToXTandemParameters(type: "input", label: "output, path", value: outputFile);
        }

        public static void UpdateTaxonomy(this XElement taxonomy, SearchParameters parameters)
        {
            XElement element = new XElement("file");
            element.SetAttributeValue("format", "peptide");
            element.SetAttributeValue("URL", parameters.FastaDatabase);
            taxonomy.Element("bioml").Element("taxon").Add(element);
        }

        public static void RunXTandem(this RawDataCollection rawData, SearchParameters searchParameters, string mgfFile, string outputFile)
        {
            XElement customPars, taxonomy;

            // write out the default input file
            XElement.Parse(Properties.Resources.XTandem_default_config).Save(Path.Combine(searchParameters.XTandemDirectory, "XTandem_default_config.xml"));

            // set up and write the taxonomy file
            taxonomy = XElement.Parse(Properties.Resources.XTandem_taxonomy);
            taxonomy.UpdateTaxonomy(searchParameters);
            taxonomy.Save(Path.Combine(searchParameters.XTandemDirectory, "XTandem_taxonomy.xml"));

            // set up and write the custom input file
            customPars = XElement.Parse(Properties.Resources.XTandem_custom_config);
            customPars.UpdateCustomXParameters(searchParameters, rawData, mgfFile, outputFile);
            string xTandemParameters = Path.Combine(searchParameters.XTandemDirectory, "XTandem_custom_config.xml");
            customPars.Save(xTandemParameters);            

            ConsoleUtils.VoidBash(Path.Combine(searchParameters.XTandemDirectory, "tandem.exe"), xTandemParameters);
        }
    }
}
