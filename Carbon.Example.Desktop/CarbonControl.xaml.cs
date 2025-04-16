using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Carbon.Example.Desktop.Model;

namespace Carbon.Example.Desktop;

partial class CarbonControl : BaseControl
{
	public CarbonControl()
	{
		InitializeComponent();
	}

	void MainTree_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		var selnode = (AppNode)e.NewValue;
		Controller.SelectedNode = selnode;
	}

	void MainTree_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			var item = MainUtility.FindVisualParent<TreeViewItem>(e.OriginalSource);
			if (item != null)
			{
				var node = (AppNode)item.DataContext;
				if (node.Type == AppNodeType.VartreeVariable || node.Type == AppNodeType.Codeframe || node.Type == AppNodeType.Code)
				{
					DragDrop.DoDragDrop((TreeView)sender, node, DragDropEffects.Copy);
				}
			}
		}
	}
}
