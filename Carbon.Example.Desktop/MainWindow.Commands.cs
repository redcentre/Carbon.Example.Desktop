using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Carbon.Example.Desktop
{
	internal static class MainCommands
	{
		public static RoutedUICommand AppExit = new RoutedUICommand("Exit", "AppExit", typeof(Window));
		public static RoutedUICommand HelpAbout = new RoutedUICommand("Help About", "HelpAbout", typeof(Window));
		public static RoutedUICommand LoginPrompt = new RoutedUICommand("Login Prompt", "LoginPrompt", typeof(Window));
		public static RoutedUICommand DismissError = new RoutedUICommand("Dismiss Error", "DismissError", typeof(Window));
		public static RoutedUICommand OpenReport = new RoutedUICommand("Open Report", "OpenReport", typeof(Window));
		public static RoutedUICommand RunSpec = new RoutedUICommand("Run Spec", "RunSpec", typeof(Window));
		public static RoutedUICommand SaveReport = new RoutedUICommand("Save Report", "SaveReport", typeof(Window));
	}

	partial class MainWindow
	{
		void CanExecuteAppExit(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
		void ExecuteAppExit(object target, ExecutedRoutedEventArgs e) => Close();

		void CanExecuteHelpAbout(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
		void ExecuteHelpAbout(object target, ExecutedRoutedEventArgs e) => Process.Start("https://github.com/redcentre");

		void CanExecuteDismissError(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.ErrorTitle != null;
		void ExecuteDismissError(object target, ExecutedRoutedEventArgs e) => Controller.DismissError();

		void CanExecuteLoginPrompt(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.Lic == null;
		void ExecuteLoginPrompt(object target, ExecutedRoutedEventArgs e) => DoLoginPrompt();

		void CanExecuteOpenReport(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.DProps != null;
		void ExecuteOpenReport(object target, ExecutedRoutedEventArgs e) => OpenReportUI();

		void CanExecuteRunSpec(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = false;// Controller.ReportTop != null && Controller.ReportSide != null && Controller.DProps != null;
		void ExecuteRunSpec(object target, ExecutedRoutedEventArgs e) => Controller.RunSpec();

		void CanExecuteSaveReport(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.GenTabLines != null;
		void ExecuteSaveReport(object target, ExecutedRoutedEventArgs e) => SaveReportUI();

		bool DoLoginPrompt()
		{
			var dialog = new SessionDialog();
			dialog.DataContext = Controller;
			dialog.AccountId = Controller.LoginId;
			dialog.Password = Controller.LoginPassword;
			dialog.Owner = this;
			if (dialog.ShowDialog() == true)
			{
				Controller.LoginId = dialog.AccountId;
				Controller.LoginPassword = dialog.Password;
				return true;
			}
			return false;
		}

		void OpenReportUI()
		{
			MessageBox.Show(this, "Open report is not implemented yet. A pick list of local or cloud reports must be presented somehow.", "Open Report", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		void SaveReportUI()
		{
			//if (!await Controller.ListSavedReports()) return;
			var dialog = new SaveReportDialog();
			dialog.Owner = this;
			if (dialog.ShowDialog() == true)
			{
				Controller.SaveReport(dialog.ReportPath, dialog.ReportName);
			}
		}
	}
}
