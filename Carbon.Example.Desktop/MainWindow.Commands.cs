using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Carbon.Example.Desktop.Model;
using Carbon.Example.Desktop.Model.Extensions;

namespace Carbon.Example.Desktop;

internal static partial class MainCommands
{
	public static RoutedUICommand GetLicence = new("Get Licence", "GetLicence", typeof(Window));
	public static RoutedUICommand CloseLicence = new("Close Licence", "CloseLicence", typeof(Window));
	public static RoutedUICommand CloseAlert = new("Close Alert", "CloseAlert", typeof(Window));
	public static RoutedUICommand GenerateReport = new("Generate Report", "GenerateReport", typeof(Window));
	public static RoutedUICommand NewReport = new("New Report", "NewReport", typeof(Window));
	public static RoutedUICommand LaunchSaveReport = new("Launch Save Report", "Launch SaveReport", typeof(Window));
	public static RoutedUICommand SaveReport = new("Save Report", "SaveReport", typeof(Window));
	public static RoutedUICommand DeleteReport = new("Delete Report", "DeleteReport", typeof(Window));
}

partial class MainWindow
{
	void HelpCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void HelpExecute(object sender, ExecutedRoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/redcentre/Carbon.Example.Desktop") { UseShellExecute = true });

	void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void CloseExecute(object sender, ExecutedRoutedEventArgs e) => Close();

	void GetLicenceCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !Controller.AuthData.AnyErrors && Controller.AuthenticatingMessage == null && Controller.Engine == null;
	async void GetLicenceExecute(object sender, ExecutedRoutedEventArgs e) => await Controller.GetLicence();

	void CloseLicenceCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.Engine != null;
	void CloseLicenceExecute(object sender, ExecutedRoutedEventArgs e) => Controller.CloseLicence();

	void CloseAlertCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.AlertTitle != null;
	void CloseAlertExecute(object sender, ExecutedRoutedEventArgs e) => Controller.CloseAlert();

	void GenerateReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;   // WORKAROUND The GenerateReport button's IsEnabled property is bound to the ReportTextBody property.
	async void GenerateReportExecute(object sender, ExecutedRoutedEventArgs e) => await Controller.GenerateReport();

	void NewReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.OpenJobNode != null;
	async void NewReportExecute(object sender, ExecutedRoutedEventArgs e) => await Controller.NewReport();

	void LaunchSaveReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true; // WORKAROUND (same as GenerateReport)
	void LaunchSaveReportExecute(object sender, ExecutedRoutedEventArgs e) => TrapLaunchSaveReport();

	void SaveReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.IsSaveNameValid;
	async void SaveReportExecute(object sender, ExecutedRoutedEventArgs e) => await Controller.SaveReport();

	void DeleteReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.SelectedNode?.Type == Model.AppNodeType.Table;
	async void DeleteReportExecute(object sender, ExecutedRoutedEventArgs e) => await TrapDeleteReport();

	void TrapLaunchSaveReport()
	{
		Controller.PrepareSaveReport();
		var dialog = new SaveReportDialog()
		{
			Owner = this,
			DataContext = Controller
		};
		dialog.ShowDialog();
	}

	async Task TrapDeleteReport()
	{
		string[] segs = Controller.SelectedNode!.Key!.Split(Constants.PathSeparators);
		string user = segs[2];
		if (string.Compare(user, Controller.Licence!.Name, StringComparison.OrdinalIgnoreCase) != 0)
		{
			MessageBox.Show(Strings.DeleteTableNotOwner.Format(Controller.Licence!.Name), Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}
		string relname = AppUtility.UserRelativeName(Controller.SelectedNode!.Key!);
		string message = Strings.DeleteTableConfirmPrompt.Format(relname);
		if (MessageBox.Show(message, Application.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
		{
			await Controller.DeleteReport();
		}
	}
}
