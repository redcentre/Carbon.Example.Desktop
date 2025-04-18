﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Carbon.Example.Desktop.Model.Extensions;
using RCS.Carbon.Licensing.Example;
using RCS.Carbon.Licensing.RedCentre;
using RCS.Carbon.Licensing.Shared;
using RCS.Carbon.Shared;
using RCS.Carbon.Tables;

namespace Carbon.Example.Desktop.Model;

#region Enums

public enum LicenceProviderType
{
	RedCentre,
	BayesPrice
}

public enum CredentialType
{
	Id,
	Name
}

#endregion

/// <summary>
/// For more information about this project see: https://github.com/redcentre/Carbon.Example.Desktop
/// </summary>
sealed partial class MainController : INotifyPropertyChanged
{
	public AppSettings Settings { get; private set; }
	public AuthenticateData AuthData { get; private set; } = new AuthenticateData();
	DateTime? closeAlertTime;
	int newReportSequence;
	long[] expandNodeIds;
	readonly PropertyChangedEventHandler jobChangeHandler;

	#region Lifetime

	public MainController()
	{
		jobChangeHandler = new PropertyChangedEventHandler(JobNode_PropertyChanged);
		ReloadSettings();
	}

	[MemberNotNull(nameof(Settings))]
	[MemberNotNull(nameof(expandNodeIds))]
	void ReloadSettings()
	{
		Settings = new AppSettings();
		Settings.Reload();
		AuthData.ActiveLicensingType = Enum.TryParse<LicenceProviderType>(Settings.ActiveLicensingType, out var lictype) ? lictype : LicenceProviderType.RedCentre;
		AuthData.RcsLicBaseAddress = Trim(Settings.RcsLicBaseAddress);
		AuthData.RcsLicApiKey = Trim(Settings.RcsLicApiKey);
		AuthData.RcsLicTimeout = Settings.RcsLicTimeout;
		AuthData.BPrLicProductKey = Trim(Settings.BPrLicProductKey);
		AuthData.BPrLicAdoConnect = Trim(Settings.BPrLicAdoConnect);
		AuthData.ActiveCredType = Enum.TryParse<CredentialType>(Settings.ActiveCredType, out var credtype) ? credtype : CredentialType.Id;
		AuthData.RememberMe = Settings.RememberMe;
		AuthData.CredentialId = AuthData.RememberMe ? Trim(Settings.UserId) : null;
		AuthData.CredentialName = AuthData.RememberMe ? Trim(Settings.UserName) : null;
		AuthData.Password = AuthData.RememberMe ? Trim(Settings.Password) : null;
		SelectedOutputFormat = Enum.TryParse<XOutputFormat>(Settings.LastOutputFormat, out var lastfmt) ? lastfmt : XOutputFormat.CSV;
		expandNodeIds = Settings.ExpandNodeIds?.Cast<string>().Select(s => long.Parse(s, NumberFormatInfo.InvariantInfo)).ToArray() ?? [];
	}

	public void AppClosing()
	{
		// Save the Ids of all navigation tree expanded nodes.
		expandNodeIds = [.. AppUtility.WalkNodes(_obsNodes).Where(n => n.IsExpanded).Select(n => n.Id)];
		Settings.ExpandNodeIds ??= [];
		Settings.ExpandNodeIds.Clear();
		Settings.ExpandNodeIds.AddRange([.. expandNodeIds.Select(n => n.ToString(NumberFormatInfo.InvariantInfo))]);
		Settings.Save();
	}

	public void TimerTick()
	{
		StatusTime = DateTime.Now.ToString("HH:mm:ss");
		if (closeAlertTime != null && closeAlertTime < DateTime.Now)
		{
			AlertTitle = null;
			AlertDetail = null;
			closeAlertTime = null;
		}
	}

	#endregion

	#region Public API

