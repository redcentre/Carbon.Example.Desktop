using System.Windows;
using Microsoft.Web.WebView2.Wpf;

namespace Carbon.Example.Desktop;

internal class BrowserSourceBehavior
{
	public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
	  "Html",
	  typeof(string),
	  typeof(BrowserSourceBehavior),
	  new FrameworkPropertyMetadata(OnHtmlChanged));

	[AttachedPropertyBrowsableForType(typeof(WebView2))]
	public static string GetHtml(WebView2 d) => (string)d.GetValue(HtmlProperty);

	public static void SetHtml(WebView2 d, string value) => d.SetValue(HtmlProperty, value);

	static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var wb = d as WebView2;
		var doc = e.NewValue as string;
		if (wb != null && doc != null)
		{
			wb.NavigateToString(doc);
		}
	}
}
