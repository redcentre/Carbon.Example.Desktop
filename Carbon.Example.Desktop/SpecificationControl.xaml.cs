using System.Windows;
using Carbon.Example.Desktop.Model;

namespace Carbon.Example.Desktop;

partial class SpecificationControl : BaseControl
{
	int wvLoadCount = 0;

	public SpecificationControl()
	{
		InitializeComponent();
		WebViewCtl.Loaded += WebViewCtl_Loaded;
	}

	async void WebViewCtl_Loaded(object sender, RoutedEventArgs e)
	{
		++wvLoadCount;
		if (wvLoadCount == 1)
		{
			// See the detailed notes about control initialisation in the MainController class.
			WebViewCtl.CoreWebView2InitializationCompleted += WebViewCtl_CoreWebView2InitializationCompleted;
			await WebViewCtl.EnsureCoreWebView2Async();
		}
	}

	void WebViewCtl_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
	{
		Controller.SetWebViewActivated();
	}

	#region Drag-Drop

	void Top_PreviewDragOver(object sender, DragEventArgs e)
	{
		var node = (AppNode)e.Data.GetData(typeof(AppNode));
		e.Effects = node == null ? DragDropEffects.None : DragDropEffects.Copy;
		e.Handled = true;
	}

	void Top_Drop(object sender, DragEventArgs e)
	{
		var node = (AppNode)e.Data.GetData(typeof(AppNode));
		if (Controller.ReportTop == null)
		{
			Controller.ReportTop = node.Key;
		}
		else
		{
			Controller.ReportTop = $"{Controller.ReportTop},{node.Key}";
		}
	}

	void Side_PreviewDragOver(object sender, DragEventArgs e)
	{
		var node = (AppNode)e.Data.GetData(typeof(AppNode));
		e.Effects = node == null ? DragDropEffects.None : DragDropEffects.Copy;
		e.Handled = true;
	}

	void Side_Drop(object sender, DragEventArgs e)
	{
		var node = (AppNode)e.Data.GetData(typeof(AppNode));
		if (Controller.ReportSide == null)
		{
			Controller.ReportSide = node.Key;
		}
		else
		{
			Controller.ReportSide = $"{Controller.ReportSide},{node.Key}";
		}
	}

	#endregion
}