	/// <summary>
	/// The Carbon engine does not have any concept of a 'login', it only authenticates credentials against a
	/// specific licnsing service provider and returns a licence object containing detailed information about
	/// the user's account and the customers and jobs they are connected to.
	/// </summary>
	public async Task GetLicence()
	{
		AuthenticatingMessage = Strings.BusyAuthenticating;
		AuthError = null;
		// Save a snapshot of all the credentials as authentication starts.
		Settings.ActiveLicensingType = AuthData.ActiveLicensingType.ToString();
		Settings.RcsLicBaseAddress = AuthData.RcsLicBaseAddress;
		Settings.RcsLicApiKey = AuthData.RcsLicApiKey;
		Settings.RcsLicTimeout = AuthData.RcsLicTimeout;
		Settings.BPrLicProductKey = AuthData.BPrLicProductKey;
		Settings.BPrLicAdoConnect = AuthData.BPrLicAdoConnect;
		Settings.ActiveCredType = AuthData.ActiveCredType.ToString();
		Settings.RememberMe = AuthData.RememberMe;
		Settings.UserId = Settings.RememberMe ? AuthData.CredentialId : null;
		Settings.UserName = Settings.RememberMe ? AuthData.CredentialName : null;
		Settings.Password = Settings.RememberMe ? AuthData.Password : null;
		Settings.Save();
		// Create the required type of licensing provider. There are currently only two provider implementations.
		// One from Red Centre Software which uses a REST API to a private service and one from BayesPrice which
		// uses a SQL database.
		ILicensingProvider prov = AuthData.ActiveLicensingType == LicenceProviderType.RedCentre ?
			new RedCentreLicensingProvider(AuthData.RcsLicBaseAddress!, AuthData.RcsLicApiKey!) :
			new ExampleLicensingProvider(AuthData.BPrLicProductKey!, AuthData.BPrLicAdoConnect!);
		try
		{
			// Create the Carbon engine using a specified licensing provider.
			var engine = new CrossTabEngine(prov);
			// Authenticate and attempt to get the licensing information response.
			if (AuthData.ActiveCredType == CredentialType.Id)
			{
				Licence = await engine.GetLicenceId(AuthData.CredentialId!, AuthData.Password!);
				LogEngine($"GetLicenceId({AuthData.CredentialId},{AuthData.Password}) -> {_licence!.Name}");
			}
			else
			{
				Licence = await engine.GetLicenceName(AuthData.CredentialName!, AuthData.Password!);
				LogEngine($"GetLicenceName({AuthData.CredentialName},{AuthData.Password}) -> {_licence!.Id}");
			}
			Engine = engine;
			Provider = prov;
			// Full authentication is complete and the navigation tree can be loaded.
			FillTree();
		}
		catch (Exception ex)
		{
			AuthError = ex;
		}
		finally
		{
			AuthenticatingMessage = null;
		}
	}

	public void CloseLicence()
	{
		ClearReport();
		OpenReportNode = null;
		OpenJobNode = null;
		OpenCustomerNode = null;
		Licence = null;
		Engine = null;
		Provider = null;
	}

	public void CloseAlert()
	{
		AlertTitle = null;
		AlertDetail = null;
		closeAlertTime = null;
	}

	public async Task GenerateReport()
	{
		await WrapWork(Strings.WorkTitleGentab, async () =>
		{
			_reportProps!.Output.Format = _selectedOutputFormat;
			Settings.LastOutputFormat = _selectedOutputFormat.ToString();
			Settings.Save();
			await Task.Run(() => _engine!.SetProps(_reportProps!));
			LogEngine("SetProps");
			ReportHtmlBody = null;
			switch (_reportProps!.Output.Format)
			{
				// The Excel output format is a special case that can't be display in a WPF
				// without the use of Office or 3rd-party controls. It's simply printed as
				// lines of hex dump of the document.
				case XOutputFormat.XLSX:
					ReportTabIndex = 0;
					byte[] workbook = await Task.Run(() => _engine!.GenTabAsXLSX(null, _reportTop!, _reportSide!, _reportFilter, _reportWeight, _reportSpec!.SpecProperties, _reportProps));
					var lines = MainUtility.HexToLines(workbook).ToArray();
					ReportTextBody = string.Join(Environment.NewLine, lines);
					LogEngine($"GenTabAsXLSX({_reportTop},{_reportSide},{_reportFilter},{_reportWeight}) {_reportProps.Output.Format} -> {workbook.Length} bytes ({lines.Length} lines)");
					break;
				// All of the remaining report formats can be displayed as plain text.
				// The HTML output format can ALSO be displayed in a WebView2 control.
				default:
					ReportTextBody = await Task.Run(() => _engine!.GenTab(null, _reportTop!, _reportSide!, _reportFilter, _reportWeight, _reportSpec!.SpecProperties, _reportProps));
					LogEngine($"GenTab({_reportTop},{_reportSide},{_reportFilter},{_reportWeight}) {_reportProps.Output.Format} -> ({_reportTextBody!.Length} lines)");
					if (_reportProps.Output.Format == XOutputFormat.HTML)
					{
						ReportHtmlBody = _reportTextBody;
						ReportTabIndex = 1;
					}
					else
					{
						ReportTabIndex = 0;
					}
					break;
			}
		});
	}

