using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Carbon.Example.Desktop")]
[assembly: AssemblyDescription("A WPF desktop application that exercises the Carbon librart")]
[assembly: CLSCompliant(true)]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Red Centre Software")]
[assembly: AssemblyProduct("Carbon Example Desktop")]
[assembly: AssemblyCopyright("Copyright © 2022")]
[assembly: AssemblyTrademark("2022-09-09 17:56 GMT+10")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: Guid("95df8d15-c848-42ee-a694-8ea2f26f290c")]
