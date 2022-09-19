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

---

## Design Principle

The Visual Studio 2022 project is a [WPF][wpf] application targeting .NET Framework 4.8.

> :star: Note that the Carbon library can be used from any application targeting .NET Standard 2.0, .NET Framework 4.7.1 and later, and .NET 6.0. This example project only targets 4.8 because it was convenient when the project was created. It may be converted to 6.0 in the near future.

The project and class structure follows a simple [MVVM][mvvm] pattern that separates the UI code from the data and business logic of the application. The UI is defined in standard XAML and most of the controls are bound to properties of a single *controller* class of notify properties and observable collections. The developer community uses a variety of MVVM coding styles, so this project attempts to use the simplest classical style possible.

The `MainController` class is mostly self-contained and acts like a simple [state machine][statem]. Most methods of the controller simply return True or False to indicate success, but internally a set of binding property values will be set so that the UI can respond accordingly. Because there are large numbers of notify binding properties, a [T4 Text Template][t4] is used to generate the repetitive code into a partial class.

---

## API Squence

This sample application demonstrates how the Carbon API can be used to create a small framework to manage jobs, variables and reports. The API calls must follow the sequence as described in the follow sections.

### Create Engine

```
var eng = new CrossTabEngine();
```

> Creates an instance of the Carbon cross-tabulation engine class. The sample code *lazily* creates the instance on first use. This class publishes most of the top-level API methods that are of general interest to client applications. A single instance of the class can be held globally for application lifetime. Multiple instances might only be useful in advanced scenarios where multiple jobs are open and being processed concurrently.

### LoginId

```
Licence Lic = await Engine.LoginId(id, password);
```

> The `LoginId` merhod must be called with valid credentials to unlock the majority of the Carbon API. The default credentials for the sample application are User Id `10000858` which corresponds to a public guest account that can be used for evaluation and demonstrations. The password for this guest account is `guest`. Red Centre Software customers can use their Ruby or Laser credentials instead of the guest ones.

> Login processing is passed through the [licensing web service][licsvc] where the credentials are validated, access to customers and jobs is calculated, and upon success an instance of a `Licence` objects is returned. If authentication fails for any reason then an Exception is thrown with a message containing an explanation for the failure.

> The returned `Licence` class contains account information for the `Id` credential. Applications may optionally use the account information for display or logging. A very useful property of the Licence class is `Customers`, which contains a 3-level hierarchical set of customers, job and variable trees that the account is authorised to access. The sample application displays this information in a TreeView control so the user can easily click available job nodes to open them.


### OpenJob (Cloud)

```
await Task.Run(() => Engine.OpenJob(_selectedCust.Name, _selectedJob.Name));
```

> When the user clicks a job node in the leftmost navigation tree, the customer and job names are passed into the `OpenJob` API method which opens a cloud job and sets it as the active job in the Carbon engine. Carbon has one job open at any time. The method does not return anything, it will throw an Exception on failure.

> Carbon can open *local* jobs stored in the local file system, and it can open *Cloud* jobs store in Azure. The sample application only opens Cloud jobs at the moment, but support for local jobs may be added in a future release.

> Once a job is open, it is possible to run cross-tabulation reports immediately, which is a simple usage scenario demonstrated in the sample Console application project. This sample project is more sophisticated and it makes the following API calls to demonstrate all of the information that is available about an open job.

```
var t1 = Task.Run(() => Engine.GetProps());
var t2 = Task.Run(() => Engine.ListVartreeNames().ToArray());
var t3 = Task.Run(() => Engine.ListAxisNames().ToArray());
var t4 = Task.Run(() => Engine.ListSavedReports());
var t5 = Task.Run(() => Engine.GetLegacyTocAsNodes());
var t6 = Task.Run(() => Engine.GetJobIniAsNodes());
```

> Note how the calls are run in parallel tasks to improve performance.  The information returned by these calls is used to create TreeView control branches under the job node.

> ![Navigation tree partial screenshot][img1]

### GetVartreeAsNodes

```
var gnodes = await Task.Run(() => Engine.GetVartreeAsNodes(_selectedVartreeName));
var gnodes = await Task.Run(() => Engine.GetAxisAsNodes(_selectedAxisTreeName));
```

> Selecting a Vartree or Axis node in the navigation tree will issue a `GetVartreeAsNodes` or `GetAxisAsNodes` API call respectively to fill the variable TreeView control.

### GetVarMetaParsed

```
VMeta = await Task.Run(() => Engine.GetVarMetaParsed(_selectedVartreeNode.Text));
```

> Selecting a Variable (red node) the first time will issue a `GetVarMetaParsed` API call to get the child codeframe and code information, which is added as child nodes. It is then possible to drag the nodes into the Top and Side axis lists to compose the specification of a cross-tabulation report.

> ![Variable selection partial screenshot][img2]

### GenTab

> After the Top and Side axis have been filled, Click the Run button (shortcut F5) to validate the report specification and run the report generation process. The report output will be displayed in the floating report window. There are 8 report formats that can be selected from the dropdown list. The HTML format report is shown in the second tab rendered in a WebView control. The XLSX format reports is shown as dump of the workbook buffer and a temporary .xlsx file containing the report bytes is Shell launched, which will normally cause Excel to open if it is installed.

> Application developers are free to generate a report using a format that suits their needs, then transform, save or render the results as required.

> ![Report partial screenshot][img3]

### TableSaveCBT

```
await Task.Run(() => Engine.TableSaveCBT(name));
```

> Clicking the Save icon :floppy_disk: will open open a dialog that prompts for a report name and optional path prefix to save the report using the `TableSaveCBT` API method.

> ![Report save dialog screenshot][img4]

> The save name can be a simple name, or it can be a multi-part path the prefixes the name, such as `July/Sales/Regional Usage`. The full path of the report will be created if it doesn't already exist. Any existing report will be silently overwritten.

> After report save, the `ListSavedReports` method is called to refresh the navigation tree and show the newly saved report.

----

#### :red_circle: NOTE — This sample project is under development and not all features are fully working yet.

----

Last updated: 19-Sep-2022

### 


[wpf]: https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf?view=vs-2022
[mvvm]: https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel
[statem]: https://en.wikipedia.org/wiki/Finite-state_machine
[t4]: https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates?view=vs-2022
[licsvc]: https://rcsapps.azurewebsites.net/licensing2/
[img1]: https://systemrcs.blob.core.windows.net/wiki-images/sample%20desktop%20nav%20tree.png
[img2]: https://systemrcs.blob.core.windows.net/wiki-images/sample%20desktop%20var%20select.png
[img3]: https://systemrcs.blob.core.windows.net/wiki-images/sample%20desktop%20report%20tsv.png
[img4]: https://systemrcs.blob.core.windows.net/wiki-images/sample%20desktop%20save%20dialog.png