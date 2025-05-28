using System;
using System.Windows;
using System.Windows.Threading;
using RCS.Carbon.Example.Desktop.Model;

namespace RCS.Carbon.Example.Desktop;

partial class MainWindow : Window
{
	MainController Controller => (MainController)DataContext;
	DispatcherTimer? mainTimer;

	public MainWindow()
	{
		InitializeComponent();
		Loaded += MainWindow_Loaded;
		Closing += MainWindow_Closing;
		Closed += MainWindow_Closed;
	}

	void MainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		mainTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
		mainTimer.Tick += (s, e2) =>
		{
			TimerTick();
		};
		mainTimer.Start();
		LoadWindowBounds();
		Controller.DeleteReportCallback = (lic, node) => DeleteReportHandler(lic, node);
	}

	void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
	{
		Controller.AppClosing();
	}

	void MainWindow_Closed(object? sender, System.EventArgs e)
	{
		mainTimer?.Stop();
		SaveWindowBounds();
	}

	void TimerTick()
	{
		Controller.TimerTick();
	}

	void LoadWindowBounds()
	{
		string rs = Controller.Settings.WindowBounds;
		RectConverter rc = new RectConverter();
		try
		{
			Rect r = Rect.Parse(rs);
			WindowStartupLocation = WindowStartupLocation.Manual;
			Top = r.Top;
			Left = r.Left;
			Width = r.Width;
			Height = r.Height;
		}
		catch (InvalidOperationException)
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}
	}

	void SaveWindowBounds()
	{
		if (WindowState == WindowState.Normal)
		{
			var r = new Rect(Left, Top, Width, Height);
			Controller.Settings.WindowBounds = r.ToString();
			Controller.Settings.Save();
		}
	}
}