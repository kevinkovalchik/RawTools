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
using System.Xml.Linq;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools;
using RawTools.Utilities;
using RawTools.Data.IO;
using RawTools.QC;
using RawTools.WorkFlows;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Interfaces;
using Serilog;

namespace RawTools.QC
{
    static class SearchQC
    {
        public static int GetMissedCleavages(string sequence)
        {
            int num = 0;

            for (int i = 0; i < sequence.Length - 1; i++)
            {
                if ((sequence[i] == 'K' | sequence[i] == 'R') & sequence[i + 1] != 'P')
                {
                    num += 1;
                }
            }

            return num;
        }
        
        public static XElement LoadSearchResults(WorkflowParameters parameters, string rawFileName)
        {
            string QcSearchDataDirectory = parameters.QcParams.QcSearchDataDirectory;
            string resultsFile = Path.Combine(QcSearchDataDirectory, Path.GetFileName(rawFileName) + ".pep.xml");

            if (!File.Exists(resultsFile))
            {
                Console.WriteLine("ERROR: Database search results file does not exist. Did the search fail?");
                Environment.Exit(1);
            }

            return XElement.Load(resultsFile);
        }

        public static SearchMetricsContainer ParseSearchResults(SearchMetricsContainer searchMetrics, WorkflowParameters parameters, string rawFileName, int nScans)
        {
            XElement results = LoadSearchResults(parameters, rawFileName);

            PsmDataCollection Psms = ExtractPsmData(results, parameters.QcParams.SearchAlgorithm);

            searchMetrics.ParsePSMs(Psms, parameters, nScans: nScans);

            return searchMetrics;
        }
        
        public static PsmDataCollection ExtractPsmData(XElement results, SearchAlgorithm searchAlgorithm)
        {
            PsmDataCollection psms = new PsmDataCollection();
            PsmData psm;

            if (searchAlgorithm == SearchAlgorithm.XTandem)
            {
                foreach (var x in results.Descendants("group").Where(x => x?.Element("protein") != null))
                {
                    psm = new PsmData();

                    psm.Id = Convert.ToInt32(x.Attribute("id").Value);

                    psm.Decoy = x.Attribute("label").Value.StartsWith("DECOY_");

                    // it is possible for each "group" in the pepXML file to have more than one protein. This just means the peptide isn't
                    // unique to a single protein. However, the scoring and modifications are identical (since it is the same PSM), so we
                    // can just use the first protein. That is what we do below.
                    XElement domain = x.Element("protein").Element("peptide").Element("domain");

                    psm.Seq = domain.Attribute("seq").Value;

                    psm.Start = Convert.ToInt32(domain.Attribute("start").Value);

                    psm.End = Convert.ToInt32(domain.Attribute("end").Value);

                    psm.Hyperscore = Convert.ToDouble(domain.Attribute("hyperscore").Value);

                    psm.ExpectationValue = Convert.ToDouble(domain.Attribute("expect").Value);

                    psm.MassDrift = (Convert.ToDouble(x.Attribute("mh")?.Value) - Convert.ToDouble(domain?.Attribute("mh").Value)) /
                        Convert.ToDouble(domain?.Attribute("mh").Value) * 1e6;

                    psm.Charge = Convert.ToInt32(x.Attribute("z").Value);

                    psm.MissedCleavages = GetMissedCleavages(psm.Seq);

                    // add the modifications, if there are any
                    if (domain?.Elements("aa") != null)
                    {
                        foreach (XElement aa in domain.Elements("aa"))
                        {
                            Modification mod = new Modification();
                            // we convert the location to a zero-based index of the peptide
                            mod.Loc = Convert.ToInt32(aa.Attribute("at").Value) - psm.Start;

                            mod.AA = aa.Attribute("type").Value;

                            mod.Mass = Convert.ToDouble(aa.Attribute("modified").Value);

                            psm.Mods.Add(mod);
                        }
                    }                    

                    psms.Add(psm.Id, psm);
                }
            }

            return psms;
        }

