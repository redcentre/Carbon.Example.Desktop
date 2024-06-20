using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Carbon.Example.Desktop;

internal static class MainCommands
{
	public static RoutedUICommand AppExit = new RoutedUICommand("Exit", "AppExit", typeof(Window));
	public static RoutedUICommand HelpAbout = new RoutedUICommand("Help About", "HelpAbout", typeof(Window));
	public static RoutedUICommand LoginPrompt = new RoutedUICommand("Login Prompt", "LoginPrompt", typeof(Window));
	public static RoutedUICommand DismissError = new RoutedUICommand("Dismiss Error", "DismissError", typeof(Window));
	public static RoutedUICommand OpenReport = new RoutedUICommand("Open Report", "OpenReport", typeof(Window));
	public static RoutedUICommand RunSpec = new RoutedUICommand("Run Spec", "RunSpec", typeof(Window), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F5) }));
	public static RoutedUICommand SaveReport = new RoutedUICommand("Save Report", "SaveReport", typeof(Window));
	public static RoutedUICommand DeleteReport = new RoutedUICommand("Delete Report", "DeleteReport", typeof(Window));
}

partial class MainWindow
{
	void CanExecuteAppExit(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void ExecuteAppExit(object target, ExecutedRoutedEventArgs e) => Close();

	void CanExecuteHelpAbout(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
	void ExecuteHelpAbout(object target, ExecutedRoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/redcentre/Carbon.Example.Desktop") { UseShellExecute = true });

	void CanExecuteDismissError(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.ErrorTitle != null;
	void ExecuteDismissError(object target, ExecutedRoutedEventArgs e) => Controller.DismissError();

	void CanExecuteLoginPrompt(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.Lic == null;
	void ExecuteLoginPrompt(object target, ExecutedRoutedEventArgs e) => DoLoginPrompt();

	void CanExecuteOpenReport(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.DProps != null;
	void ExecuteOpenReport(object target, ExecutedRoutedEventArgs e) => OpenReportUI();

	void CanExecuteRunSpec(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.ReportTop != null && Controller.ReportSide != null && Controller.DProps != null && Controller.SelectedOutputFormat != RCS.Carbon.Shared.XOutputFormat.None;
	async void ExecuteRunSpec(object target, ExecutedRoutedEventArgs e) => await Controller.RunSpecAsync();

	void CanExecuteSaveReport(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.GenTabLines != null;
	async void ExecuteSaveReport(object target, ExecutedRoutedEventArgs e) => await SaveReportUI();

	void CanExecuteDeleteReport(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Controller.SelectedNavNode?.Type == "File";
	void ExecuteDeleteReport(object target, ExecutedRoutedEventArgs e) => DeleteReportUI((BindNode)e.Parameter);

	/// <summary>
	/// The login dialog prompt will only return true if authentication was successful.
	/// In that case the controller will have already put itself into a working state.
	/// </summary>
	bool DoLoginPrompt()
	{
		var dialog = new SessionDialog
		{
			DataContext = Controller,
			AccountId = Controller.LoginId,
			Password = Controller.LoginPassword,
			Owner = this
		};
		if (dialog.ShowDialog() == true)
		{
			Controller.LoginId = dialog.AccountId;
			Controller.LoginPassword = dialog.Password;
			ShowReportWindow();
			return true;
		}
		return false;
	}

	void OpenReportUI()
	{
		MessageBox.Show(this, "Open report is not implemented yet. A pick list of local or cloud reports must be presented somehow.", "Open Report", MessageBoxButton.OK, MessageBoxImage.Warning);
	}

	async Task SaveReportUI()
	{
		// We could show the tree of saved reports here, but for now the user just
		// names the optional path prefix and the name of the report.
		var dialog = new SaveReportDialog
		{
			Owner = this,
			DataContext = DataContext
		};
		if (dialog.ShowDialog() == true)
		{
			await Controller.SaveReport(dialog.ReportName);
		}
	}

	void DeleteReportUI(BindNode node)
	{
		if (MessageBox.Show(this, $"Do you want to delete the saved report?\n\n{node.Text}", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
		{
			Controller.DeleteReport(node.Text);
		}
	}
}
