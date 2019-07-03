# RawTools

Welcome to the RawTools page! RawTools is an open-source and freely available package designed to perform scan data parsing and quantification, and quality control analysis of Thermo Orbitrap raw mass spectrometer files. RawTools is written in C# and uses the Thermo RawFileReader library. RawTools is fully compatible with Windows, Linux, and MacOS operating systems. RawTools is the replacement for the previously described [RawQuant](https://github.com/kevinkovalchik/RawQuant) Python package. 

The RawTools manuscript was just accepted for the Journal of Proteome Research special issue on Software Tools and Resources, [here](https://pubs.acs.org/doi/10.1021/acs.jproteome.8b00721). The RawTools original manuscript preprint is [available on bioRxiv](https://www.biorxiv.org/content/early/2018/09/15/418400).

<br>

### RawTools releases and installation

To obtain the latest compiled release of RawTools or to obtain an older version, please visit the [Releases page](https://github.com/kevinkovalchik/RawTools/releases). 

If you need help installing and using RawTools, please visit the wiki page appropriate for your system and follow the instructions outlined [here](https://github.com/kevinkovalchik/RawTools/wiki).

<br>

### RawTools 2.0.0 released

- A lot has changed since v1.4.2!
- The command line interface has been updated and streamlined. While this breaks compatibility with old versions, we think the improvements
are worth it. Parsing and QC are no longer separate operations, but can be performed at the same time. As such, there are no longer "parse"
and "qc" modes to invoke. We have made efforts to preserve most of the original arguments as they were, so there shouldn't be too much to
update if you use RawTools as part of a script. For help with this new interface, see RawTools -commands.
- We have now integrated data visualization into the tool itself (see examples below). The new RawToolsViz.exe allows for visualization of QC and parse data
and RawTools-generated chromatograms, and exploration of Thermo .raw files themselves. While the .raw file exploration has only been
extensively tested in Windows, it should be compatible with Linux and Mac systems as well.
- Some metrics have been renamed to align the naming schemes used in the various data files (parse, qc, metrics). Because of this, the 
hiny app may not work anymore. It is unlikely the app will be updated as visualization can now be carried out locally.
- New metrics have been added to QC output, including the number of cycles across the average peak profile, median peptide hyperscore
and cutoff hyperscore used for FDR.
- IdentiPy is no longer supported as a search engine. While we don't have any issues with IdentiPy, we felt it was easiest to maintain
support for only a single search engine. As X! Tandem does not have any external dependencies (i.e. Python), we felt it was the best choice
for this particular case.
- We will be working to update the wiki over the next week to reflect changes in v2.0.0.

#### RawToolsViz parse visualization
![](https://github.com/kevinkovalchik/RawTools/blob/master/documentation/parseFullWindow.png)

#### RawToolsViz qc visualization
![](https://github.com/kevinkovalchik/RawTools/blob/master/documentation/qcFullWindow.png)

#### RawToolsViz raw file visualization
![](https://github.com/kevinkovalchik/RawTools/blob/master/documentation/rawFileFullWindow.png)

<br>

### Notable news

* RawTools 2.0.0 ig [here!](https://github.com/kevinkovalchik/RawTools/releases/tag/2.0.0)

* Thanks to a lot of behind the scenes work, the file downloads of plots from the Shiny app are now working! [2018-12-06]  

* The R Shiny application now has greater support for plot customisation. We have also added the ability to highlight points on QC plots. Lastly, we have added a new 'multi-plot' page where multiple scan matrix files can be input at once and plots for all files will be generated alongside one another in the same space. The latest version is live on the web and available here.  [2018-11-29]

* The Shiny application has been updated to allow downloading of pdf versions of the created plots. These images can be imported to programs like Adobe Illustrator and will be treated as vector images. We have also added the ability to mouseover data points to see values, as well as corrected some bugs. We have also added the ability to add mean and median lines to the QC plots. The latest Shiny web interface is available [here](https://rawtoolsqcdv.bcgsc.ca/). [2018-11-07] 

* RawTools now offers support for X! Tandem in as a search engine to facilitate calculation of identification-related metrics. IdentiPy remains available and the user may decide which search engine they wish to use. We are working on updating the help documentation to reflect this addition. [2018-09-24]

<br>

### RawTools R Shiny page

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