        public static void ParsePSMs(this SearchMetricsContainer searchMetrics, PsmDataCollection psmCollection, WorkflowParameters parameters, int nScans)
        {
            int numGoodPSMs, pepsWithNoMissedCleavages;
            IEnumerable<int> charges;
            double IdRate, chargeRatio3to2, chargeRatio4to2;
            double digestionEfficiency, topDecoyScore;
            double missedCleavageRate;
            Dictionary<int, int> numCharges = new Dictionary<int, int>();
            List<PsmData> psms;
            IEnumerable<PsmData> goodPsms, nonDecoys;

            int numSearched = parameters.QcParams.NumberSpectra;

            if (numSearched > nScans) numSearched = nScans;

            // convert the dictionary to a list for easy parsing
            psms = psmCollection.Values.ToList();

            // get the top decoy score
            topDecoyScore = (from x in psms
                             where x.Decoy
                             select x.Hyperscore)
                             .ToArray().Percentile(95);

            // get the non-decoys
            nonDecoys = from x in psms
                        where !x.Decoy
                        select x;

            // and select the non-decoy hits which are above the top decoy score
            goodPsms = from x in psms
                       where !x.Decoy & x.Hyperscore > topDecoyScore
                       select x;

            searchMetrics.CutoffDecoyScore = topDecoyScore;
            searchMetrics.NumPSMs = goodPsms.Count();

            searchMetrics.MedianPeptideScore = (from x in goodPsms
                                                select x.Hyperscore)
                                                .ToArray().Percentile(50);

            HashSet<string> seqs = new HashSet<string>();

            foreach (var psm in goodPsms) seqs.Add(psm.Seq);

            searchMetrics.NumUniquePeptides = seqs.Count;


            Console.WriteLine("Total hits: {0}", psms.Count());
            Console.WriteLine("Top decoy score: {0}", topDecoyScore);
            Console.WriteLine("Non-decoy hits: {0}", nonDecoys.Count());
            Console.WriteLine("Non-decoy hits above top decoy score: {0}", goodPsms.Count());
            

            // parse out the charges
            charges = from x in goodPsms
                      select x.Charge;

            // get the number of each charge, add to a dictionary
            foreach (int charge in new List<int>() { 2, 3, 4 })
            {
                numCharges.Add(charge, (from x in charges where x == charge select 1).Count());
            }

            // calculate charge ratios
            chargeRatio3to2 = Convert.ToDouble(numCharges[3]) / Convert.ToDouble(numCharges[2]);
            chargeRatio4to2 = Convert.ToDouble(numCharges[4]) / Convert.ToDouble(numCharges[2]);

            // parse out the missed cleavage data
            pepsWithNoMissedCleavages = (from x in goodPsms
                                       where x.MissedCleavages == 0
                                       select 1).Sum();

            // number of PSMs is the length of this collection
            numGoodPSMs = goodPsms.Count();

            // missed cleavages per PSM
            digestionEfficiency = (double)pepsWithNoMissedCleavages / numGoodPSMs;
            Console.WriteLine("Digestion efficiency: {0}", digestionEfficiency);

            // get missed cleavage rate, i.e. number of missed cleavages per psm
            missedCleavageRate = (double)(from x in goodPsms select x.MissedCleavages).Sum() / numGoodPSMs;
            Console.WriteLine("Missed cleavage rate (/PSM): {0}", missedCleavageRate);

            // calculate ID rate
            IdRate = (double)numGoodPSMs / numSearched;
            Console.WriteLine("IDrate: {0}", IdRate);

            // get labeling efficiency metrics
            if (!String.IsNullOrEmpty(parameters.QcParams.VariableMods))
            {
                searchMetrics.GetModificationFrequency(goodPsms, parameters);
            }

            // get median mass drift
            searchMetrics.MedianMassDrift = (from x in goodPsms
                                      select x.MassDrift)
                                      .ToArray().Percentile(50);

            searchMetrics.SearchData.PSMsWithNoMissedCleavages = pepsWithNoMissedCleavages;
            searchMetrics.SearchData.TotalNumGoodPSMs = numGoodPSMs;
            searchMetrics.SearchData.NumCharge2 = numCharges[2];
            searchMetrics.SearchData.NumCharge3 = numCharges[3];
            searchMetrics.SearchData.NumCharge4 = numCharges[4];

            searchMetrics.IdentificationRate = IdRate;
            searchMetrics.MissedCleavageRate = missedCleavageRate;
            searchMetrics.DigestionEfficiency = digestionEfficiency;
            searchMetrics.ChargeRatio3to2 = chargeRatio3to2;
            searchMetrics.ChargeRatio4to2 = chargeRatio4to2;
        }

