# Changelog
All notable changes to RawTools will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Fixed
- RawTools is unable to properly process files from very short duration acquistions. Problems seem to often come up with handeling precursor peaks. As a quick workaround, metrics are not reported for precursor peaks when this issue occurs. A future fix will address the problem more thoroughly. [[issue2]](https://github.com/kevinkovalchik/RawTools/issues/2)

### Added
- MS1 isolation interference calculations added to parse and qc outputs.
- Refinement of precursor charge state and monoisotopic mass. This can be invoked using `-R` as an argument. This can be important if monoisotopic precursor selection is turned off in your instrument method.

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