	public async Task NewReport()
	{
		await WrapWork(Strings.WorkTitleNewReport, async () =>
		{
			++newReportSequence;
			ReportSpec = await Task.Run(() => _engine!.GetNewSpec());
			LogEngine("GetNewSpec()");
			ReportProps = new XDisplayProperties();
			ReportTop = null;
			ReportSide = null;
			ReportFilter = null;
			ReportWeight = null;
			ReportTextBody = null;
			ReportHtmlBody = null;
			IsNewReport = true;
		});
	}

	public void PrepareSaveReport()
	{
		// Set nice defaults for the save report name.
		if (_isNewReport)
		{
			SaveReportName = Strings.NewReportName.Format(newReportSequence);
		}
		else
		{
			// The currently open report will be formatted as something like:
			// 'Tables/User/henry/RootReport.cbt'
			// 'Tables/User/henry/Path-1/Path-2/SubReport.cbt'
			// Note that the Path segments can zero or more. Skip the three prefix segments and join
			// the optional path segments with a plain name. Both / and \ can be segment separators.
			string[] allsegs = _selectedNode!.Key!.Split(Constants.PathSeparators);
			var pathsegs = allsegs[3..^1];
			var name = Path.GetFileNameWithoutExtension(allsegs[^1]);
			SaveReportName = string.Join('/', pathsegs.Concat([name]));
		}
		ValidateSaveName();     // BUG This should be done by binding when the dialog opens. It's a hack to do it here.
	}

	/// <summary>
	/// The entered save name will be a full job relative path and name (without extension).
	/// </summary>
	public async Task SaveReport()
	{
		await WrapWork(Strings.WorkTitleSaveReport, async () =>
		{
			var segs = _saveReportName!.Split(Constants.PathSeparators);
			var path = string.Join("/", segs[..^1]);
			var name = segs.Last();
			await Task.Run(() => _engine!.SaveTableUserTOC(name, path, true));
			LogEngine($"TableSaveCBT({name},{path})");
			await ReloadTocs();
		});
	}

	public async Task DeleteReport()
	{
		await WrapWork(Strings.WorkTitleSaveReport, async () =>
		{
			string? message = null;
			bool success = await Task.Run(() => _engine!.DeleteInUserTOC(_selectedNode!.Key!, true, out message));
			LogEngine($"DeleteInUserTOC({_selectedNode!.Key}) -> {success}");
			if (!success)
			{
				string msg = string.IsNullOrEmpty(message) ? "No details are available." : message;
				throw new ApplicationException($"Carbon engine method DeleteInUserTOC returned failure. {msg}");
			}
			await ReloadTocs();
		});
	}