        public static void GetModificationFrequency(this SearchMetricsContainer searchMetrics, IEnumerable<PsmData> psms, WorkflowParameters parameters)
        {
            Dictionary<string, string> Modifications = new Dictionary<string, string>();
            Dictionary<char, int> TotalModificationsSites = new Dictionary<char, int>();
            Dictionary<string, int> ModificationSitesHit = new Dictionary<string, int>();
            Dictionary<string, double> ModificationEfficiency = new Dictionary<string, double>();
            Dictionary<string, int> AmbiguousSites = new Dictionary<string, int>();
            List<Modification> mods;
            List<char> AminosOfInterest = new List<char>();
            
            string[] Mods = parameters.QcParams.VariableMods.Split(',');

            //TotalLabelingSites.Add('[', 0);
            //AmbiguousSites.Add("[", 0);

            // "Prime" the dictionaries
            foreach (var item in Mods)
            {
                if (item == null)
                {
                    continue;
                }

                var splitString = item.Split('@');

                Modifications.Add(splitString.Last(), splitString.First());

                TotalModificationsSites.Add(splitString.Last().Last(), 0);
                AmbiguousSites.Add(splitString.Last(), 0);

                ModificationSitesHit.Add(item, 0);

                AminosOfInterest.Add(splitString.Last().Last());
            }

            bool nTermIsVariable = Modifications.ContainsKey("[");

            foreach (PsmData psm in psms)
            {
                mods = psm.Mods.OrderBy(x => x.Loc).ToList();
                int startLoc = 0;

                if (AminosOfInterest.Contains(psm.Seq[0]) && nTermIsVariable &&
                    Modifications["["] == Modifications[psm.Seq[0].ToString()])
                {
                    // if both nTerm and the AA are variable and have the same modification mass,
                    // then we can't tell which one is labeled if only one gets a hit.
                    // If that is the case, skip the first amino acid in the sequence

                    int hits = (from x in mods where x.Loc == 0 select 1).Count();

                    if (hits == 1)
                    {
                        startLoc = 1;
                        AmbiguousSites["["]++;
                        AmbiguousSites[psm.Seq[0].ToString()]++;
                    }// do nothing
                    else if (hits == 0)
                    {
                        startLoc = 0;
                    }
                    else
                    {
                        startLoc = 2;
                        ModificationSitesHit[Modifications[psm.Seq[0].ToString()] + "@["]++;
                        ModificationSitesHit[Modifications[psm.Seq[0].ToString()] + "@" + psm.Seq[0].ToString()]++;
                    }

                }
                for (int i = startLoc; i < mods.Count; i++)
                {
                    var mod = mods[i];

                    if (mod.Loc == 0 & ModificationSitesHit.ContainsKey(mod.Mass.ToString() + "@["))
                    {
                        ModificationSitesHit[mod.Mass.ToString() + "@["]++;
                    }
                    else if (ModificationSitesHit.ContainsKey(mod.MassAtAa))
                    {
                        ModificationSitesHit[mod.MassAtAa]++;
                    }
                }

                // now add to total modifications sites

                if (nTermIsVariable) TotalModificationsSites['[']++;

                foreach (var aa in psm.Seq)
                {
                    if (AminosOfInterest.Contains(aa))
                    {
                        if (TotalModificationsSites.ContainsKey(aa))
                        {
                            TotalModificationsSites[aa]++;
                        }
                    }
                }
            }

            // remove ambiguous modification sites from total modification sites
            foreach (var site in TotalModificationsSites.Keys.ToList())
            {
                TotalModificationsSites[site] = TotalModificationsSites[site] - AmbiguousSites[site.ToString()];
            }

            // spit out some metrics to the console and write efficiencies to the search metrics container
            /*
            foreach (char aa in AminosOfInterest)
            {
                if (aa == '[')
                {
                    Console.WriteLine("Total N-term sites: {0}", TotalLabelingSites['[']);
                }
                else
                {
                    Console.WriteLine("Total {0} sites: {1}", aa, TotalLabelingSites[aa]);
                }
            }*/

            foreach (var hits in ModificationSitesHit)
            {
                double efficiency = (double)hits.Value / TotalModificationsSites[hits.Key.Last()];

                Console.WriteLine("{0} modification frequency: {1}", hits.Key, efficiency);

                searchMetrics.ModificationFrequency[hits.Key] = efficiency;
            }
        }
    }
}
