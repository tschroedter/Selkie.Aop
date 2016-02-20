# Selkie.Aop

An implementation of an Ant Colony Optimization algorithm using a generic algorithm to improve the quality of the result. The implementation is heavily customized and focused on easy to understand code which can be extended by anyone. - Worst code compared to the other projects, but everything is covered by unit tests!

### Selkie Projects

* [Selkie Aco] (https://github.com/tschroedter/Selkie.Aco)
An implementation of an Ant Colony Optimization algorithm using a generic algorithm to improve the quality of the result. The implementation is heavily customized and focused on easy to understand code which can be extended by anyone. - Worst code compared to the other projects, but everything is covered by unit tests!

* [Selkie.Aop](https://github.com/tschroedter/Selkie.Aop)
This package contains aspect oriented programming extensions for the Selkie project: LogAspect, MessageHandlerAspect and PublishExceptionAspect.

* [Selkie Common] (https://github.com/tschroedter/Selkie.Common)
This package contains common classes and interfaces to replace some .Net classes: IRandom, IDateTime, ITimer, ITimerFactory, IDisposer, ISelkieConsole…

* [Selkie EasyNetQ] (https://github.com/tschroedter/Selkie.EasyNetQ)
Selkie EasyNetQ hides the EasyNetQ package. It also contains extensions and installers for Selkie EasyNetQ. There are classes to subscribe to messages sync/async, a logger, a bus and other helpful classes to send and receive messages with RabbitMQ.

* [Selkie Geometry] (https://github.com/tschroedter/Selkie.Geometry)
This package contains classes for 2D geometry calculations. It defines primitives like angle and distance. Shapes like circles, lines, arc segments and polylines. Multiple calculators e.g. to find intersection points and tangents. The package is used in the racetracks calculation. – A lot of math is implemented here not everything is ‘clean-code’ but as good as possible. - The second worst package!

* [Selkie NUnit Extensions] (https://github.com/tschroedter/Selkie.NUnit.Extensions)
A small package containing NUnit extensions: NUnitHelper and StringExtensions.

* [Selkie Racetrack] (https://github.com/tschroedter/Selkie.Racetrack)
This package contains classes to calculate a racetrack from one point two another using a minimum turn circle. The U-turn option is included in the calculation and will be used if it's the only or shortest option. The calculation is based on triangles.

* [Selkie Services ACO] (https://github.com/tschroedter/Selkie.Services.Aco)
This package contains the ant colony service which finds the shortest path for a given cost matrix and cost per line. - If you want to look at nice clean tests check-out all the services.

* [Selkie Services ACO Common] (https://github.com/tschroedter/Selkie.Services.Aco.Common)
This package contains the Aco service common messages.

* [Selkie Services Common] (https://github.com/tschroedter/Selkie.Services.Common)
This package contains common classes used by Selkie services, e.g. ServiceConsole, ServiceMain, ServiceManager... - Makes writing a Selkie service very easy.

* [Selkie Services Common Dto] (https://github.com/tschroedter/Selkie.Services.Common.Dto)
This package contains the common DTOs used in different services.

* [Selkie Services Lines] (https://github.com/tschroedter/Selkie.Services.Lines)
This package contains the Lines service which provides some test lines and later will support importing/exporting GeoJSON.

* [Selkie Services Lines Common] (https://github.com/tschroedter/Selkie.Services.Lines.Common)
This package contains the common messages published and received by the lines service.

* [Selkie Services Monitor] (https://github.com/tschroedter/Selkie.Services.Monitor)
The monitor service starts all the required Selkie services: Aco, Lines and Racetrack. The monitor is a simple console application which starts the other services as separate processes. – This is easier for development than installing and updating Windows services all the time.

* [Selkie Services Racetracks] (https://github.com/tschroedter/Selkie.Services.Racetracks)
The racetracks service calculates all the possible racetracks for all lines forward and reverse to all lines forward and reverse. The result is a distance graph used by the Aco service.  

* [Selkie Services Racetracks Common] (https://github.com/tschroedter/Selkie.Services.Racetracks.Common)
This package contains the common messages published and received by the racetracks service.

* [Selkie.Scripts (Bamboo) ](https://github.com/tschroedter/Selkie.Scripts)
A lot of PowerShell scripts to build all the Selkie projects on a Bamboo build server.

* [Selkie.Tools (Bamboo)] (https://github.com/tschroedter/Selkie.Tools)
A bunch of custom tools to update assembly version, NuGet spec files and other things.

* Selkie Web

* [Selkie Windsor] (https://github.com/tschroedter/Selkie.Windsor)
This package is an extension to Castle Windsor and provides class attributes to mark and load classes automatically as singleton, transient or start-able. It also contains an ITypedFactory interface which loads interfaces inheriting from it as factories at start-up.
The packages include different installers and a loader class. The loader class finds all classes using the custom class attributes and registers the class according to the attribute. The included base installer is used to create a loader class and run the registration. - *If you only want to look at one ‘only’ package look at this one. It makes dependency inversion such much simpler!*

* [Selkie WPF] (https://github.com/tschroedter/Selkie.WPF)
The first try to show the results in some way. I used a simple WPF application which is implemented like the rest: SOLID, Lean and Dry. – Not a fancy UI!  Not very user friendly! - But does the job of showing the settings, lines, racetracks and history.

* [Selkie XUnit Extensions] (https://github.com/tschroedter/Selkie.XUnit.Extensions)
This small package contains XUnit extensions: AutoNSubstituteDataAttribute and InlineAutoNSubstituteDataAttribute.
AutoNSubstituteDataAttribute
