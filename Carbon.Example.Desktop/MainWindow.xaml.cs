using System;
using System.Windows;

namespace Carbon.Example.Desktop
{
	public partial class MainWindow : Window, System.Windows.Forms.IWin32Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
			Closed += MainWindow_Closed;
			LoadWindowBounds();
		}

		public IntPtr Handle => new System.Windows.Interop.WindowInteropHelper(this).Handle;

		public System.Windows.Forms.IWin32Window Win32Window => this;

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Controller.Startup();
			MainCommands.LoginPrompt.Execute(null, this);
		}

		void MainWindow_Closed(object sender, EventArgs e)
		{
			SaveWindowBounds();
			Controller.Shutdown();
		}

		void LoadWindowBounds()
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			var r = Controller.Settings.Get(null, nameof(WindowStartupLocation), default(Rect));
			if (r.Width != 0)
			{
				WindowStartupLocation = WindowStartupLocation.Manual;
				Top = r.Top;
				Left = r.Left;
				Width = r.Width;
				Height = r.Height;
			}
		}

		void SaveWindowBounds()
		{
			if (WindowState == WindowState.Normal)
			{
				Controller.Settings.Put(null, nameof(WindowStartupLocation), new Rect(Left, Top, Width, Height));
			}
		}

		MainController Controller => (MainController)DataContext;

		private void NavTree_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var selnode = (BindNode)e.NewValue;
			Controller.SelectedNavNode = selnode;
		}
	}
}