	/// <summary>
	/// After a report/TOC save the shape of the TOC can change in non trvial ways.
	/// The trees could be adjusted in-place, but the logic is tricky for the three
	/// trees in the job. It's easier to reload the TOC tree branches. Their expanded
	/// states will be preserved so it won't be visually annoying.
	/// </summary>
	async Task ReloadTocs()
	{
		GenNode[] fullTocNodes = await Task.Run(() => _engine!.FullTOCGenNodes());
		var fullParent = _openJobNode!.Children.FirstOrDefault(n => n.Type == AppNodeType.FullToc)!;
		fullParent.Children.Clear();
		AddGenNodes(fullParent, fullTocNodes);
		GenNode[] execTocNodes = await Task.Run(() => _engine!.ExecUserTOCGenNodes());
		var execParent = _openJobNode!.Children.FirstOrDefault(n => n.Type == AppNodeType.ExecToc)!;
		execParent.Children.Clear();
		AddGenNodes(execParent, execTocNodes);
		GenNode[] simpleTocNodes = await Task.Run(() => _engine!.SimpleTOCGenNodes());
		var simpleParent = _openJobNode!.Children.FirstOrDefault(n => n.Type == AppNodeType.SimpleToc)!;
		simpleParent.Children.Clear();
		AddGenNodes(simpleParent, simpleTocNodes);
		LogEngine($"ReloadTocs() -> {GenNode.WalkNodes(fullTocNodes).Count()} full nodes, {GenNode.WalkNodes(execTocNodes).Count()} exec nodes, {GenNode.WalkNodes(simpleTocNodes).Count()} simple nodes");
	}

	#endregion

	#region Tree and Nodes

	/// <summary>
	/// The navigation tree is initally loaded with the customers, jobs and real vartree names that are
	/// returned as part of the authenticated licence. Clicking on certain nodes will cause them to be
	/// deep loaded.
	/// </summary>
	void FillTree()
	{
		ObsNodes.Clear();
		var lnode = new LicenceNode(_licence!);
		_obsNodes.Add(lnode);
		foreach (var cust in _licence!.Customers.OrderBy(c => c.Name.ToUpper()))
		{
			var cnode = new CustomerNode(cust);
			AddChild(lnode, cnode);
			foreach (var job in cust.Jobs.OrderBy(j => j.Name.ToUpper()))
			{
				var jnode = new JobNode(job);
				jnode.PropertyChanged += jobChangeHandler;
				AddChild(cnode, jnode);
				if (job.RealCloudVartreeNames?.Length > 0)
				{
					var fnode = new AppNode(AppNodeType.Folder, "VTRealFolder", Strings.NodeLabelVtReal, null);
					AddChild(jnode, fnode);
					foreach (var vt in job.RealCloudVartreeNames)
					{
						var rvtnode = new AppNode(AppNodeType.RealVartree, "VTReal", vt, vt);
						AddChild(fnode, rvtnode);
					}
				}
			}
		}
	}

