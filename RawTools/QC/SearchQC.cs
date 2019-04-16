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

            return XElement.Load(resultsFile);
        }

        public static SearchMetricsContainer ParseSearchResults(SearchMetricsContainer searchMetrics, WorkflowParameters parameters, string rawFileName)
        {
            XElement results = LoadSearchResults(parameters, rawFileName);

            PsmDataCollection Psms = ExtractPsmData(results, parameters.QcParams.SearchAlgorithm);

            searchMetrics.ParsePSMs(Psms, parameters);

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

            if (searchAlgorithm == SearchAlgorithm.IdentiPy)
            {
                XNamespace nsp = "http://regis-web.systemsbiology.net/pepXML";

                // first we need to make a dictionary of modification masses etc for the identipy results
                // the keys are the amino acid mass after modification, which is what identipy reports
                // the values are the mass difference values, which is what is given in the mass@aa arguments to the CLI
                XElement summary = results.Descendants(nsp + "search_summary").First();
                Dictionary<double, double> modInfo = new Dictionary<double, double>();

                foreach (XElement mod in summary.Elements(nsp + "aminoacid_modification"))
                {
                    modInfo.Add(Convert.ToDouble(mod.Attribute("mass").Value), Convert.ToDouble(mod.Attribute("massdiff").Value));
                }
                foreach (XElement mod in summary.Elements(nsp + "terminal_modification"))
                {
                    modInfo.Add(Convert.ToDouble(mod.Attribute("mass").Value), Convert.ToDouble(mod.Attribute("massdiff").Value));
                }

                // now we can parse out the data

                foreach (var x in results.Descendants(nsp + "spectrum_query"))
                {
                    psm = new PsmData();
                    
                    psm.Id = Convert.ToInt32(x.Attribute("index").Value);

                    XElement searchHit = x.Element(nsp + "search_result").Element(nsp + "search_hit");

                    psm.Decoy = searchHit.Attribute("protein").Value.StartsWith("DECOY_");

                    psm.Seq = searchHit.Attribute("peptide").Value;

                    psm.Start = -1;

                    psm.End = -1;

                    psm.Hyperscore = Convert.ToDouble(searchHit.Elements(nsp + "search_score")
                        .Where(y => y.Attribute("name").Value == "hyperscore").First().Attribute("value").Value);

                    psm.ExpectationValue = Convert.ToDouble(searchHit.Elements(nsp + "search_score")
                        .Where(y => y.Attribute("name").Value == "expect").First().Attribute("value").Value);

                    psm.MassDrift = Convert.ToDouble(searchHit.Attribute("massdiff").Value) / Convert.ToDouble(x.Attribute("precursor_neutral_mass").Value) * 1e6;

                    psm.Charge = Convert.ToInt32(x.Attribute("assumed_charge").Value);

                    psm.MissedCleavages = GetMissedCleavages(psm.Seq);

                    // add the modifications, if there are any
                    if (searchHit.Element(nsp + "modification_info")?.Attribute("mod_nterm_mass") != null)
                    {
                        Modification mod = new Modification();

                        mod.Loc = 0; // its the n-terminus

                        mod.AA = psm.Seq[0].ToString();

                        mod.Mass = modInfo[Convert.ToDouble(searchHit.Element(nsp + "modification_info").Attribute("mod_nterm_mass").Value)];

                        psm.Mods.Add(mod);
                    }

                    if (searchHit.Element(nsp + "modification_info")?.Elements(nsp + "mod_aminoacid_mass") != null)
                    {
                        foreach (XElement aa in searchHit.Element(nsp + "modification_info").Elements(nsp + "mod_aminoacid_mass"))
                        {
                            Modification mod = new Modification();
                            // we convert the location to a zero-based index of the peptide
                            mod.Loc = Convert.ToInt32(aa.Attribute("position").Value) - 1;

                            mod.AA = psm.Seq[mod.Loc].ToString();

                            mod.Mass = modInfo[Convert.ToDouble(aa.Attribute("mass").Value)];

                            psm.Mods.Add(mod);
                        }
                    }

                    psms.Add(psm.Id, psm);
                }
            }

            return psms;
        }

        public static void ParsePSMs(this SearchMetricsContainer searchMetrics, PsmDataCollection psmCollection, WorkflowParameters parameters)
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
            if ((parameters.QcParams.NMod != null) | (parameters.QcParams.KMod != null) | (parameters.QcParams.XMod != null))
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

            string nmod = parameters.QcParams.NMod;
            string kmod = parameters.QcParams.KMod;
            string xmod = parameters.QcParams.XMod;
            Dictionary<string, string> Modifications = new Dictionary<string, string>();
            Dictionary<string, int> TotalLabelingSites = new Dictionary<string, int>();
            Dictionary<string, int> LabelingSitesHit = new Dictionary<string, int>();
            Dictionary<string, double> LabelingEfficiency = new Dictionary<string, double>();
            List<Modification> mods;
            List<string> AminosOfInterest = new List<string>();

            string[] Mods = new string[] { nmod, kmod, xmod };

            // "Prime" the dictionaries
            foreach (var item in Mods)
            {
                if (item == null)
                {
                    continue;
                }
                var splitString = item.Split('@');
                // add the key: value pairs as mass@AA:AA
                Modifications.Add(item, splitString.Last());
                // and AA:int
                TotalLabelingSites.Add(splitString.Last(), 0);
                LabelingSitesHit.Add(splitString.Last(), 0);
                AminosOfInterest.Add(splitString.Last());
            }

            foreach (PsmData psm in psms)
            {
                mods = psm.Mods;

                // check the sequence in two steps. First the n-terminus, then remove the n-terminus and check the rest of it.

                // FIRST STEP: N-TERMINUS

                if (nmod != null)
                {
                    // check if the first residue is lysine
                    if (psm.Seq[0] == 'K')
                    {
                        // if so, we need to see if it was only labeled once. Skip the psm if that is the case because it is ambiguous
                        IEnumerable<Modification> nMods = from x in mods
                                                          where x.Loc == 0
                                                          select x;
                        int numMods = nMods.Count();

                        if (numMods == 1)
                        {
                            // we can't know which reactive site is modified, so don't include this peptide
                            continue;
                        }
                        if (numMods == 0)
                        {
                            // nothing is labeled
                            TotalLabelingSites["["] += 1;
                            if (AminosOfInterest.Contains("K"))
                            {
                                TotalLabelingSites["K"] += 1;
                            }
                        }
                        if (numMods == 2)
                        {
                            TotalLabelingSites["["] += 1;
                            LabelingSitesHit["["] += 1;

                            if (AminosOfInterest.Contains("K"))
                            {
                                TotalLabelingSites["K"] += 1;
                                LabelingSitesHit["K"] += 1;
                            }
                        }
                    }
                    // If the first residue is not lysine
                    else
                    {
                        IEnumerable<Modification> nMods = from x in mods
                                                          where x.Loc == 0
                                                          select x;

                        // add 1 to total n-termini, because it is always there
                        TotalLabelingSites["["] += 1;

                        // get the aa residue letter
                        string residue = psm.Seq[0].ToString();

                        //see if it is of interest
                        if (AminosOfInterest.Contains(residue))
                        {
                            // if so, add 1 to total sites for it
                            TotalLabelingSites[residue] += 1;
                        }

                        // now go through each detected modification
                        foreach (Modification mod in nMods)
                        {
                            if (nmod.Contains(mod.Mass.ToString()))
                            {
                                LabelingSitesHit["["] += 1;
                            }
                            else
                            {
                                if (AminosOfInterest.Contains(mod.AA))
                                {
                                    LabelingSitesHit[mod.AA] += 1;
                                }
                            }
                        }
                    }
                }
                int start;
                if (nmod != null)
                {
                    start = 1;
                }
                else
                {
                    start = 0;
                }

                // now continue with the rest

                for (int i = start; i < psm.Seq.Length; i++)
                {
                    // check if we care about this amino acid
                    string aa = psm.Seq[i].ToString();
                    if (AminosOfInterest.Contains(aa))
                    {
                        // add one to potential labeling sites
                        TotalLabelingSites[aa] += 1;

                        // There should only ever be one modification for each of the rest of the residues, so we can reference it by location to see if it exists
                        bool hit = (from x in mods
                                    where x.Loc == i
                                    select 1).Count() == 1;
                        if (hit)
                        {
                            LabelingSitesHit[aa] += 1;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            // spit out some metrics to the console

            foreach (string aa in AminosOfInterest)
            {
                if (aa == "[")
                {
                    Console.WriteLine("Total N-term sites: {0}", TotalLabelingSites["["]);
                }
                else
                {
                    Console.WriteLine("Total {0} sites: {1}", aa, TotalLabelingSites[aa]);
                }
            }

            foreach (string aa in AminosOfInterest)
            {
                if (aa == "[")
                {
                    Console.WriteLine("Missed modifications at N-term: {0}", TotalLabelingSites["["] - LabelingSitesHit["["]);
                }
                else
                {
                    Console.WriteLine("Missed modifications at {0}: {1}", aa, TotalLabelingSites[aa] - LabelingSitesHit[aa]);
                }
            }

            // calculate labelling efficiency for each site
            foreach (var aa in AminosOfInterest)
            {
                double efficiency = (double)LabelingSitesHit[aa] / TotalLabelingSites[aa];
                LabelingEfficiency.Add(aa, efficiency);
                if (aa == "[")
                {
                    Console.WriteLine("Modification frequency at N-term: {0}", efficiency);
                }
                else
                {
                    Console.WriteLine("Modification frequency at {0}: {1}", aa, efficiency);
                }

                // if the sites are n-term or K add them to their own attributes
                if (aa == "[")
                {
                    searchMetrics.LabelingEfficiencyAtNTerm = efficiency;
                    searchMetrics.SearchData.NLabelSites = TotalLabelingSites[aa];
                    searchMetrics.SearchData.NLabelSitesHit = LabelingSitesHit[aa];
                }
                else
                {
                    if (aa == "K")
                    {
                        searchMetrics.LabelingEfficiencyAtK = efficiency;
                        searchMetrics.SearchData.KLabelSites = TotalLabelingSites[aa];
                        searchMetrics.SearchData.KLabelSitesHit = LabelingSitesHit[aa];
                    }
                    // if not, then add it to xmod attributes
                    else
                    {
                        searchMetrics.LabelingEfficiencyAtX = efficiency;
                        searchMetrics.LabelX = aa;
                        searchMetrics.SearchData.XLabelSites = TotalLabelingSites[aa];
                        searchMetrics.SearchData.XLabelSitesHit = LabelingSitesHit[aa];
                    }
                }
            }
        }
    }
}
