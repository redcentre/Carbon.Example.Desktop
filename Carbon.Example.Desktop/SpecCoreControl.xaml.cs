using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Carbon.Example.Desktop
{
	public partial class SpecCoreControl : UserControl
	{
		public SpecCoreControl()
		{
			InitializeComponent();
		}

		MainController Controller => (MainController)DataContext;

		void Top_DragOver(object sender, DragEventArgs e)
		{
			var node = e.Data.GetData(typeof(BindNode));
			e.Effects = node == null ? DragDropEffects.None : DragDropEffects.Copy;
		}

		void Top_Drop(object sender, DragEventArgs e)
		{
			var node = (BindNode)e.Data.GetData(typeof(BindNode));
			Controller.ObsTopNodes.Add(node);
		}

		void Side_DragOver(object sender, DragEventArgs e)
		{
			var node = e.Data.GetData(typeof(BindNode));
			e.Effects = node == null ? DragDropEffects.None : DragDropEffects.Copy;
		}

		void Side_Drop(object sender, DragEventArgs e)
		{
			var node = (BindNode)e.Data.GetData(typeof(BindNode));
			Controller.ObsSideNodes.Add(node);
		}

		void Top_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				Controller.RemoveSelectedTopNode();
			}
		}

		void Side_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				Controller.RemoveSelectedSideNode();
			}
		}
	}
}
