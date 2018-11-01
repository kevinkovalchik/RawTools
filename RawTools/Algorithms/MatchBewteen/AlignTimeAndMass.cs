using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using RawTools.QC;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.WorkFlows;

namespace RawTools.Algorithms.MatchBewteen
{
    static class AlignTimeAndMass
    {
        public static Dictionary<double, int> MasterReverseRetentionTimeLookup(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans)
        {
            Dictionary<double, int> reverse = new Dictionary<double, int>();

            foreach (var key in retentionTimes.Keys)
            {
                reverse.Add(Math.Round(retentionTimes[key], 4), precursorScans[key].MasterScan);
            }

            return reverse;
        }

        public static Dictionary<double, int> ReverseRetentionTimeLookup(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans)
        {
            Dictionary<double, int> reverse = new Dictionary<double, int>();

            foreach (var key in retentionTimes.Keys)
            {
                reverse.Add(Math.Round(retentionTimes[key], 4), key);
            }

            return reverse;
        }

        public static PsmDataCollection LoadPsmData(RetentionTimeCollection retentionTimes, PrecursorScanCollection precursorScans, WorkflowParameters parameters, string rawFileName)
        {
            Dictionary<double, int> reverseTimeLookup = ReverseRetentionTimeLookup(retentionTimes, precursorScans);

            string QcSearchDataDirectory = parameters.QcParams.QcSearchDataDirectory;
            string resultsFile = Path.Combine(QcSearchDataDirectory, Path.GetFileName(rawFileName) + ".pep.xml");

            XElement results = XElement.Load(resultsFile);

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

                psm.RetentionTime = Math.Round(Convert.ToDouble(x.Attribute("rt").Value), 4);

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

        public static PsmDataCollection AddPeakApex(PsmDataCollection psms, PrecursorPeakCollection peakData)
        {
            foreach (var key in psms.Keys)
            {
                var psm = psms[key];
                int scan = psm.Scan;
                psms[key].PeakApexRT = peakData[scan].MaximumRetTime;
            }

            return psms;
        }

        public static MultiRunFeatureCollection CorrelateFeatures(PsmDataCollection psms1, PsmDataCollection psms2, PrecursorPeakCollection peaks1, PrecursorPeakCollection peaks2, PrecursorMassCollection masses1, PrecursorMassCollection masses2)
        {
            MultiRunFeatureCollection features = new MultiRunFeatureCollection();

            int ID = 0;

            foreach (var key1 in psms1.Keys)
            {
                // get basic info on the feature
                List<PsmData> possibles = new List<PsmData>();
                int ms2scan1 = psms1[key1].Scan;
                double rt1 = peaks1[key1].MaximumRetTime;
                double mass1 = masses1[key1].MonoisotopicMZ;
                bool found = false;
                MultiRunFeature feature = new MultiRunFeature();
                feature.FoundIn1 = true;
                feature.IdIn1 = true;

                feature.RT1 = rt1;
                feature.Mass1 = mass1;

                // look for the feature in the other set of psms
                foreach (var key2 in psms2.Keys)
                {
                    double rt2 = peaks2[key2].MaximumRetTime;
                    double mass2 = masses2[key2].MonoisotopicMZ;
                    int ms2scan2 = psms2[key2].Scan;

                    if ((Math.Abs(rt1 - rt2)/rt1 < 0.05) & (Math.Abs(mass1 - mass2)/mass1 * 1e6 < 6))
                    {
                        //feature.Add(psms1.FileName, psms1[key1]);
                        //feature.Add(psms2.FileName, psms1[key2]);
                        feature.IdIn2 = true;
                        feature.FoundIn2 = true;

                        feature.RT2 = rt2;
                        feature.Mass2 = mass2;
                        //Console.WriteLine("{0}\t{1}", psms1[key1].Seq, psms2[key2].Seq);
                        string seq1 = psms1[key1].Seq;
                        string seq2 = psms2[key2].Seq;

                        if (seq1 == seq2)
                        {
                            feature.ConfirmSeqMatch = true;
                        }

                        features.Add(ID, feature);
                        ID++;
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                foreach (var peak2 in peaks2.Values)
                {
                    double rt2 = peak2.MaximumRetTime;
                    double mass2 = masses2[peak2.Ms2Scan].MonoisotopicMZ;

                    if ((Math.Abs(rt1 - rt2) / rt1 < 0.05) & (Math.Abs(mass1 - mass2) / mass1 * 1e6 < 6))
                    {
                        //feature.Add(psms1.FileName, psms1[key1]);
                        //feature.Add(psms2.FileName, null);
                        feature.IdIn2 = false;
                        feature.FoundIn2 = true;

                        feature.RT2 = rt2;
                        feature.Mass2 = mass2;

                        features.Add(ID, feature);
                        ID++;
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                feature.IdIn2 = false;
                feature.FoundIn2 = false;

                features.Add(ID, feature);
                ID++;
            }

            foreach (var key2 in psms2.Keys)
            {
                // get basic info on the feature
                List<PsmData> possibles = new List<PsmData>();
                int ms2scan2 = psms2[key2].Scan;
                double rt2 = peaks2[key2].MaximumRetTime;
                double mass2 = masses2[key2].MonoisotopicMZ;
                bool found = false;
                MultiRunFeature feature = new MultiRunFeature();
                feature.FoundIn2 = true;
                feature.IdIn2 = true;

                feature.RT2 = rt2;
                feature.Mass2 = mass2;

                // look for the feature in the other set of psms
                foreach (var key1 in psms1.Keys)
                {
                    double rt1 = peaks1[key1].MaximumRetTime;
                    double mass1 = masses1[key1].MonoisotopicMZ;
                    int ms2scan1 = psms1[key1].Scan;

                    if ((Math.Abs(rt2 - rt1) / rt2 < 0.05) & (Math.Abs(mass2 - mass1) / mass2 * 1e6 < 6))
                    {
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                foreach (var peak1 in peaks1.Values)
                {
                    double rt1 = peak1.MaximumRetTime;
                    double mass1 = masses1[peak1.Ms2Scan].MonoisotopicMZ;

                    if ((Math.Abs(rt2 - rt1) / rt2 < 0.05) & (Math.Abs(mass2 - mass1) / mass2 * 1e6 < 6))
                    {
                        //feature.Add(psms2.FileName, psms2[key2]);
                        //feature.Add(psms1.FileName, null);
                        feature.IdIn1 = false;
                        feature.FoundIn1 = true;

                        feature.RT1 = rt1;
                        feature.Mass1 = mass1;

                        features.Add(ID, feature);
                        ID++;
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                feature.IdIn1 = false;
                feature.FoundIn1 = false;

                features.Add(ID, feature);
                ID++;
            }

            return features;
        }

        public static void AlignRT(PsmDataCollection psms1, PsmDataCollection psms2)
        {
            
        }
    }
}
