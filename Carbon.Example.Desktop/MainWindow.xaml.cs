using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows;
using wf = System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;
using Carbon.Example.Desktop.Model;

namespace Carbon.Example.Desktop
{
    /// <summary>
    /// The code-behind for the main window tries to be thin and only related to UI matters.
    /// Most of the business logic happens inside the DataContext controller class.
    /// </summary>
    public partial class MainWindow : Window, System.Windows.Forms.IWin32Window
	{
		ReportWindow _repwin;

		public MainWindow()
		{
			InitializeComponent();
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				Loaded += MainWindow_Loaded;
				Closed += MainWindow_Closed;
				LoadWindowBounds();
			}
		}

		public IntPtr Handle => new System.Windows.Interop.WindowInteropHelper(this).Handle;

		public System.Windows.Forms.IWin32Window Win32Window => this;

		MainController Controller => (MainController)DataContext;

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Controller.Startup();
			// Immediately show a login prompt. A successful login will cause the controller
			// to continue and put itself into a state where business logic can begin.
			MainCommands.LoginPrompt.Execute(null, this);
		}

		void MainWindow_Closed(object sender, EventArgs e)
		{
			_repwin?.Dispose();
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

		void ShowReportWindow()
		{
			_repwin = new ReportWindow
			{
				DataContext = Controller,
				Owner = this
			};
			_repwin.Closed += (s, e) =>
			{
				Controller.Settings.Put(null, nameof(WindowStartupLocation) + "Report", new Rect(_repwin.Left, _repwin.Top, _repwin.Width, _repwin.Height));
			};
			_repwin.Show();
			var r = Controller.Settings.Get(null, nameof(WindowStartupLocation) + "Report", default(Rect));
			if (r.Width != 0)
			{
				_repwin.Top = r.Top;
				_repwin.Left = r.Left;
				_repwin.Width = r.Width;
				_repwin.Height = r.Height;
			}
			else
			{
				// The report window is placed vertial centre-align right of the main window.
				_repwin.Top = Top + Height / 2 - _repwin.Height / 2;
				if (_repwin.Top < 10)
				{
					_repwin.Top = 10;
				}
				_repwin.Left = Left + Width + 20;
				if (_repwin.Left > wf.Screen.PrimaryScreen.WorkingArea.Width)
				{
					_repwin.Left = wf.Screen.PrimaryScreen.WorkingArea.Width - 100;
				}
			}
		}

		void NavTree_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var selnode = (BindNode)e.NewValue;
			Controller.SelectedNavNode = selnode;
		}

		void Vartree_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var selnode = (BindNode)e.NewValue;
			Controller.SelectedVartreeNode = selnode;
		}

		void NavTree_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Delete)
			{
				if (Controller.SelectedNavNode?.Type == "File")
				{
					MainCommands.DeleteReport.Execute(Controller.SelectedNavNode, this);
				}
			}
		}

		void VarTree_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				var item = MainUtility.FindVisualParent<TreeViewItem>(e.OriginalSource);
				if (item != null)
				{
					var node = (BindNode)item.DataContext;
					if (node.Type == BindNode.TypeVariable || node.Type == BindNode.TypeCodeframe || node.Type == BindNode.TypeCode)
					{
						DragDrop.DoDragDrop((TreeView)sender, node, DragDropEffects.Copy);
					}
				}
			}
		}
	}
}
