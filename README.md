# Overview

This repository contains a Windows [WPF][wpf] desktop application that demonstrates how to use the basic API calls of the [Carbon][carbon] cross-tabulation engine. This example project is for developers who are interested in using Carbon to build a more comprehensive application.

Carbon is primarily promoted as a library that generates cross-tabulation reports, but it has an API that covers the following broader functionality.

- Jobs in the local filesystem or Azure Cloud storage can be listed, opened and closed.
- Data can be imported and exported in a variety of standard formats.
- Variables of different types can be defined, categorised and listed.
- Reports can be listed, generated and saved.

Software developers can combine this functionality to create complete application frameworks or services that manage cross-tabulation in a custom business environment.

This example project is for a Windows desktop application that shows how the Carbon API can be used to build a simple but practical UI that covers many aspects of managing Carbon jobs and reports. Developers may extend this project or steal parts of it to create their own custom application over the Carbon library.

The following screenshot of the application shows an existing report has been selected in the TOC (table of contents) and displayed in HTML format.

![screenshot](_support/example-desktop-load-report.png)

----

## Design Principle

The repository contains a Visual Studio 2022 solution and WPF project targeting net8.0-windows using the C# language.

The project and class structure follow a simple [MVVM][mvvm] pattern that separates the UI code from the data and business logic of the application. The UI is defined in standard XAML and most of the controls are bound to properties of a single controller class of notify properties and observable collections. The developer community uses a variety of MVVM coding styles, so this project attempts to use the simplest classical style possible with no dependecies on any external packages.

The `MainController` class is mostly self-contained and acts like a simple [state machine][state]. As methods on the controller are called, binding properties change to represent the result of processing and the UI updates accordingly. Because there are large numbers of notify binding properties, a [T4 Text Template][t4] is used to generate the repetitive code into a partial class.

----

## Licensing Providers

It's important to note that different companies can host Carbon data in their own Azure cloud storage subscriptions, and each host can create their own licensing system that defines the relationships between users, customers and jobs. Carbon has knowledge of host licensing systems, so it must be initialised with a reference to a class that implements the `ILicensingProvider` interface. Carbon will make calls through that interface to authenticate user credentials and retrieve a user's *licence* which lists the customers and jobs that the user has access to.

There are currently two licensing providers available as NuGet packages:

1. [RCS.Carbon.Licensing.RedCentre][licrcs]  
This provider is proprietary and its use requires consultation with [Red Centre Software][rcshome] who will provide an API key for legitimate use cases.

2. [RCS.Carbon.Licensing.Example][licsamp]  
This provider is general purpose and available for free use. It uses an Azure SQL Server database as the backing storage for user licences, customers and jobs.

Detailed technical information about the providers can be found in the README files for each NuGet package.

----

## Carbon API Calls

Although the project contains a few thousand lines of C# and XAML (some of it template generated), most of the code is boilerplate that would be found in any WPF application of similar design. This section largely ignores the platform specific code and concentrates on the basic sequence of Carbon API calls required to create a useful application over the Carbon libraries.

----

### &#x23e9; Create Licensing Provider

Depending on the values provided in the UI authentication screen, one of the previously described licensing providers is created.

```C#
if (useExampleProvider)
{
  licprov = new RedCentreLicensingProvider(baseUri, apiKey);
}
else
{
  licprov = new ExampleLicensingProvider(productKey, adoConnectString);
}
```

----

### &#x23e9; Create Carbon Engine

 The provider is passed to the Carbon engine's constructor.

```C#
var engine = new CrossTabEngine(licprov);
```

----

### &#x23e9; Authenticate (Get Licence)

 Carbon authenticates either the user Id or Name and returns a licence object containing details about the user's licence and what customers and jobs they have access to.

```C#
if (useUserId)
{
  licence = await engine.GetLicenceId(userId, password);
}
else
{
  licence = await engine.GetLicenceName(userName, password);
}
```

The example app uses the information in the licence object to prepare the navigation tree with a root licence node and hierarchical customer and job children. 

![screenshot](_support/example-desktop-licnodes.png)

----

### &#x23e9; Open Job

When a Job node is clicked or expanded, the job is opened and all available information about the job is loaded. All of the job's information would not normally be immediately needed, but the example code loads all job information to demonstrate the job related API calls and populate the job node's children nodes.

