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

        public static void ParseXTandem(QcDataContainer qcData, QcParameters qcParameters)
        {
            XElement results, searchSummary;
            IEnumerable<XElement> top_scores, decoyPSMs, search_hits, spectrumQueries;
            int numGoodPSMs, totalCleavageSites, numMissedCleavages, peptidesWithNoMissedCleavages;
            IEnumerable<int> allMissedCleavages, charges;
            double IdRate, chargeRatio3to2, chargeRatio4to2;
            double digestionEfficiencyByCleavage, digestionEfficiencyByPeptide, topDecoyScore;
            double missedCleavageRate;
            Dictionary<int, int> numCharges = new Dictionary<int, int>();
            SearchParameters searchParameters = qcParameters.searchParameters;
            int numSearched = searchParameters.NumSpectra;

            // load the xml Identipy results file
            string QcSearchDataDirectory = qcParameters.QcSearchDataDirectory;
            string resultsFile = Path.Combine(QcSearchDataDirectory, Path.GetFileName(qcData.RawFile) + ".pep.xml");
            results = XElement.Load(resultsFile);

            // add the namespace
            XNamespace nsp = "http://www.bioml.com/gaml/";

            // parse out the search summary
            searchSummary = results.Descendants("group").Where(x => { return x?.Attribute("type").Value == "parameters"; }).FirstOrDefault();

            // parse out the PSMs
            spectrumQueries = results.Descendants("group").Where(x => { return x?.Attribute("type").Value == "model"; });
            Console.WriteLine(spectrumQueries.Count());

            // keep only the PSMs with scores higher than the highest scoring decoy
            // parse out the decoy hits
            var decoyQueries = from x in spectrumQueries
                               where x.Attribute("label").Value.StartsWith("DECOY_")
                               select x;

            // and the non-decoy hits
            var nonDecoyQueries = from x in spectrumQueries
                                  where !x.Attribute("label").Value.StartsWith("DECOY_")
                                  select x;

            // get the top decoy score
            topDecoyScore = (from x in decoyQueries
                             select Convert.ToDouble(x?.Element("protein")?.Element("peptide")?.Element("domain")?.Attribute("hyperscore")?.Value)).ToArray().Percentile(95);

            // and finally select the non-decoy hits which are above the top decoy score
            top_scores = from x in nonDecoyQueries
                         where Convert.ToDouble(x?.Element("protein")?.Element("peptide")?.Element("domain")?.Attribute("hyperscore")?.Value) > topDecoyScore
                         select x;

            Console.WriteLine("Top decoy score: {0}", topDecoyScore);
            Console.WriteLine("Non-decoy hits: {0}", nonDecoyQueries.Count());
            Console.WriteLine("Search hits above top decoy: {0}", top_scores.Count());

            // parse out the charges
            charges = from x in top_scores select Convert.ToInt32(x.Attribute("z").Value);

            // get the number of each charge, add to a dictionary
            foreach (int charge in new List<int>() { 2, 3, 4 })
            {
                numCharges.Add(charge, (from x in charges where x == charge select x).Count());
            }

            // calculate charge ratios
            chargeRatio3to2 = Convert.ToDouble(numCharges[3]) / Convert.ToDouble(numCharges[2]);
            chargeRatio4to2 = Convert.ToDouble(numCharges[4]) / Convert.ToDouble(numCharges[2]);

            // get out the peptide info
            search_hits = top_scores.Elements("protein").Elements("peptide").Elements("domain");

            // parse out the missed cleavage data
            allMissedCleavages = (from x in search_hits select SearchQC.GetMissedCleavages(x.Attribute("seq").Value));

            // number of PSMs is the length of this collection
            numGoodPSMs = search_hits.Count();

            // total missed cleavages is the sum
            numMissedCleavages = allMissedCleavages.Sum();

            // missed cleavages per PSM
            missedCleavageRate = Convert.ToDouble(numMissedCleavages) / numGoodPSMs;

            // calculate ID rate
            IdRate = Convert.ToDouble(numGoodPSMs) / numSearched;
            Console.WriteLine("IDrate: {0}", IdRate);

            // the total number of cleavage sites is missed cleavages + number of PSMs (not 2 X missed cleavages. that would count each terminus twice)
            totalCleavageSites = numGoodPSMs + numMissedCleavages;

            // this leads us to the cleavage digestion efficiency
            digestionEfficiencyByCleavage = Convert.ToDouble(totalCleavageSites - numMissedCleavages) / totalCleavageSites;

            // now get number of peptides with no missed cleavages
            peptidesWithNoMissedCleavages = (from x in allMissedCleavages where x == 0 select x).Count();

            // and the peptide digestion efficiency is peptides with no cleavages over psms
            digestionEfficiencyByPeptide = Convert.ToDouble(peptidesWithNoMissedCleavages) / numGoodPSMs;
            Console.WriteLine("Digestion efficiency: {0}", digestionEfficiencyByPeptide);

            // get labeling efficiency metrics
            if ((searchParameters.NMod != null) | (searchParameters.KMod != null) | (searchParameters.XMod != null))
            {
                qcData.GetModificationFrequencyXTandem(searchSummary, top_scores, nsp, searchParameters.NMod, searchParameters.KMod,
                    searchParameters.XMod);
            }

            // get median mass drift
            Console.WriteLine(top_scores.Count());
            qcData.MedianMassDrift = GetMassDrift(top_scores, search: SearchAlgorithm.XTandem);

            qcData.IdentificationRate = IdRate;
            qcData.MissedCleavageRate = missedCleavageRate;
            qcData.DigestionEfficiency = digestionEfficiencyByPeptide;
            qcData.ChargeRatio3to2 = chargeRatio3to2;
            qcData.ChargeRatio4to2 = chargeRatio4to2;
        }

        public static void ParseIdentipy(QcDataContainer qcData, RawDataCollection rawData, IRawDataPlus rawFile, QcParameters qcParameters)
        {
            XElement results, searchSummary;
            IEnumerable<XElement> top_scores, decoyPSMs, search_hits, spectrumQueries;
            int numGoodPSMs, totalCleavageSites, numMissedCleavages, peptidesWithNoMissedCleavages;
            IEnumerable<int> allMissedCleavages, charges;
            double IdRate, chargeRatio3to2, chargeRatio4to2;
            double digestionEfficiencyByCleavage, digestionEfficiencyByPeptide, topDecoyScore;
            double missedCleavageRate;
            Dictionary<int, int> numCharges = new Dictionary<int, int>();
            SearchParameters searchParameters = qcParameters.searchParameters;
            int numSearched = searchParameters.NumSpectra;
            string QcSearchDataDirectory = qcParameters.QcSearchDataDirectory;

            // load the xml Identipy results file
            string resultsFile = Path.Combine(QcSearchDataDirectory, Path.GetFileName(rawData.rawFileName) + ".pep.xml");
            results = XElement.Load(resultsFile);

            // add the namespace
            XNamespace nsp = "http://regis-web.systemsbiology.net/pepXML";

            // parse out the search summary
            searchSummary = results.Descendants(nsp + "search_summary").First();

            // parse out the PSMs
            spectrumQueries = results.Descendants(nsp + "spectrum_query");

            // keep only the PSMs with scores higher than the highest scoring decoy
            // parse out the decoy hits
            var decoyQueries = from x in spectrumQueries
                               where x.Element(nsp + "search_result").Element(nsp + "search_hit").Attribute("protein").Value.StartsWith("DECOY_")
                               select x;

            // and the non-decoy hits
            var nonDecoyQueries = from x in spectrumQueries
                                  where !x.Element(nsp + "search_result").Element(nsp + "search_hit").Attribute("protein").Value.StartsWith("DECOY_")
                                  select x;

            // get the top decoy score
            topDecoyScore = (from x in decoyQueries.Elements(nsp + "search_result").Elements(nsp + "search_hit").Elements(nsp + "search_score")
                             where x.Attribute("name").Value == "hyperscore"
                             select Convert.ToDouble(x.Attribute("value").Value)).ToArray().Percentile(95);

            // add a score attribute to the search_hit element so we can easily parse out the higher scores
            foreach (var query in nonDecoyQueries)
            {
                var score = (from x in query.Element(nsp + "search_result").Element(nsp + "search_hit").Elements(nsp + "search_score")
                             where x.Attribute("name").Value == "hyperscore"
                             select x.Attribute("value").Value).FirstOrDefault();
                query.Element(nsp + "search_result").Element(nsp + "search_hit").SetAttributeValue("score", score);
            }

            // and finally select the non-decoy hits which are above the top decoy score
            top_scores = from x in nonDecoyQueries
                         where Convert.ToDouble(x.Element(nsp + "search_result").Element(nsp + "search_hit").Attribute("score").Value) > topDecoyScore
                         select x;

            Console.WriteLine("Top decoy score: {0}", topDecoyScore);
            Console.WriteLine("Non-decoy hits: {0}", nonDecoyQueries.Count());
            Console.WriteLine("Search hits above top decoy: {0}", top_scores.Count());

            //searchHits = spectrumQueries.Descendants(nsp + "search_hit");

            //decoyPSMs = from x in searchHits where x.Attribute("protein").Value.StartsWith("DECOY_") select x;
            //goodPSMs = from x in searchHits where !x.Attribute("protein").Value.StartsWith("DECOY_") select x;

            // parse out the charges
            charges = from x in top_scores select Convert.ToInt32(x.Attribute("assumed_charge").Value);

            // get the number of each charge, add to a dictionary
            foreach (int charge in new List<int>() { 2, 3, 4 })
            {
                numCharges.Add(charge, (from x in charges where x == charge select x).Count());
            }

            // calculate charge ratios
            chargeRatio3to2 = Convert.ToDouble(numCharges[3]) / Convert.ToDouble(numCharges[2]);
            chargeRatio4to2 = Convert.ToDouble(numCharges[4]) / Convert.ToDouble(numCharges[2]);

            // get out search_hit descendents
            search_hits = top_scores.Elements(nsp + "search_result").Elements(nsp + "search_hit");

            // parse out the missed cleavage data
            allMissedCleavages = (from x in search_hits select Convert.ToInt32(x.Attribute("num_missed_cleavages").Value));

            // number of PSMs is the length of this collection
            numGoodPSMs = search_hits.Count();

            // total missed cleavages is the sum
            numMissedCleavages = allMissedCleavages.Sum();

            // missed cleavages per PSM
            missedCleavageRate = Convert.ToDouble(numMissedCleavages) / numGoodPSMs;

            // calculate ID rate
            IdRate = Convert.ToDouble(numGoodPSMs) / numSearched;
            Console.WriteLine("IDrate: {0}", IdRate);

            // the total number of cleavage sites is missed cleavages + number of PSMs (not 2 X missed cleavages. that would count each terminus twice)
            totalCleavageSites = numGoodPSMs + numMissedCleavages;

            // this leads us to the cleavage digestion efficiency
            digestionEfficiencyByCleavage = Convert.ToDouble(totalCleavageSites - numMissedCleavages) / totalCleavageSites;

            // now get number of peptides with no missed cleavages
            peptidesWithNoMissedCleavages = (from x in allMissedCleavages where x == 0 select x).Count();

            // and the peptide digestion efficiency is peptides with no cleavages over psms
            digestionEfficiencyByPeptide = Convert.ToDouble(peptidesWithNoMissedCleavages) / numGoodPSMs;
            Console.WriteLine("Digestion efficiency: {0}", digestionEfficiencyByPeptide);

            // get labeling efficiency metrics
            if ((searchParameters.NMod != null) | (searchParameters.KMod != null) | (searchParameters.XMod != null))
            {
                qcData.GetModificationFrequencyIdentipy(searchSummary, search_hits, nsp, searchParameters.NMod, searchParameters.KMod,
                    searchParameters.XMod);
            }

            // get median mass drift
            qcData.MedianMassDrift = GetMassDrift(search_hits, search: SearchAlgorithm.IdentiPy);

            qcData.IdentificationRate = IdRate;
            qcData.MissedCleavageRate = missedCleavageRate;
            qcData.DigestionEfficiency = digestionEfficiencyByPeptide;
            qcData.ChargeRatio3to2 = chargeRatio3to2;
            qcData.ChargeRatio4to2 = chargeRatio4to2;
        }

        public static double GetMassDrift(IEnumerable<XElement> PSMs, SearchAlgorithm search)
        {
            double[] ppms;

            if (search == SearchAlgorithm.IdentiPy)
            {
                ppms = (from x in PSMs select (double)x.Attribute("massdiff") / (double)x.Attribute("calc_neutral_pep_mass") * 1e6).ToArray();
            }
            else
            {
                ppms = (from x in PSMs
                           select ((Convert.ToDouble(x.Attribute("mh")?.Value) -
                           Convert.ToDouble(x.Element("protein").Element("peptide").Element("domain")?.Attribute("mh").Value)) /
                           Convert.ToDouble(x.Element("protein").Element("peptide").Element("domain")?.Attribute("mh").Value))*1e6).ToArray();
            }
            var median_ppm = ppms.ToArray().Percentile(50); ;
            Console.WriteLine("Median mass drift(ppm): {0}", median_ppm);
            return median_ppm;
        }

        // public static void GetModificationFrequency(this QcDataContainer qcData)

        public static void GetModificationFrequencyXTandem(this QcDataContainer qcData, XElement searchSummary, IEnumerable<XElement> PSMs, XNamespace nsp, string nmod, string kmod, string xmod)
        {
            // get the modification mass and location out of qmods
            Dictionary<string, string> Modifications = new Dictionary<string, string>();
            Dictionary<string, int> LabelingSites = new Dictionary<string, int>();
            Dictionary<string, double> LabelingEfficiency = new Dictionary<string, double>();
            Dictionary<string, int> MissedMods = new Dictionary<string, int>();
            string xMod = null;

            string[] Mods = new string[] { nmod, kmod, xmod };

            foreach (var item in Mods)
            {
                if (item == null)
                {
                    continue;
                }
                var splitString = item.Split('@');
                // add the key: value pairs as AA:mass
                Modifications.Add(splitString.Last(), splitString.First());
            }

            // now we need to get at labeling efficiency
            int KTotalSites = 0;
            int KTotalMissed = 0;
            int NTotalSites = 0;
            int NTotalMissed = 0;
            int XTotalSites = 0;
            int XTotalMissed = 0;
            foreach (string aa in Modifications.Keys)
            {
                // if the modification is to the n-terminus, this is Nmod
                if (aa == "[")
                {
                    string massToLookFor = Modifications[aa];
                    int hits = 0;
                    int termini = 0;

                    IEnumerable<XElement> nTermMods;

                    foreach (var domain in PSMs.Elements("protein").Elements("peptide").Elements("domain"))
                    {
                        string term = domain.Attribute("start").Value;

                        nTermMods = from x in domain.Elements("aa")
                                        where (x.Attribute("at").Value == term & x.Attribute("modified").Value == massToLookFor)
                                        select x;

                        if (nTermMods.Count() > 0)
                        {
                            hits += 1;
                        }
                        termini += 1;
                    }
                    int missed = termini - hits;
                    MissedMods.Add(aa, missed);
                    NTotalMissed = missed;
                    NTotalSites = PSMs.Count();
                    LabelingSites.Add(aa, PSMs.Count());
                }

                // if the modification is not to the n-terminus or K, this is X
                if ((aa != "[") & (aa != "K"))
                {
                    xMod = aa;
                    string massToLookFor = Modifications[aa];

                    int missed = 0;

                    LabelingSites.Add(aa, 0);

                    foreach (var hit in PSMs)
                    {
                        int numLabeled = 0;
                        string peptide = hit?.Element("protein")?.Element("peptide")?.Element("domain")?.Attribute("seq").Value;

                        if (peptide == null) { continue; }

                        if (!peptide.Contains(aa))
                        {
                            // the amino acid isn't in the peptide, so go to the next PSMs
                            continue;
                        }

                        int numPresent = peptide.Count(x => x.ToString() == aa);
                        XTotalSites += numPresent;
                        LabelingSites[aa] += numPresent;

                        if (hit.Element("protein").Element("peptide").Element("domain").Elements("aa") != null)
                        {
                            numLabeled = (from x in hit.Element("protein").Element("peptide").Element("domain").Elements("aa")
                                          where x.Attribute("modified").Value == massToLookFor & x.Attribute("type").Value == aa
                                          select 1).Count();
                        }

                        if (numPresent == numLabeled)
                        {
                            // everything is labeled, so continue to the next
                            continue;
                        }
                        else
                        {
                            missed += (numPresent - numLabeled);
                        }
                    }

                    MissedMods.Add(aa, missed);
                    XTotalMissed = missed;
                }

                // if the modification is to K, this is Kmod
                if (aa == "K")
                {
                    string massToLookFor = Modifications[aa];

                    int missed = 0;

                    LabelingSites.Add(aa, 0);

                    foreach (var hit in PSMs)
                    {
                        int numLabeled = 0;
                        string peptide = hit?.Element("protein")?.Element("peptide")?.Element("domain")?.Attribute("seq").Value;

                        if (peptide == null){ continue; }

                        if (!peptide.Contains(aa))
                        {
                            // the amino acid isn't in the peptide, so go to the next PSMs
                            continue;
                        }

                        int numPresent = peptide.Count(x => x.ToString() == aa);
                        KTotalSites += numPresent;
                        LabelingSites[aa] += numPresent;

                        if (hit.Element("protein").Element("peptide").Element("domain").Elements("aa") != null)
                        {
                            numLabeled = (from x in hit.Element("protein").Element("peptide").Element("domain").Elements("aa")
                                          where x.Attribute("modified").Value == massToLookFor & x.Attribute("type").Value == aa
                                          select 1).Count();

                            if (Modifications[aa] == Modifications["["])
                            {
                                // the K mod is the same as the N-mod (this is probably always the case)
                                if (numLabeled > 1 & hit.Element("protein").Element("peptide").Element("domain").Attribute("seq").Value[0] == 'K')
                                {
                                    // we want to see if there are two labels or not
                                    int nTermKs = (from x in hit.Element("protein").Element("peptide").Element("domain").Elements("aa")
                                                   where x.Attribute("modified").Value == massToLookFor & x.Attribute("type").Value == aa & x.Attribute("at").Value == hit.Element("protein").Element("peptide").Element("domain").Attribute("start").Value
                                                   select 1).Count();
                                    if (nTermKs == 1)
                                    {
                                        // there's only one label, so give it to the n-term, not lysine
                                        numLabeled -= 1;
                                    }
                                }
                            }

                        }

                        if (numPresent == numLabeled)
                        {
                            // everything is labeled, so continue to the next
                            continue;
                        }
                        else
                        {
                            missed += (numPresent - numLabeled);
                        }
                    }

                    MissedMods.Add(aa, missed);
                    KTotalMissed += missed;
                }
            }

            // spit out some metrics to the console
            Console.WriteLine("Total N-term sites: {0}", NTotalSites);
            Console.WriteLine("Total K sites: {0}", KTotalSites);
            Console.WriteLine("Total {0} sites: {1}", xMod, XTotalSites);
            foreach (var key in MissedMods.Keys)
            {
                Console.WriteLine("Missed labels at {0}: {1}", key, MissedMods[key]);
            }

            // calculate labelling efficiency for each site
            foreach (var aa in LabelingSites.Keys)
            {
                double efficiency = Convert.ToDouble(LabelingSites[aa] - MissedMods[aa]) / LabelingSites[aa];
                LabelingEfficiency.Add(aa, efficiency);
                Console.WriteLine("Labeling efficiency at {0}: {1}", aa, efficiency);

                // if the sites are n-term or K add them to their own attributes
                if (aa == "[")
                {
                    qcData.LabelingEfficiencyAtNTerm = efficiency;
                }
                else
                {
                    if (aa == "K")
                    {
                        qcData.LabelingEfficiencyAtK = efficiency;

                    }
                    else
                    {
                        qcData.LabelingEfficiencyAtX = efficiency;
                        qcData.LabelX = aa;
                    }
                }
            }
        }

        public static void GetModificationFrequencyIdentipy(this QcDataContainer qcData, XElement searchSummary, IEnumerable<XElement> PSMs, XNamespace nsp, string nmod, string kmod, string xmod)
        {
            // get the modification mass and location out of qmods
            Dictionary<string, string> Modifications = new Dictionary<string, string>();
            Dictionary<string, int> LabelingSites = new Dictionary<string, int>();
            Dictionary<string, double> LabelingEfficiency = new Dictionary<string, double>();
            Dictionary<string, int> MissedMods = new Dictionary<string, int>();
            string xMod = null;

            string[] Mods = new string[] { nmod, kmod, xmod };

            foreach (var item in Mods)
            {
                if (item == null)
                {
                    continue;
                }
                var splitString = item.Split('@');
                // add the key: value pairs as AA:mass
                Modifications.Add(splitString.Last(), splitString.First());
            }

            // now we need to get at labeling efficiency
            int KTotalSites = 0;
            int KTotalMissed = 0;
            int NTotalSites = 0;
            int NTotalMissed = 0;
            int XTotalSites = 0;
            int XTotalMissed = 0;
            foreach (string aa in Modifications.Keys)
            {
                // if the modification is to the n-terminus, this is Nmod
                if (aa == "[")
                {
                    string massToLookFor = (from x in searchSummary.Elements(nsp + "terminal_modification")
                                            where x.Attribute("terminus").Value == "n"
                                            select x.Attribute("mass").Value).FirstOrDefault();

                    int missed = (from x in PSMs.Elements(nsp + "modification_info")
                                  where x?.Attribute("mod_nterm_mass")?.Value != massToLookFor
                                  select 1).Count();

                    MissedMods.Add(aa, missed);
                    NTotalMissed = missed;
                    NTotalSites = PSMs.Count();
                    LabelingSites.Add(aa, PSMs.Count());
                }

                // if the modification is to the c-terminus, this is an X
                if (aa == "]")
                {
                    xMod = "]";
                    string massToLookFor = (from x in searchSummary.Elements(nsp + "terminal_modification")
                                            where x.Attribute("terminus").Value == "c"
                                            select x.Attribute("mass").Value).FirstOrDefault();

                    int missed = (from x in PSMs.Elements(nsp + "modification_info")
                                  where x?.Attribute("mod_cterm_mass")?.Value != massToLookFor
                                  select 1).Count();

                    MissedMods.Add(aa, missed);
                    XTotalMissed = missed;
                    XTotalSites = PSMs.Count();
                    LabelingSites.Add(aa, PSMs.Count());
                }

                // if the modification is not to the c-terminus, n-terminus or K, this is X
                if ((aa != "[") & (aa != "]") & (aa != "K"))
                {
                    xMod = aa;
                    string massToLookFor = (from x in searchSummary.Elements(nsp + "aminoacid_modification")
                                            where x.Attribute("aminoacid").Value == aa
                                            select x.Attribute("mass").Value).FirstOrDefault();

                    int missed = 0;

                    LabelingSites.Add(aa, 0);

                    foreach (var hit in PSMs)
                    {
                        int numLabeled = 0;
                        string peptide = hit.Attribute("peptide").Value;

                        if (!peptide.Contains(aa))
                        {
                            // the amino acid isn't in the peptide, so go to the next PSMs
                            continue;
                        }

                        int numPresent = peptide.Count(x => x.ToString() == aa);
                        XTotalSites += numPresent;
                        LabelingSites[aa] += numPresent;

                        if (hit.Element(nsp + "modification_info")?.Elements(nsp + "mod_aminoacid_mass") != null)
                        {
                            numLabeled = (from x in hit.Elements(nsp + "modification_info").Elements(nsp + "mod_aminoacid_mass")
                                          where x.Attribute("mass").Value == massToLookFor
                                          select 1).Count();
                        }

                        if (numPresent == numLabeled)
                        {
                            // everything is labeled, so continue to the next
                            continue;
                        }
                        else
                        {
                            missed += (numPresent - numLabeled);
                        }
                    }

                    MissedMods.Add(aa, missed);
                    XTotalMissed = missed;
                }

                // if the modification is to K, this is Kmod
                if (aa == "K")
                {
                    string massToLookFor = (from x in searchSummary.Elements(nsp + "aminoacid_modification")
                                            where x.Attribute("aminoacid").Value == aa
                                            select x.Attribute("mass").Value).FirstOrDefault();

                    int missed = 0;

                    LabelingSites.Add(aa, 0);

                    foreach (var hit in PSMs)
                    {
                        int numLabeled = 0;
                        string peptide = hit.Attribute("peptide").Value;

                        if (!peptide.Contains(aa))
                        {
                            // the amino acid isn't in the peptide, so go to the next PSMs
                            continue;
                        }

                        int numPresent = peptide.Count(x => x.ToString() == aa);
                        KTotalSites += numPresent;
                        LabelingSites[aa] += numPresent;

                        if (hit.Element(nsp + "modification_info")?.Elements(nsp + "mod_aminoacid_mass") != null)
                        {
                            numLabeled = (from x in hit.Elements(nsp + "modification_info").Elements(nsp + "mod_aminoacid_mass")
                                          where x.Attribute("mass").Value == massToLookFor
                                          select 1).Count();
                        }

                        if (numPresent == numLabeled)
                        {
                            // everything is labeled, so continue to the next
                            continue;
                        }
                        else
                        {
                            missed += (numPresent - numLabeled);
                        }
                    }

                    MissedMods.Add(aa, missed);
                    KTotalMissed += missed;
                }
            }

            // spit out some metrics to the console
            Console.WriteLine("Total N-term sites: {0}", NTotalSites);
            Console.WriteLine("Total K sites: {0}", KTotalSites);
            Console.WriteLine("Total {0} sites: {1}", xMod, XTotalSites);
            foreach (var key in MissedMods.Keys)
            {
                Console.WriteLine("Missed labels at {0}: {1}", key, MissedMods[key]);
            }

            // calculate labelling efficiency for each site
            foreach (var aa in LabelingSites.Keys)
            {
                double efficiency = Convert.ToDouble(LabelingSites[aa] - MissedMods[aa]) / LabelingSites[aa];
                LabelingEfficiency.Add(aa, efficiency);
                Console.WriteLine("Labeling efficiency at {0}: {1}", aa, efficiency);

                // if the sites are n-term or K add them to their own attributes
                if (aa == "[")
                {
                    qcData.LabelingEfficiencyAtNTerm = efficiency;
                }
                else
                {
                    if (aa == "K")
                    {
                        qcData.LabelingEfficiencyAtK = efficiency;

                    }
                    else
                    {
                        qcData.LabelingEfficiencyAtX = efficiency;
                        qcData.LabelX = aa;
                    }
                }
            }
        }

        /*
        public static void GetLabelingEfficiency(this QcDataContainer qcData, XElement searchSummary, IEnumerable<XElement> PSMs, XNamespace nsp, string qmods)
        {
            // get the modification mass and location out of qmods
            Dictionary<string, string> QMods = new Dictionary<string, string>();
            Dictionary<string, int> labelingSites = new Dictionary<string, int>();
            Dictionary<string, double> labelingEfficiency = new Dictionary<string, double>();
            Dictionary<string, int> missedTags = new Dictionary<string, int>();

            string[] qMods = qmods.Split(',');

            foreach (var item in qMods)
            {
                var splitString = item.Split('@');
                // add the key: value pairs as AA:mass
                QMods.Add(splitString.Last(), splitString.First());
            }

            // now we need to get at labeling efficiency
            int totalSites = 0;
            int totalMissed = 0;
            foreach (string aa in QMods.Keys)
            {
                // if the modification is to the n-terminus
                if (aa == "[")
                {
                    string massToLookFor = (from x in searchSummary.Elements(nsp + "terminal_modification")
                                            where x.Attribute("terminus").Value == "n"
                                            select x.Attribute("mass").Value).FirstOrDefault();

                    int missed = (from x in PSMs.Elements(nsp + "modification_info")
                                  where x?.Attribute("mod_nterm_mass")?.Value != massToLookFor
                                  select 1).Count();

                    missedTags.Add(aa, missed);
                    totalMissed += missed;
                    totalSites += PSMs.Count();
                    labelingSites.Add(aa, PSMs.Count());
                }
                // if the modification is to the c-terminus
                if (aa == "]")
                {
                    string massToLookFor = (from x in searchSummary.Elements(nsp + "terminal_modification")
                                            where x.Attribute("terminus").Value == "c"
                                            select x.Attribute("mass").Value).FirstOrDefault();

                    int missed = (from x in PSMs.Elements(nsp + "modification_info")
                                  where x?.Attribute("mod_cterm_mass")?.Value != massToLookFor
                                  select 1).Count();

                    missedTags.Add(aa, missed);
                    totalMissed += missed;
                    totalSites += PSMs.Count();
                    labelingSites.Add(aa, PSMs.Count());
                }
                // if the modification is to any other amino acid
                if (aa != "[" & aa != "]")
                {
                    string massToLookFor = (from x in searchSummary.Elements(nsp + "aminoacid_modification")
                                            where x.Attribute("aminoacid").Value == aa
                                            select x.Attribute("mass").Value).FirstOrDefault();

                    int missed = 0;

                    labelingSites.Add(aa, 0);

                    foreach (var hit in PSMs)
                    {
                        int numLabeled = 0;
                        string peptide = hit.Attribute("peptide").Value;

                        if (!peptide.Contains(aa))
                        {
                            // the amino acid isn't in the peptide, so go to the next PSMs
                            continue;
                        }

                        int numPresent = peptide.Count(x => x.ToString() == aa);
                        totalSites += numPresent;
                        labelingSites[aa] += numPresent;

                        if (hit.Element(nsp + "modification_info")?.Elements(nsp + "mod_aminoacid_mass") != null)
                        {
                            numLabeled = (from x in hit.Elements(nsp + "modification_info").Elements(nsp + "mod_aminoacid_mass")
                                          where x.Attribute("mass").Value == massToLookFor
                                          select 1).Count();
                        }

                        if (numPresent == numLabeled)
                        {
                            // everything is labeled, so continue to the next
                            continue;
                        }
                        else
                        {
                            missed += (numPresent - numLabeled);
                        }
                    }

                    missedTags.Add(aa, missed);
                    totalMissed += missed;
                }
            }

            // spit out some metrics to the console
            Console.WriteLine("Total labeling sites: {0}", totalSites);
            foreach (var key in missedTags.Keys)
            {
                Console.WriteLine("Missed labels at {0}: {1}", key, missedTags[key]);
            }
            Console.WriteLine("Overall labeling efficiency: {0}", Convert.ToDouble(totalSites - totalMissed) / totalSites);

            // calculate labelling efficiency for each site
            foreach (var aa in labelingSites.Keys)
            {
                double efficiency = Convert.ToDouble(labelingSites[aa] - missedTags[aa]) / labelingSites[aa];
                labelingEfficiency.Add(aa, efficiency);
                Console.WriteLine("Labeling efficiency at {0}: {1}", aa, efficiency);

                // if the sites are n-term or K add them to their own attributes
                if (aa == "[")
                {
                    qcData.LabelingEfficiencyAtNTerm = efficiency;
                }
                if (aa == "K")
                {
                    qcData.LabelingEfficiencyAtK = efficiency;

                }
            }

            qcData.LabelingEfficiency = Convert.ToDouble(totalSites - totalMissed) / totalSites;
        }
        */
        public static void ExampleMods()
        {
            StringBuilder examples = new StringBuilder();
            examples.AppendLine("\n");
            examples.AppendLine("-----------------------------");
            examples.AppendLine("Peptide modification examples");
            examples.AppendLine("-----------------------------");

            examples.AppendLine("\nThere are four arguments which may be used on the command line to specify peptide modifications: " +
                "fmods, nmod, kmod, xmod. RawTools passes these modifications to IdentiPy is a search is performed. " +
                "fmods can be any number of fixed modifications, separated by a comma (an no spaces). The other three are always " +
                "variable, and modification frequency (e.g. labeling efficiency) is calculated for each. nmod specifies a modification to the peptide N-terminus, kmod " +
                "specifies a modification to lysine, and xmod can specify a modification to any other residue. The peptide modifications which RawTools passes to " +
                "IdentiPy must be in mass@aa format. Examples of common modifications are given below.\n");

            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Oxidation of methionine:", "15.99491@M");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Carboxyamidomethylation of cysteine:", "57.02146@C");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Acetylation of (peptide) N-terminus:", "42.01056@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Phosphorylation at X:", "79.96633@X");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT0 label at K or N-terminus:", "224.15247@K,224.15247@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT2 label at K or N-terminus:", "225.15583@K,225.15583@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT6+ label at K or N-terminus:", "229.16293@K,229.16293@[");

            examples.AppendLine("\nFor example, to specify fixed CamC, and variable oxidation of M and TMT10 quant labels, you would invoke the following arguments:");
            examples.AppendLine("\t--fmods 57.02146@C --nmod 229.16293@[ --kmod 229.16293@K --xmod 15.99491@M");

            examples.AppendLine("\nOr to include the TMT10 labels as fixed modifications, you could invoke the following:");
            examples.AppendLine("\t--fmods 57.02146@C,229.16293@K,229.16293@[ --xmod 15.99491@M");

            Console.Write(examples);

        }
    }
}
