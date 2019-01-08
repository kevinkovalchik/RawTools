# Changelog
All notable changes to RawTools will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.4.0] 2019-01-08
### Changed
- We have streamlined the processing workflows to improve performance, and RawTools can now run on multiple processors. Hopefully this will speed up the processing of very large files. [[issue6]](https://github.com/kevinkovalchik/RawTools/issues/6)
- Occasionally there are scans with corrupt headers or which are otherwise not linked to their precursor/dependent scan(s) (e.g. a MS2 scan which has no master scan associated with it in the file). This is pretty rare, but when it comes up RawTools now detects such "orphan scans" and prints out a report to the console letting you know which scans are affected. These scans are not included in the output tables, so you can check them manually if desired to make sure they are not critical to your analysis.
- Relative paths are now supported for all arguments.
- Precursor peaks are now extracted based on picked m/z rather than monoisotopic m/z.
- The next release will add a new "logdump" function, which outputs the raw file instrument log to a text file. Use it like this:
`RawTools logdump -f [path to raw file]`
`RawTools logdump -d [directory in which to get log files from all raw files]`
Pretty much everything about how the instrument was operating is in there, scan by scan, from capillary temperature to lens voltages to turbopump lifetimes, etc.

### Fixed
- Previously the total number of scans reported in the metrics was incorrect. This has been fixed, though in the event of orphan scans (as described above) those scans will not be counted towards the total.
- MGF files are now compatible with MS Amanda.

## [1.3.3] 2018-11-23
### Changed
- Users with heightened Windows security settings might have difficulty running RawTools since it is acquired via internet download. The
solution has been to modify the RawTools.exe.config file to allow the app to load assemblies from remote locations. This has come up enough that we've
decided to make the change permanent. [[issue5]](https://github.com/kevinkovalchik/RawTools/issues/5)

###Fixed
- The program will now give a warning when a filename provided via -f does not exist (as opposed to simply crashing).

## [1.3.2] 2018-11-09
### Changed
- We came across a file with an apparently corrupted MS3 scan in it which was cause RawTools to crash. The MS data for the scan was in the raw file,
but the meta data was incomplete. To address this, the scan lists RawTools uses to process are now built off of the scan dependents found in the
raw file meta data. This means scans which are missing meta data will not appear in the output data table. Note that these occurances are rare, and
we are planning to add a text output to alert the user when this happens.
- The examples have been updated. There are now two sections to the examples function: --interface and --modifications.

### Fixed
- MS3ScanRate was not referencing the correct data. Is fixed now.
- Fixed a bug which caused a crash when MS scan data was absent or corrupted for a scan.
- Fixed a bug where precursor charge state and monoisotopic m/z was not refined when the only output was an MGF file.

## [1.3.1] 2018-10-26
### Fixed
- Fixed a QC bug in which the -N argument was always used as the number of ms2 spectra, even if it was greater than the actual number of spectra in the raw file.
- Fixed a bug in the master scan index would not be found for some instruments or Tune versions.

## [1.3.0] 2018-10-09
### Fixed
- RawTools is unable to properly process files from very short duration acquistions. Problems seem to often come up with handeling precursor peaks. As a quick workaround, metrics are not reported for precursor peaks when this issue occurs. A future fix will address the problem more thoroughly. [[issue2]](https://github.com/kevinkovalchik/RawTools/issues/2)

### Added
- MS1 isolation interference calculations added to parse and qc outputs.
- Refinement of precursor charge state and monoisotopic mass. This can be invoked using `-R` as an argument. This can be important if monoisotopic precursor selection is turned off in your instrument method. The refinement works by creating a list of possible charge states for each precursor based upon isotope neighbors, then generating a set of averagine isotope envelopes representing the possible monoisotopic masses of the precursor m/z value and nearby isotope peaks. Scoring is based on the [Bhattacharyya distance](https://en.wikipedia.org/wiki/Bhattacharyya_distance) between the isotope envelope of the observed spectrum and the theoretical averagine spectrum.

## [1.2.0] 2018-09-24
### Added
- Added support for X! Tandem as a search engine.
- Chromatograms of raw files can be exported as tab-delimited text files using `--chro` as an argument. See `RawTools.exe parse --help` for more info.

### Deprecated
- The `-i` argument for invoking an IdentiPy search has been deprecated and will not be available in future releases. Instead, please use `-s identipy` to
invoke an IdentiPy search. Other IdentiPy-related arguments will remain as-is.

### Changed
- The code for parsing IdentiPy results and calculating identification-based metrics has been reworked to facilitate compatibility with X!Tandem results files.
This should have no impact upon end-users, but will make it easier to add support for other search engines in the future if it is desired.

## [1.1.1] 2018-09-14
### Fixed
- Fixed a bug which occurs when no variable modifications are specified.

## [1.1.0] 2018-09-13
### Added
- Everything! But that isn't very informative, so check out the [wiki page](https://github.com/kevinkovalchik/RawTools/wiki) to learn about RawTools.
