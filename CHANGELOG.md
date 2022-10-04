# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.0.1] - 2022-10-04

### Fixed
- Fixed a versioning issue with Open UPM

## [3.0.0] - 2022-10-03

### Changed
- Changed the project strucute and layout to make it comply with UPM package conventions.
- Restructured the repository to make it compatible with UPM.
- Reworked IL2CPP type defintion generation.

### Added
- Static struct member can now be monitored.
- Added multiple assemblies to be ignored during profiling by default. (List is part of the settings file)
- Added package manager dependencies for com.baracuda.thread-dispatcher

### Fixed
- Fixed an IL2CPP type defintion issue caused by missing support for generic monitored types. 

### Removed
- Removed obsolete scripts, API and assemblies. 
- Removed .unitpackages for UI assets. The same assets can now be imported via the samples section in the package manager window.


## [2.1.5] - 2022-09-24

### Fixed
- Fixed some IL2CPP type generation issues caused by potentially inaccessbile type generations beeing generated.
- Fixed some potential runtime exceptions caused by missing generic type checks.


## [2.1.4] - 2022-08-20

### Fixed
- Fixed ExecutionEngineException (missing AOT code) that occurred in IL2CPP builds for custom IList{T} value processors.
- Improved value processor warning messages.


## [2.1.3] - 2022-08-20

### Fixed
- Fixed an IL2CPP build issue caused by a missing namespace.


## [2.1.2] - 2022-08-16

### Changed
- Refactored internal classes and removed unused code.


## [2.1.1] - 2022-08-14

### Changed
- Deprecated Assembly-Baracuda-Pooling.
- Deprecated Assembly-Baracuda-Reflection.
- Scripts from obsolete assemblies are now contained in Assembly-Baracuda-Utilties.
- Note that obsolete asselbies and their contents can be removed and only exist because of update compatibility.
- The first MonitoringUIController instance in a scene will be used as the active MonitoringUIController. Only if no instance is located in a scene the selected prefab from the settings window is instantiated and used.

### Added
- Interface monitoring. Interface member can now be used as a target for any monitor attribute.

### Fixed
- Fixed the scale of some TMP UI elements
- Fixed an issue caused by unsafe code usage.


## [2.1.0] - 2022-08-05

### Changed
- Reworked the systems core structure. Systems are now managed and communicate using a Service Locator Pattern.
- Old static API is now obsolete.
- Refactored and moved internal classes and namespaces.
- Reworked how events are monitored. (more options, coloring & subscriber display)
- Reworked AOT file generation.
- Reworked dynamic filering of monitored member.
- Reworked and optimized TMP & UIToolkit controller.

### Added
- Methods can now be monitored with support for out parameter.
- Added filtering options to the settings window. The example scene showcases new capabilities of the filtering system.
- Global value processor methods can be declared by adding the [GlobalValueProcessorAttribute] to a static method with a valid signature.
- Added validation logic to dynamically enable / disable monitored member during runtime.
- Added new MetaAttributes & Formatting options.
- Added attributes for custom text, background & group coloring.
- Added MFontAttribute to set the font asset for a monitored unit.
- Added multiple smaller options like text align etc.
- Added ConsoleMonitor Prefab as a preset to easily monitor console logs during runtime.
- Added SystemMonitor Prefab as a preset to easily visualize information about the current system during runtime.
- Added MColorAttribute to set the color of a monitored unit.
- Added profiling data to AOT type generation. AOT file now contains additional information about monitored types, member etc.
- Added multiple font assets.

### Fixed
- Fixed an issue that was caused when monitoring constant fields.
- Fixed coditonal compilation using DISABLE_MONITORING
- Fixed WebGL runtime issues caused by WebGL initialization order.