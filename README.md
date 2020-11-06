# RawTools

Welcome to the RawTools page! RawTools is an open-source and freely available package designed to perform scan data parsing and quantification, and quality control analysis of Thermo Orbitrap raw mass spectrometer files. RawTools is written in C# and uses the Thermo RawFileReader library. RawTools is fully compatible with Windows, Linux, and MacOS operating systems. RawTools is the replacement for the previously described [RawQuant](https://github.com/kevinkovalchik/RawQuant) Python package. 

The RawTools manuscript was just accepted for the Journal of Proteome Research special issue on Software Tools and Resources, [here](https://pubs.acs.org/doi/10.1021/acs.jproteome.8b00721). The RawTools original manuscript preprint is [available on bioRxiv](https://www.biorxiv.org/content/early/2018/09/15/418400).

<br>

### RawTools releases and installation

To obtain the latest compiled release of RawTools or to obtain an older version, please visit the [Releases page](https://github.com/kevinkovalchik/RawTools/releases). 

If you need help installing and using RawTools, please visit the wiki page appropriate for your system and follow the instructions outlined [here](https://github.com/kevinkovalchik/RawTools/wiki).

<br>

### RawTools 2.0.3 released

- 2.0.3 brings functionality for processing TMTPro data.

### RawTools 2.0.2 released

- 2.0.0 and 2.0.1 contained a bug which crashed the command line help in Linux and MacOS. This is now fixed.

### RawTools 2.0.1 released

- 2.0.0 contained a fatal bug which prevented RawToolsViz from running. This is now fixed!
- Note that the .raw file viewer is working on Linux systems, but the QC and parse viewers are having some issues on Linux. We will try to
address this in the next few weeks.

### RawTools 2.0.0 released

- A lot has changed since v1.4.2!
- The command line interface has been updated and streamlined. While this breaks compatibility with old versions, we think the improvements
are worth it. Parsing and QC are no longer separate operations, but can be performed at the same time. As such, there are no longer "parse"
and "qc" modes to invoke. We have made efforts to preserve most of the original arguments as they were, so there shouldn't be too much to
update if you use RawTools as part of a script. For help with this new interface, see `>RawTools.exe -commands`.
- We have now integrated data visualization into the tool itself (see examples below). The new RawToolsViz.exe allows for visualization of QC and parse data
and RawTools-generated chromatograms, and exploration of Thermo .raw files themselves. While the .raw file exploration has only been
extensively tested in Windows, it should be compatible with Linux and Mac systems as well.
- Some metrics have been renamed to align the naming schemes used in the various data files (parse, qc, metrics). Because of this, the 
R Shiny app may not work anymore. The Shiny app will no longer be updated and will be taken offline in the near future, as visualization can now be carried out locally with RawToolsViz.
- New metrics have been added to QC output, including the number of cycles across the average peak profile, median peptide hyperscore
and cutoff hyperscore used for FDR.
- IdentiPy is no longer supported as a search engine. While we don't have any issues with IdentiPy, we felt it was easiest to maintain
support for only a single search engine. As X! Tandem does not have any external dependencies (i.e. Python), we felt it was the best choice for this particular case.
- We will be working to update the wiki over the next week to reflect changes in v2.0.0.

#### RawToolsViz parse visualization
<img src="https://github.com/kevinkovalchik/RawTools/blob/master/documentation/parseFullWindow.png" width="50%">

#### RawToolsViz qc visualization
<img src="https://github.com/kevinkovalchik/RawTools/blob/master/documentation/qcFullWindow.png" width="50%">

#### RawToolsViz raw file visualization
<img src="https://github.com/kevinkovalchik/RawTools/blob/master/documentation/rawFileFullWindow.png" width="50%">

<br>

### Notable news

* RawTools 2.0.0 is [here!](https://github.com/kevinkovalchik/RawTools/releases/tag/2.0.0)

<br>

### RawTools R Shiny page

* NOTE: The [newest version of RawTools](https://github.com/kevinkovalchik/RawTools/releases/tag/2.0.0) contains its own tools for
data visualization. As such, it is unlikely that we will be continuing support for the R Shiny web application.

If you are just here looking for the web application page for visualizing your quality control results: [RawTools R Shiny web application](https://rawtoolsqcdv.bcgsc.ca/). You can also download the application [from here](https://github.com/kevinkovalchik/RawTools/tree/master/documentation/manuscript/RawTools_RShiny_Application) for local use on your own machine.

<br>

### Getting help

If you need help getting started with RawTools or with understanding the output, please visit the [RawTools Wiki](https://github.com/kevinkovalchik/RawTools/wiki).

If you have noticed an error in RawTools or have an idea for a new feature, please make a post on the [Issues page](https://github.com/kevinkovalchik/RawTools/issues).

<br>

### Staying up to date

If you want to see what features we have in mind or are actively working on integrating into RawTools, visit the [Projects page](https://github.com/kevinkovalchik/RawTools/projects) for more details.

<br>

### Modifying RawTools

If you want to modify the RawTools source code, you will need to get a copy of RawFileReader from ThermoFisher. Otherwise the project will not have access to RawFileReader and you won't be able to debug or compile any code you write. You need to request access to Thermo Fisher's file share site to do this. It is a pretty simple process: you just need to email and request access. Details for access and subsequent installation of the package in Visual Studio can be found on the [RawFileReader page on planetorbitrap.com](http://planetorbitrap.com/rawfilereader#.W6471U9lA0M).

Please make note of and respect the [RawFileReader license](https://github.com/kevinkovalchik/RawTools/blob/master/RawFileReaderLicense) in any modifications you make and wish to distribute.

<br>

### License and copyright

RawTools (Copyright 2018 Kevin Kovalchik & Christopher Hughes) is licensed under Apache 2.0

<br>

### Third-party licenses and copyright

RawFileReader reading tool. Copyright © 2016 by Thermo Fisher Scientific, Inc. All rights reserved. See [RawFileReaderLicense](https://github.com/kevinkovalchik/RawTools/blob/master/RawFileReaderLicense) for licensing information. 
Note: anyone recieving RawFileReader as part of a larger software distribution (in the current context, as part of RawTools) is considered an "end user" under 
section 3.3 of the RawFileReader License, and is not granted rights to redistribute RawFileReader.

CommandLineParser. Copyright 2005-2018 Giacomo Stelluti Scala & Contributors. See [CommandLineParserLicense](https://github.com/kevinkovalchik/RawTools/blob/master/CommandLineParserLicense) for licensing information.

Serilog. Copyright 2013-2015 Serilog Contributors. See [SerilogLicense](https://github.com/kevinkovalchik/RawTools/blob/master/SerilogLicense) for licensing information.
