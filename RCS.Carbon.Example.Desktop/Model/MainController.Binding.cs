using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using RCS.Carbon.Shared;
using RCS.Carbon.Tables;
using RCS.Licensing.Provider.Shared;

namespace RCS.Carbon.Example.Desktop.Model;

partial class MainController : INotifyPropertyChanged
{
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

	public bool IsBusy => !string.IsNullOrEmpty(BusyMessage);

	public bool IsIdle => string.IsNullOrEmpty(BusyMessage);

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsBusy))]
	[NotifyPropertyChangedFor(nameof(IsIdle))]
	string? _busyMessage;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(DeleteReportCommand))]
	AppNode? _selectedNode;

	partial void OnSelectedNodeChanged(AppNode? value) => Application.Current.Dispatcher.InvokeAsync(async () => await AfterNodeSelectAsync());

	[ObservableProperty]
	string? _statusMessage;

	[ObservableProperty]
	XOutputFormat _selectedOutputFormat = XOutputFormat.CSV;

	[ObservableProperty]
	int _appFontSize = 13;

	[ObservableProperty]
	int _mainTabIndex;

	[ObservableProperty]
	string? _statusTime = "Loading...";

	[ObservableProperty]
	string? _alertTitle;

	[ObservableProperty]
	string? _alertDetail;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(GetLicenceCommand))]
	string? _authenticatingMessage;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(GetLicenceCommand))]
	int _authErrorCount;

	[ObservableProperty]
	AuthenticateData _authData = new();

	[ObservableProperty]
	Exception? _authError;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(GetLicenceCommand))]
	[NotifyCanExecuteChangedFor(nameof(CloseLicenceCommand))]
	ILicensingProvider? _provider;

	[ObservableProperty]
	CrossTabEngine? _engine;

	[ObservableProperty]
	LicenceInfo? _licence;

	[ObservableProperty]
	ObservableCollection<AppNode> _obsNodes = [];

	[ObservableProperty]
	ObservableCollection<LogRow> _obsLog = [];

	[ObservableProperty]
	CustomerNode? _openCustomerNode;

	[ObservableProperty]
	string? _openVartreeName;

	[ObservableProperty]
	JobNode? _openJobNode;

	[ObservableProperty]
	TocLeafNode? _openReportNode;

	[ObservableProperty]
	TableSpec? _reportSpec;

	[ObservableProperty]
	XDisplayProperties? _reportProps;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(GenerateReportCommand))]
	string? _reportTop;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(GenerateReportCommand))]
	string? _reportSide;

	[ObservableProperty]
	string? _reportFilter;

	[ObservableProperty]
	string? _reportWeight;

	[ObservableProperty]
	bool _filterActive;

	[ObservableProperty]
	bool _weightActive;

	[ObservableProperty]
	string[]? _textLines;

	[ObservableProperty]
	string? _reportTextBody;

	[ObservableProperty]
	int _reportTabIndex;

	[ObservableProperty]
	bool _isNewReport;

	[ObservableProperty]
	string? _saveReportName;

	[ObservableProperty]
	string? _saveReportFeedback;

}