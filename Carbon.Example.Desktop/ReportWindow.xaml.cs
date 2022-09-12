using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop
{
	public partial class ReportWindow : Window
	{
		bool init;

		public ReportWindow()
		{
			InitializeComponent();
			Loaded += ReportWindow_Loaded;
			Closing += ReportWindow_Closing;
		}

		MainController Controller => (MainController)DataContext;

		void ReportWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// WARNING: The web view control is very delicate and the
			// slightest mistake will cause incomprehensible exceptions.
			WebViewCtl.Source = new Uri("https://rcsapps.azurewebsites.net/noreport.htm");
			Controller.PropertyChanged += (s, ev) =>
			{
				if (ev.PropertyName == nameof(MainController.GenTabLines))
				{
					if (Controller.SelectedOutputFormat == XOutputFormat.HTML)
					{
						_ = ShowHtml();
					}
					else if (Controller.SelectedOutputFormat == XOutputFormat.XLSX)
					{
						ShowExcel();
					}
				}
			};
		}

		async Task ShowHtml()
		{
			if (!init)
			{
				await WebViewCtl.EnsureCoreWebView2Async();
				init = true;
			}
			if (Controller.GenTabLines == null)
			{
				WebViewCtl.Source = new Uri("https://rcsapps.azurewebsites.net/noreport.htm");
			}
			else
			{
				string html = Controller.GenTabLines == null ? "<html><head><title>No Content</title></head><body><h1>No Content</h1></body></html>" : string.Join(Environment.NewLine, Controller.GenTabLines);
				WebViewCtl.NavigateToString(html);
			}
		}

		void ShowExcel()
		{
			Process.Start(new ProcessStartInfo() { FileName = Controller.LastXlsxSaveFile.FullName, UseShellExecute = true });
		}

		public void Dispose()
		{
			WebViewCtl.Dispose();
		}

		void ReportWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
		}
	}
}
