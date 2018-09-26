# RawTools

Welcome to the RawTools page! RawTools is an open-source and freely available package designed to perform scan data parsing and quantification, and quality control analysis of Thermo Orbitrap raw mass spectrometer files. RawTools is written in C# and uses the Thermo RawFileReader library. RawTools is fully compatible with Windows, Linux, and MacOS operating systems. RawTools is the replacement for the previously described [RawQuant](https://github.com/kevinkovalchik/RawQuant) Python package. 

The RawTools manuscript preprint is [available on bioRxiv](https://www.biorxiv.org/content/early/2018/09/15/418400).

<br>

### RawTools releases

To obtain the latest compiled release of RawTools or to obtain an older version, please visit the [Releases page](https://github.com/kevinkovalchik/RawTools/releases). 

<br>

### Notable news

* **Edit** - The Shiny application is temporarily down for some updates. [2018-09-26] 

* The RawTools Shiny application has been revamped to use a new interface and to provide functionality for plotting of 'Matrix' and 'chromatogram' outputs. Matrix files are generated using the RawTools parse functionality and can be useful for visualizing data temporally across the acquisition. Chromatogram files are also generated using the parse functionality, with the `--chro` flag. The new application is currently available on GitHub [from here](https://github.com/kevinkovalchik/RawTools/tree/master/documentation/manuscript/RawTools_RShiny_Application). [2018-09-26]

* RawTools now offers support for X! Tandem in as a search engine to facilitate calculation of identification-related metrics. IdentiPy remains available and the user may decide which search engine they wish to use. We are working on updating the help documentation to reflect this addition. [2018-09-24]

<br>

### Getting help

If you need help getting started with RawTools or with understanding the output, please visit the [RawTools Wiki](https://github.com/kevinkovalchik/RawTools/wiki).

If you have noticed an error in RawTools or have an idea for a new feature, please make a post on the [Issues page](https://github.com/kevinkovalchik/RawTools/issues).

<br>

### RawTools R Shiny page

If you are just here looking for the web application page for visualizing your quality control results: [RawTools R Shiny web application](https://rawtoolsqcdv.bcgsc.ca/). You can also download the application [from here](https://github.com/kevinkovalchik/RawTools/tree/master/documentation/manuscript/RawTools_RShiny_Application) for local use on your own machine.

<br>

### Staying up to date

If you want to see what features we have in mind or are actively working on integrating into RawTools, visit the [Projects page](https://github.com/kevinkovalchik/RawTools/projects/2) for more details.

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