```C#
engine.OpenJob(custName, jobName);
string[] vartreeNames = engine!.ListVartreeNames()];
string[] axisNames = engine.Job.GetAxisNames()];
GenNode[] fullTocNodes = engine.FullTOCGenNodes();
GenNode[] execTocNodes = engine.ExecUserTOCGenNodes();
GenNode[] simpleTocNodes = engine.SimpleTOCGenNodes();
```

![screenshot](_support/example-desktop-jobnodes.png)

----

### &#x23e9; Load Variable Trees

When one of the real vartree (variable tree) nodes is clicked it is set as the active vartree name and it is loaded and its contents are displayed as a hierarchy of child nodes.

```C#
engine.SetTreeNames(vartreeName);
GenNode[] gnodes = _engine!.VarTreeAsNodes();
```

![screenshot](_support/example-desktop-vtnodes.png)

Similarly, when an Axes leaf node is clicked, the axis is loaded and displayed as a hierarchy of child nodes.

```C#
GenNode[] gnodes = _engine!.AxisTreeAsNodes();
```

----

### &#x23e9; Load Variable Metadata

When a variable node (red circles) is clicked its child codeframes and codes are loaded and displayed as a hierarchy of child nodes.

```C#
GenNode[] gnodes = engine.VarAsNodes(variableName);
```

![screenshot](_support/example-desktop-varnodes.png)

----

### &#x23e9; Load Report

Expanding one of the three TOC (table of contents) nodes and selecting one of the leaf report nodes will load the report and display it in the Specification tab in the right hand pane. This is shown in the first screenshot above. A set of APi calls are made to load all information about the report. All the report information would not normally be immediately needed, but the example code loads everything to demonstrate the report related API calls. Many of the loaded properties are bound to controls at the top of the Specification tab.

```C#
string[] lines = [.. engine.ReadFileLines(reportPath)];
engine.TableLoadCBT(reportPath);
XDisplayProperties dprops = engine.GetProps();
TableSpec spec = engine.GetEditSpec();
string syntax = engine.Job.DisplayTable.TableSpec.AsSyntax();  // Experimental
```

The array of `lines` is used to display the raw report in a text box. The raw report is rarely of interest, but it's loaded and shown how it coul dbe useful for debugging.

The `TableLoadCBT` call loads the report and sets it as the active in the Carbon engine.

The `dprops` and `spec` values contain values that fully define the report. In a sophisticated app it would be desirable to editing most of the values, but the example app only binds controls to the most commonly used ones, as shown in the first screenshot above.

The `AsSyntax` call is experimental and has not yet been added to the top level API. It will be in a future release.

----

### &#x23e9; Load Report

After a report has been selected and loaded, the report is *generated* in the format specified in the Format combo box.

```C#
dprops.Output.Format = /* value from combo picker */;
engine.SetProps(dprops);
if (format is XLSX)
{
  byte[] doc = engine.GenTabAsXLSX(null, top, side, filter, weight, sprops, dprops));
}
else
{
  string body = engine.GenTab(null, top, side, filter, weight, sprops, dprops));
}
```

The `SetProps` call is required to update the report's properties in the active report in the engine before the report is generated.

A report in XLSX (Microsoft&reg; Excel&reg;) format is a special case and requires a call to `GenTabAsXLSX` which returns a byte array containing an Excel document. The buffer can be saved to a file or displayed in a suitable enabled 3rd-party UI control.

Other report formats are returned as a string blob which usually contains multiple lines of text separated by newline `\n` characters. The example app simply displays the report in a text box, but it could be parsed and converted into tables, charts or other visualisations.

The HTML output format is suitable for display as both plain text and rendered as a web page in a [WebView2][webview] control.

----

### &#x23e9; New Report

Clicking the **New Report** button clears most report values, creates a default set of properties and load a new default specification.

```C#
TableSpec spec = _engine.GetNewSpec();
XDisplayProperties dprops = new XDisplayProperties();
top = null;
side = null;
// etc
```

Variables, codeframes and codes can be drag-dropped from the tree into the Top and Side text boxes.

![screenshot](_support/example-desktop-vardrop.png)

----

Last udpated: 16-Apr-2025

[wpf]: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/
[carbon]: https://rcsapps.azurewebsites.net/doc/carbon/articles/overview.htm
[mvvm]: https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel
[state]: https://en.wikipedia.org/wiki/Finite-state_machine
[t4]: https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates
[licrcs]: https://www.nuget.org/packages/RCS.Carbon.Licensing.RedCentre
[licsamp]: https://www.nuget.org/packages/RCS.Carbon.Licensing.Example
[rcshome]: https://www.redcentresoftware.com/
[webview]: https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/winforms