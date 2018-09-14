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
        public static bool CheckForParametersFile(string XTandemDirectory)
        {
            return File.Exists(Path.Combine(XTandemDirectory, "input.xml"));
        }

        public static bool CheckForDefaultParametersFile(string XTandemDirectory)
        {
            return File.Exists(Path.Combine(XTandemDirectory, "default_input.xml"));
        }

        public static void WriteDefaultParametersFile(string XTandemDirectory)
        {
            XElement.Parse(Properties.Resources.XTandem_default_config).Save(Path.Combine(XTandemDirectory, "default_input.xml"));
        }

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

            // add the other modifications
            foreach (string modification in new List<string>() { parameters.NMod, parameters.KMod, parameters.XMod })
            {
                customParameters.AddNoteToXTandemParameters(type: "input", label: "residue, potential modification mass", value: parameters.FixedMods);
            }

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

        public static void WriteCustomParametersFile(string directory, XElement parameters, string fileName)
        {
            parameters.Save(Path.Combine(directory, fileName));
        }

        public static void RunXTandem(this RawDataCollection rawData, string xTandemParameters, SearchParameters searchParameters, string mgfFile, string outputFile)
        {
            XElement customPars, taxonomy;

            // write out the default input file
            XElement.Parse(Properties.Resources.XTandem_default_config).Save(Path.Combine(searchParameters.XTandemDirectory, "XTandem_default_config.xml"));

            // set up the taxonomy file
            taxonomy = XElement.Parse(Properties.Resources.XTandem_taxonomy);
            taxonomy.UpdateTaxonomy(searchParameters);

            // set up the custom input file
            customPars = XElement.Parse(Properties.Resources.XTandem_custom_config);
            customPars.UpdateCustomXParameters(searchParameters, rawData, mgfFile, outputFile);

            ConsoleUtils.VoidBash(Path.Combine(searchParameters.XTandemDirectory, "tandem.exe"), xTandemParameters);
        }
    }
}
