# Overview

A WPF desktop application that exercises a larger surface area of the Carbon API.

This example project is for software developers who are interested in using the Carbon API to build a more comprehensive and realistic application.

Carbon is primarily promoted as a library that generates cross-tabulation reports, but it has an API that covers the following broader functionality.

- *Jobs* in the local filesystem or Azure Cloud storage can be listed, opened and closed.
- Data can be imported and exported in a variety of standard formats.
- Variables of different types can be defined, categorised and listed.
- Reports can be listed, generated and saved.

Software developers can combine this functionality to create complete application frameworks or services that manage cross-tabulation in a custom business environment.

This example project is for a Windows desktop application that shows how the Carbon API can be used to build a simple but practical UI that covers many aspects of managing Carbon *jobs* and reports. Developers may extend this project or steal parts of it to create their own custom application over the Carbon library.

## Design Principle

The Visual Studio 2022 project is a [WPF][wpf] application targeting .NET Framework 4.8.

> :star: Note that the Carbon library can be used from any application targeting .NET Standard 2.0, .NET Framework 4.7.1 and later, and .NET 6.0. This example project only targets 4.8 because it was convenient when the project was created. It may be converted to 6.0 in the near future.

The project and class structure follows a simple [MVVM][mvvm] pattern that separates the UI code from the data and business logic of the application. The UI is defined in standard XAML and most of the controls are bound to properties of a single *controller* class of notify properties and observable collections. The developer community uses a variety of MVVM coding styles, so this project attempts to use the simplest classical style possible.

The `MainController` class is mostly self-contained and acts like a simple [state machine][statem]. Most methods of the controller simply return True or False to indicate success, but internally a set of binding property values will be set so that the UI can respond accordingly.

Because there are large numbers of notify binding properties, a [T4 Text Template][t4] is used to generate the repetitive code into a `MainController` partial class.

[wpf]: https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf?view=vs-2022
[mvvm]: https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel
[statem]: https://en.wikipedia.org/wiki/Finite-state_machine
[t4]: https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates?view=vs-2022