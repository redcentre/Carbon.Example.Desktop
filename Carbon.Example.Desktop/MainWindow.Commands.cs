using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Carbon.Example.Desktop;

internal static partial class MainCommands
{
	public static RoutedUICommand GetLicence = new("Get Licence", "GetLicence", typeof(Window));
	public static RoutedUICommand CloseLicence = new("Close Licence", "CloseLicence", typeof(Window));
	public static RoutedUICommand CloseAlert = new("Close Alert", "CloseAlert", typeof(Window));
	public static RoutedUICommand GenerateReport = new("Generate Report", "GenerateReport", typeof(Window));
	public static RoutedUICommand NewReport = new("New Report", "NewReport", typeof(Window));
}

partial class MainWindow
{
	void HelpCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void HelpExecute(object sender, ExecutedRoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/redcentre/") { UseShellExecute = true });

	void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void CloseExecute(object sender, ExecutedRoutedEventArgs e) => Close();

	void GetLicenceCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !Controller.AuthData.AnyErrors && Controller.AuthenticatingMessage == null && Controller.Engine == null;
	async void GetLicenceExecute(object sender, ExecutedRoutedEventArgs e) => await Controller.GetLicence();

	void CloseLicenceCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.Engine != null;
	void CloseLicenceExecute(object sender, ExecutedRoutedEventArgs e) => Controller.CloseLicence();

	void CloseAlertCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.AlertTitle != null;
	void CloseAlertExecute(object sender, ExecutedRoutedEventArgs e) => Controller.CloseAlert();

	void GenerateReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.ReportTop != null && Controller.ReportSide != null;
	async void GenerateReportExecute(object sender, ExecutedRoutedEventArgs e) => await Controller.GenerateReport();

	void NewReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.OpenJobNode != null;
	async void NewReportExecute(object sender, ExecutedRoutedEventArgs e) => await Controller.NewReport();
}
