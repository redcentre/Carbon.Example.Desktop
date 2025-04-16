using System.ComponentModel;
using System.Diagnostics;
using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop.Model;

partial class MainController : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	public bool IsBusy => _busyMessage != null;

	public bool IsIdle => _busyMessage == null;

	string? webBuffer;
	bool webActivated;
	const string NoHtml = """
		<html>
		<body>
		  <p style="color:silver;margin:1rem;font-family:sans-serif;">No HTML Data</p>
		</body>
		</html>
		""";

	/// <summary>
	/// There is some irritating complicated logic to prevent the WebView2 control from navigating to any HTML
	/// content before it is fully initialised. The control is probably not visible when it's first created,
	/// which means the EnsureCoreWebView2Async will not complete until the control is visible at some unpredictable
	/// future time. The workaround is to buffer the HTML content until the control is fully initialised,
	/// then the html is unbuffered and thereafter the buffering stops and the property is set normally.
	/// The parent control of the WebView2 will detect when initialisation completes and call this method.
	/// </summary>
	public void SetWebViewActivated()
	{
		Debug.Assert(!webActivated);
		webActivated = true;
		ReportHtmlBody = webBuffer;
	}

	string? _reportHtmlBody;
	public string? ReportHtmlBody
	{
		get => _reportHtmlBody;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? NoHtml : value;
			if (_reportHtmlBody != newval)
			{
				if (!webActivated)
				{
					webBuffer = newval;
				}
				else
				{
					_reportHtmlBody = newval;
					OnPropertyChanged(nameof(ReportHtmlBody));
				}
			}
		}
	}

	/// <summary>
	/// Items source for the report format picker. NOTE --> Same order as the enum.
	/// </summary>
	public XOutputFormat[] FormatPicks { get; } =
	[
		XOutputFormat.None,
		XOutputFormat.TSV,
		XOutputFormat.CSV,
		XOutputFormat.SSV,
		XOutputFormat.XLSX,
		XOutputFormat.XML,
		XOutputFormat.HTML,
		XOutputFormat.OXT,
		XOutputFormat.Pandas
	];

	/// <summary>
	/// Item source for the report property significance type picker.
	/// </summary>
	public XSigType[] SigTypePicks { get; } =
	[
		XSigType.SingleCell,
		XSigType.ColumnGroups,
		XSigType.RefColumn,
		XSigType.RefRow,
		XSigType.RowGroups
	];
}
