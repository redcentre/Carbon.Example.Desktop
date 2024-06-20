using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Carbon.Example.Desktop;

internal sealed class Program
{
	static App _app;

	[STAThread]
	public static void Main()
	{
		Assembly asm = typeof(Program).Assembly;
		Company = asm.GetCustomAttributes(false).OfType<AssemblyCompanyAttribute>().Single().Company;
		Product = asm.GetCustomAttributes(false).OfType<AssemblyProductAttribute>().Single().Product;
		Title = asm.GetCustomAttributes(false).OfType<AssemblyTitleAttribute>().Single().Title;
		Description = asm.GetCustomAttributes(false).OfType<AssemblyDescriptionAttribute>().Single().Description;
		Copyright = asm.GetCustomAttributes(false).OfType<AssemblyCopyrightAttribute>().Single().Copyright;
		InfoVersion = asm.GetCustomAttributes(false).OfType<AssemblyInformationalVersionAttribute>().Single().InformationalVersion;
		AsmVersion = asm.GetName().Version;
		FileVersion = asm.GetCustomAttributes(false).OfType<AssemblyFileVersionAttribute>().Single().Version;
		HomeFolder = new DirectoryInfo(Path.GetDirectoryName(asm.Location));
		_app = new App();
		_app.InitializeComponent();
		_app.Run();
	}

	public static MainController MainTroller { get; set; }
	public static string Company { get; set; }
	public static string Product { get; set; }
	public static string Title { get; set; }
	public static string Description { get; set; }
	public static string Copyright { get; set; }
	public static string InfoVersion { get; set; }
	public static Version AsmVersion { get; set; }
	public static string FileVersion { get; set; }
	public static DirectoryInfo HomeFolder { get; set; }
}