	/// <summary>
	/// Special cases. When a job node is expanded it will be opened and deep loaded as an act
	/// of user friendliness, so there is no need to actually select the job node.
	/// </summary>
	async void JobNode_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender is JobNode jnode)
		{
			if (e.PropertyName == nameof(JobNode.IsExpanded) && jnode.IsExpanded && !jnode.IsLoaded)
			{
				await WrapWork(Strings.WorkTitleOpenLoadJob, async () =>
				{
					await GuardedOpenJob(jnode);
					await GuardedDeepLoadJob(jnode);
				});
			}
		}
	}

	/// <summary>
	/// Selecting certain types of nodes triggers special processing such as deep loading
	/// and displaying details for the node.
	/// </summary>
	async Task AfterNodeSelectAsync()
	{
		if (_selectedNode == null) return;
		if (_selectedNode.Type == AppNodeType.Job)
		{
			MainTabIndex = 0;
			await WrapWork(Strings.WorkTitleOpenLoadJob, async () =>
			{
				var jnode = (JobNode)_selectedNode;
				await GuardedOpenJob(jnode);
				await GuardedDeepLoadJob(jnode);
			});
		}
		else if (_selectedNode.Type == AppNodeType.Customer)
		{
			MainTabIndex = 0;
		}
		else if (_selectedNode.Type == AppNodeType.RealVartree)
		{
			await WrapWork(Strings.WorkTitleLoadVartree, async () =>
			{
				await GuardedDeepLoadRealVartree(_selectedNode);
			});
		}
		else if (_selectedNode.Type == AppNodeType.Axis)
		{
			await WrapWork(Strings.WorkTitleLoadAxis, async () =>
			{
				await GuardedDeepLoadAxis(_selectedNode);
			});
		}
		else if (_selectedNode.Type == AppNodeType.VartreeVariable)
		{
			await WrapWork(Strings.WorkTitleLoadVar, async () =>
			{
				await GuardedDeepLoadVariable(_selectedNode);
			});
		}
		else if (_selectedNode.Type == AppNodeType.Table)
		{
			await WrapWork(Strings.WorkTitleLoadLines, async () =>
			{
				await LoadAndGenerateReport((TocLeafNode)_selectedNode);
			});
		}
	}

	/// <summary>
	/// Recursively adds GenNodes and their children to an existing node the tree.
	/// The GenNode is a generic node returned from many Carbon API calls.
	/// A table lookup partly controls how the GenNode to be converted into some
	/// kind of local AppNode.
	/// </summary>
	void AddGenNodes(AppNode parent, IEnumerable<GenNode> gnodes)
	{
		if (gnodes == null) return;
		foreach (var gnode in gnodes)
		{
			AppNode node;
			if (gnode.Type == "Table")
			{
				// TOC leaf nodes are a special case.
				node = new TocLeafNode(AppNodeType.Table, gnode.Value1, gnode.Value2);
			}
			else
			{
				// Table-driven conversion of other similar nodes.
				AppNodeType type = AppNodeType.Undefined;
				string? key = null;
				string label;
				var tups = new[]
				{
				("Folder", AppNodeType.Folder, 1, 0),
				("Variable", AppNodeType.VartreeVariable, 3, 1),
				("Codeframe", AppNodeType.Codeframe, 1, 0),
				("Code", AppNodeType.Code, 3, 0),
				("Section" , AppNodeType.Section, 1, 0),
				("User" , AppNodeType.User, 1, 0),
				("Arith", AppNodeType.Arith, 3, 0),
				("Net", AppNodeType.Net, 3, 0),
				("Nes", AppNodeType.Nes, 3, 0),
				("Nes", AppNodeType.Stat, 3, 0),
				("Axis", AppNodeType.AxisChild, 1, 0)
			};
				var tup = tups.FirstOrDefault(t => t.Item1 == gnode.Type);
				if (tup.Item1 == null)
				{
					label = $"{gnode.Id}|{gnode.Type}|{gnode.Value1}|{gnode.Value2}";
				}
				else
				{
					type = tup.Item2;
					label = tup.Item3 == 1 ? gnode.Value1 : tup.Item3 == 2 ? gnode.Value2 : BestLabel(gnode);
					key = tup.Item4 == 1 ? gnode.Value1 : tup.Item4 == 2 ? gnode.Value2 : null;
				}
				node = new AppNode(type, parent.Id.ToString(NumberFormatInfo.InvariantInfo), label, key);
			}
			AddChild(parent, node);
			AddGenNodes(node, gnode.Children);
			parent.IsExpanded = expandNodeIds.Contains(parent.Id);
		}
	}

	readonly AppNodeType[] NoAutoExpandTypes = [AppNodeType.Job, AppNodeType.VartreeVariable, AppNodeType.Codeframe, AppNodeType.RealVartree];

	/// <summary>
	/// For user friendliness, every time a child node is added to a parent node, the parent is automatically
	/// expanded if its in the settings list of previously expanded nodes AND it's not in the list of node types
	/// that should NOT be auto expanded because that would cause unexpected demand child loading work to happen.
	/// </summary>
	void AddChild(AppNode parent, AppNode child)
	{
		parent.AddChild(child);
		if (!NoAutoExpandTypes.Contains(parent.Type))
		{
			parent.IsExpanded = expandNodeIds.Contains(parent.Id);
		}
	}

	#endregion

	#region Guarded Methods

	/// <summary>
	/// Opens a job if it's not already open. Opening (switching to) a new job causes some lower dependent properties to null.
	/// </summary>
	async Task GuardedOpenJob(JobNode jnode)
	{
		if (jnode.Job.Id == _openJobNode?.Job.Id) return;
		string custName = jnode.CustomerParentNode.Customer.Name;
		string jobName = jnode!.Job.Name;
		await Task.Run(() => _engine!.OpenJob(custName, jobName));
		LogEngine($"OpenJob({custName},{jobName})");
		OpenJobNode = jnode;
		OpenCustomerNode = jnode.CustomerParentNode;
		ClearReport();
	}

	/// <summary>
	/// Deep loads a job node if not already done. Note that the job is expected to be open.
	/// </summary>
	async Task GuardedDeepLoadJob(JobNode jnode)
	{
		if (jnode.IsLoaded) return;
		string[] vartreeNames = [];
		string[] axisNames = [];
		GenNode[] fullTocNodes = [];
		GenNode[] execTocNodes = [];
		GenNode[] simpleTocNodes = [];
		await Task.Run(() =>
		{
			vartreeNames = [.. _engine!.ListVartreeNames()];
			LogEngine($"ListVartreeNames() -> {vartreeNames.Length}");
			axisNames = [.. _engine.Job.GetAxisNames()];
			LogEngine($"GetAxisNames() -> {axisNames.Length}");
			fullTocNodes = _engine.FullTOCGenNodes();
			LogEngine($"FullTOCGenNodes() -> {GenNode.WalkNodes(fullTocNodes).Count()} nodes]");
			execTocNodes = _engine.ExecUserTOCGenNodes();
			LogEngine($"ExecUserTOCGenNodes() -> {GenNode.WalkNodes(execTocNodes).Count()} nodes]");
			simpleTocNodes = _engine.SimpleTOCGenNodes();
			LogEngine($"SimpleTOCGenNodes() -> {GenNode.WalkNodes(simpleTocNodes).Count()} nodes]");
			ReportProps = _engine.GetProps();
			LogEngine($"GetProps() -> Decimals[{_reportProps!.Decimals.Frequencies},{_reportProps!.Decimals.Percents},{_reportProps!.Decimals.Statistics},{_reportProps!.Decimals.Expressions}]");
			ReportSpec = _engine.GetEditSpec();
			LogEngine($"GetEditSpec() -> {GenNode.WalkNodes(_reportSpec!.TopAxis).Count()} top nodes - {GenNode.WalkNodes(_reportSpec!.SideAxis).Count()} side nodes");

		});
		var vtsnode = new AppNode(AppNodeType.Folder, "VTNameFolder", Strings.NodeLabelVtNamed, null);
		AddChild(jnode, vtsnode);
		foreach (var vt in vartreeNames)
		{
			var vtnode = new AppNode(AppNodeType.Vartree, "VTName", vt, vt);
			AddChild(vtsnode, vtnode);
		}
		vtsnode.IsExpanded = expandNodeIds.Contains(vtsnode.Id);

		var axsnode = new AppNode(AppNodeType.Folder, "AxFolder", Strings.NodeLabelAxes, null);
		AddChild(jnode, axsnode);
		foreach (var ax in axisNames)
		{
			var axnode = new AppNode(AppNodeType.Axis, "Ax", ax, ax);
			AddChild(axsnode, axnode);
		}
		axsnode.IsExpanded = expandNodeIds.Contains(axsnode.Id);

		var tocnode = new AppNode(AppNodeType.FullToc, "FullFolder", Strings.NodeLabelFullToc, null);
		AddChild(jnode, tocnode);
		AddGenNodes(tocnode, fullTocNodes);
		tocnode.IsExpanded = expandNodeIds.Contains(tocnode.Id);

		tocnode = new AppNode(AppNodeType.ExecToc, "ExecFolder", Strings.NodeLabelExecToc, null);
		AddChild(jnode, tocnode);
		AddGenNodes(tocnode, execTocNodes);
		tocnode.IsExpanded = expandNodeIds.Contains(tocnode.Id);

		tocnode = new AppNode(AppNodeType.SimpleToc, "SimpleFolder", Strings.NodeLabelSimpleToc, null);
		AddChild(jnode, tocnode);
		AddGenNodes(tocnode, simpleTocNodes);
		tocnode.IsExpanded = expandNodeIds.Contains(tocnode.Id);

		jnode.IsLoaded = true;
		jnode.IsExpanded = true;
	}

	/// <summary>
	/// Guarded deep load of a real vartree. Ensure the ancestor job is open and the vartree is the active
	/// tree name in the job. Then the vartree can be deep loaded if needed.
	/// </summary>
	async Task GuardedDeepLoadRealVartree(AppNode node)
	{
		JobNode jnode = AppUtility.FindAncestorNodeByType<JobNode>(node, AppNodeType.Job);
		await GuardedOpenJob(jnode);
		_engine!.SetTreeNames(node.Key!);
		LogEngine($"SetTreeNames({node.Key!}) in GuardedDeepLoadVartree");
		OpenVartreeName = node.Label;
		if (!node.IsLoaded)
		{
			GenNode[] gnodes = await Task.Run(() => _engine!.VarTreeAsNodes());
			LogEngine($"VarTreeAsNodes() -> {GenNode.WalkNodes(gnodes).Count()} nodes");
			AddGenNodes(node, gnodes);
			node.IsLoaded = true;
			node.IsExpanded = true;
		}
	}

	/// <summary>
	/// Guarded deep load of a TOC variable. Ensure the ancestor job is open and the
	/// selected vartree set as active, then the variable can be deep loaded if needed.
	/// </summary>
	async Task GuardedDeepLoadVariable(AppNode node)
	{
		var rvtnode = AppUtility.FindAncestorNodeByType<AppNode>(node, AppNodeType.RealVartree);
		var jnode = AppUtility.FindAncestorNodeByType<JobNode>(rvtnode, AppNodeType.Job);
		await GuardedOpenJob(jnode);
		_engine!.SetTreeNames(rvtnode.Label);
		LogEngine($"SetTreeNames({node.Key!}) in GuardedDeepLoadVariable");
		OpenVartreeName = rvtnode.Label;
		if (!node.IsLoaded)
		{
			GenNode[] gnodes = await Task.Run(() => _engine!.VarAsNodes(node.Key!));
			LogEngine($"VarAsNodes({node.Key}) -> {GenNode.WalkNodes(gnodes).Count()} nodes");
			// There might be no children (eg Weights)
			if (gnodes.FirstOrDefault()?.Children == null) return;
			// Note that there are two types of returned nodes. A 'hierarchic' and a 'simple' return have
			// different shapes, but luckily the difference doesn't matter here and we just add all the
			// children because the root node in each case is useless. Some apps may care about the difference.
			AddGenNodes(node, gnodes[0].Children);
			node.IsLoaded = true;
			node.IsExpanded = true;
		}
	}

	/// <summary>
	/// Loading an axis tree is very similar to loading a vartree.
	/// </summary>
	async Task GuardedDeepLoadAxis(AppNode node)
	{
		JobNode jnode = AppUtility.FindAncestorNodeByType<JobNode>(node, AppNodeType.Job);
		await GuardedOpenJob(jnode);
		_engine!.SetTreeNames(node.Key!);
		LogEngine($"SetTreeNames({node.Key!}) in GuardedDeepLoadAxis");
		OpenVartreeName = node.Label;
		if (!node.IsLoaded)
		{
			GenNode[] gnodes = await Task.Run(() => _engine!.AxisTreeAsNodes());
			LogEngine($"VarAsNodes({node.Key}) -> {GenNode.WalkNodes(gnodes).Count()} nodes");
			AddGenNodes(node, gnodes);
			node.IsLoaded = true;
			node.IsExpanded = true;
		}
	}

	#endregion

	/// <summary>
	/// This validation runs on every change of the save report name.
	/// </summary>
	void ValidateSaveName()
	{
		string? error = null;
		if (_saveReportName == null)
		{
			error = Strings.SaveErrorRequired;
		}
		else
		{
			string[] parts = _saveReportName.Split(Constants.PathSeparators);
			if (parts.Any(parts => parts.Length == 0))
			{
				error = Strings.SaveErrorBlankSeg;
			}
			else if (parts.Any(p => p.StartsWith(' ') || p.EndsWith(' ')))
			{
				error = Strings.SaveErrorSpace;
			}
			else if (parts.Any(p => p.Intersect(Path.GetInvalidFileNameChars()).Any()))
			{
				error = Strings.SaveErrorBadChars;
			}
		}
		SaveReportFeedback = error;
		IsSaveNameValid = _saveReportFeedback == null;
	}

	/// <summary>
	/// The only leaf nodes in the TOCs are currently Table nodes. Selecting one causes
	/// everything known about the report to be loaded and generated.
	/// </summary>
	async Task LoadAndGenerateReport(TocLeafNode node)
	{
		JobNode jnode = AppUtility.FindAncestorNodeByType<JobNode>(node, AppNodeType.Job);
		await GuardedOpenJob(jnode);
		// Read the raw lines for browsing display.
		TextLines = await Task.Run(() => _engine!.ReadFileLines(node.Key!).ToArray());
		LogEngine($"ReadFileLines({node.Key}) -> {_textLines!.Length}");
		// Set the report as the active one in the engine.
		await Task.Run(() => _engine!.TableLoadCBT(node.Key!));
		LogEngine($"TableLoadCBT({node.Key})");
		// Get the report properties and set defaults.
		ReportProps = await Task.Run(() => _engine!.GetProps());
		LogEngine($"GetProps() -> {_reportProps}");
		// Get the report specification.
		ReportSpec = await Task.Run(() => _engine!.GetEditSpec());
		LogEngine($"GetEditSpec() -> {_reportSpec}");
		// The current report syntax needs to be retrieved separately.
		// This obscure call was made for the Platinum UI and hasn't
		// been added to the engine API yet. It comes back as an unusual
		// joined key=value lines which need clumsy parsing.
		string syntax = _engine!.Job.DisplayTable.TableSpec.AsSyntax();
		LogEngine($"Job.DisplayTable.TableSpec.AsSyntax() -> {syntax.Replace("\n", "\xb6")}");
		string[] synparts = syntax.Split('\n');
		foreach (string part in synparts)
		{
			int ix = part.IndexOf('=');
			string key = part[..ix];
			string? value = part[(ix + 1)..];
			if (value.Length == 0) value = null;
			if (key == "Top") ReportTop = value;
			else if (key == "Side") ReportSide = value;
			else if (key == "Filter") ReportFilter = value;
			else if (key == "Weight") ReportWeight = value;
		}
		OpenReportNode = node;
		IsNewReport = false;
		MainTabIndex = 1;
		MainCommands.GenerateReport.Execute(null, null);
	}

	void ClearReport()
	{
		ReportTextBody = null;
		ReportProps = null;
		ReportSpec = null;
		ReportTop = null;
		ReportSide = null;
		ReportFilter = null;
		ReportWeight = null;
	}

	/// <summary>
	/// Wraps some asynchronous work and shows a standard alert if an exception occurs.
	/// </summary>
	async Task WrapWork(string title, Func<Task> work)
	{
		BusyMessage = title;
		try
		{
			await work();
		}
		catch (Exception ex)
		{
			ShowAlert(title, ex.Message);
		}
		finally
		{
			BusyMessage = null;
		}
	}

	void ShowAlert(string title, string detail, int closeSeconds = 60)
	{
		AlertTitle = title;
		AlertDetail = detail;
		closeAlertTime = DateTime.Now.AddSeconds(closeSeconds);
	}

	#region Logging

	public sealed record LogRow(DateTime Time, string Category, int ThreadId, string Message);

	void LogEngine(string message) => Log("Engine", message);

	void Log(string category, string message)
	{
		var row = new LogRow(DateTime.Now, category, Environment.CurrentManagedThreadId, message);
		if (Application.Current.Dispatcher.CheckAccess())
		{
			_obsLog.Insert(0, row);
			Trace.WriteLine(row.ToString());
			if (_obsLog.Count > 200)
			{
				_obsLog.RemoveAt(200);
			}
		}
		else
		{
			Application.Current.Dispatcher.InvokeAsync(() => Log(category, message));
		}
	}

	#endregion

	static string BestLabel(GenNode node) => string.IsNullOrEmpty(node.Value2) ? node.Value1 : $"{node.Value1}={node.Value2}";

	static string? Trim(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
