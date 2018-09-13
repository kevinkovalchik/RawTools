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
using RawTools;
using RawTools.Utilities;

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

        public static void WriteCustomParametersFile(string directory, XElement parameters, string fileName)
        {
            parameters.Save(Path.Combine(directory, fileName));
        }

        public static void SetInputParameters(SearchParameters parameters)
        {
            XElement test = XElement.Parse(Properties.Resources.XTandem_default_config);
        }

        public static void RunXTandem(string xTandemParameters, SearchParameters parameters)
        {
            ConsoleUtils.VoidBash(parameters.XTandemDirectory, xTandemParameters);
        }


    }
}
