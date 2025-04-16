using System;
using System.Windows;
using System.Windows.Threading;
using Carbon.Example.Desktop.Model;

namespace Carbon.Example.Desktop;

partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		Loaded += MainWindow_Loaded;
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
	}

	MainController Controller => (MainController)DataContext;
	DispatcherTimer? mainTimer;

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