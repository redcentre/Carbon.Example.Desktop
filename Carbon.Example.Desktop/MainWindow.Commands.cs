using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Carbon.Example.Desktop.Model;
using Carbon.Example.Desktop.Model.Extensions;
using RCS.Carbon.Licensing.Shared;

namespace Carbon.Example.Desktop;

internal static partial class MainCommands
{
	public static RoutedUICommand LaunchSaveReport = new("Launch Save Report", "Launch SaveReport", typeof(Window));
}

partial class MainWindow
{
	void HelpCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void HelpExecute(object sender, ExecutedRoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/redcentre/Carbon.Example.Desktop") { UseShellExecute = true });

	void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void CloseExecute(object sender, ExecutedRoutedEventArgs e) => Close();

	void LaunchSaveReportCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.ReportTextBody != null;
	void LaunchSaveReportExecute(object sender, ExecutedRoutedEventArgs e) => TrapLaunchSaveReport();

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

	bool DeleteReportHandler(LicenceInfo? licence, AppNode? selectedNode)
	{
		string[] segs = selectedNode!.Key!.Split(Constants.PathSeparators);
		string user = segs[2];
		if (string.Compare(user, licence!.Name, StringComparison.OrdinalIgnoreCase) != 0)
		{
			MessageBox.Show(Strings.DeleteTableNotOwner.Format(licence!.Name), Strings.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
			return false;
		}
		string relname = AppUtility.UserRelativeName(selectedNode!.Key!);
		string message = Strings.DeleteTableConfirmPrompt.Format(relname);
		return MessageBox.Show(message, Strings.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes;
	}
}
