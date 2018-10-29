using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using RawTools.QC;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using RawTools.Utilities;

namespace RawTools.Algorithms.MatchBewteen
{
    static class AlignTimeAndMass
    {
        public static Dictionary<double, int> MasterReverseRetentionTimeLookup(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans)
        {
            Dictionary<double, int> reverse = new Dictionary<double, int>();

            foreach (var key in retentionTimes.Keys)
            {
                reverse.Add(retentionTimes[key], precursorScans[key].MasterScan);
            }

            return reverse;
        }

        public static Dictionary<double, int> ReverseRetentionTimeLookup(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans)
        {
            Dictionary<double, int> reverse = new Dictionary<double, int>();

            foreach (var key in retentionTimes.Keys)
            {
                reverse.Add(retentionTimes[key], key);
            }

            return reverse;
        }

        public static PsmDataCollection LoadPsmData(string pepXML, RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans)
        {
            Dictionary<double, int> reverseTimeLookup = ReverseRetentionTimeLookup(retentionTimes, precursorScans);

            XElement results = XElement.Load(pepXML);

            PsmDataCollection psms = new PsmDataCollection();
            PsmData psm;
            List<PsmData> psmList = new List<PsmData>();

            foreach (var x in results.Descendants("group").Where(x => x?.Element("protein") != null))
            {
                psm = new PsmData();
                
                psm.Decoy = x.Attribute("label").Value.StartsWith("DECOY_");

                // it is possible for each "group" in the pepXML file to have more than one protein. This just means the peptide isn't
                // unique to a single protein. However, the scoring and modifications are identical (since it is the same PSM), so we
                // can just use the first protein. That is what we do below.
                XElement domain = x.Element("protein").Element("peptide").Element("domain");
                
                psm.Hyperscore = Convert.ToDouble(domain.Attribute("hyperscore").Value);

                psm.ExpectationValue = Convert.ToDouble(domain.Attribute("expect").Value);

                psm.Charge = Convert.ToInt32(x.Attribute("z").Value);

                double measuredMZ = (Convert.ToDouble(x.Attribute("mh")?.Value) + psm.Charge) / psm.Charge;
                double matchedMZ = (Convert.ToDouble(domain?.Attribute("mh").Value) + psm.Charge) / psm.Charge;

                psm.MassDrift = (measuredMZ - matchedMZ) / matchedMZ * 1e6;

                psm.mz = measuredMZ;

                psm.RetentionTime = Convert.ToDouble(x.Attribute("rt").Value);

                psm.Scan = reverseTimeLookup[psm.RetentionTime];

                psm.MasterScan = precursorScans[psm.Scan].MasterScan;

                psm.MasterScanRetentionTime = retentionTimes[psm.MasterScan];

                psm.Seq = x.Element("protein").Element("peptide").Element("domain").Attribute("seq").Value;

                psmList.Add(psm);
            }

            double topDecoyScore = (from x in psmList where x.Decoy select x.Hyperscore).ToArray().Percentile(95);

            IEnumerable<PsmData> goodPsms = from x in psmList where !x.Decoy & x.Hyperscore > topDecoyScore select x;

            foreach (var x in goodPsms)
            {
                psms.Add(x.Scan, x);
            }

            return psms;
        }
    }
}
