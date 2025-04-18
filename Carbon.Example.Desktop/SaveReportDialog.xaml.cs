using System.Windows;
using Carbon.Example.Desktop.Model;

namespace Carbon.Example.Desktop;

public partial class SaveReportDialog : Window
{
	public SaveReportDialog()
	{
		InitializeComponent();
		Loaded += SaveReportDialog_Loaded;
	}

	void SaveReportDialog_Loaded(object sender, RoutedEventArgs e)
	{
		AppUtility.HideMinimizeAndMaximizeButtons(this);
		TextName.Focus();
	}

	void SaveOK_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = true;
	}
}
