using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Orthogonal.NSettings;
using RCS.Carbon.Tables;
using System.IO;
using RCS.Carbon.Shared;
using System.Windows.Input;
using System.Collections.Generic;
using DocumentFormat.OpenXml.VariantTypes;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Office2021.Excel.NamedSheetViews;

namespace Carbon.Example.Desktop
{
	sealed partial class MainController : INotifyPropertyChanged
	{
		public ISettingsProcessor Settings { get; private set; }
		public event PropertyChangedEventHandler PropertyChanged;
		DispatcherTimer appTimer;

		#region Lifetime

		public MainController()
		{
			Settings = new RegistrySettings();
			ReloadSettings();
			Engine = new CrossTabEngine();
		}

		public void Startup()
		{
			appTimer = new DispatcherTimer()
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			appTimer.Tick += AppTimer_Tick;
			appTimer.Start();
			StatusMessage = "Not logged in";
		}

		void AppTimer_Tick(object sender, EventArgs e)
		{
			StatusTime = DateTime.Now.ToString("HH:mm:ss");
		}

		public void Shutdown()
		{
			_engine?.CloseJob();
			appTimer?.Stop();
		}

		void ReloadSettings()
		{
			_appFontSize = Settings.GetInt(null, nameof(AppFontSize), 13);
			OnPropertyChanged(nameof(AppFontSize));
			_reportFontSize = Settings.GetInt(null, nameof(ReportFontSize), 13);
			OnPropertyChanged(nameof(ReportFontSize));
			_loginId = Settings.Get(null, nameof(LoginId), Environment.GetEnvironmentVariable("RCSTESTID") ?? "10000858");
			OnPropertyChanged(nameof(LoginId));
			_loginPassword = Settings.Get(null, nameof(LoginPassword), Environment.GetEnvironmentVariable("RCSTESTPASS") ?? "guest");
			OnPropertyChanged(nameof(LoginPassword));
		}

		void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		#endregion

		public async Task<bool> LoginAsync(string id, string password)
		{
			try
			{
				Lic = await _engine.LoginId(id, password);
				StatusMessage = $"Logged in as Id {_lic.Id} Name '{_lic.Name}'";
				LoginError = null;
				BusyMessage = null;
				LoadAccounts();
				return true;
			}
			catch (Exception ex)
			{
				LoginError = ex;
				Trace.WriteLine(ex.Message);
				BusyMessage = null;
				return false;
			}
		}

		void LoadAccounts()
		{
			ObsAccNodes = new ObservableCollection<BindNode>();
			var doc = XDocument.Load("Accounts-Available.xml");
			//var accounts = doc.Root.Elements("account").Select(e => new AccountData() { Name = (string)e.Attribute("name") }).ToArray();
			foreach (var aelem in doc.Root.Elements("account"))
			{
				var acc = new AccountData() { Name = (string)aelem.Attribute("name") };
				var anode = new BindNode("CUST", acc.Name, 0, acc, null);
				foreach (var jelem in aelem.Elements("job"))
				{
					var job = new ContainerData((string)jelem.Attribute("name"), null, null, null);
					var jnode = new BindNode("JOB", job.Name, 1, job, anode);
					anode.AddChild(jnode);
				}
				anode.IsExpanded = true;
				_obsAccNodes.Add(anode);
			}
		}

		public void DismissError()
		{
			ErrorTitle = ErrorMessage = null;
		}

		/// <summary>
		/// A variety of processing in response to clicks on certain navigation tree node types.
		/// </summary>
		void PostNavSelect()
		{
			if (_selectedNavNode == null)
			{
				SelectedAccount = null;
				SelectedJob = null;
				SelectedVartreeName = null;
				SelectedAxisTreeName = null;
				ObsVartreeNodes = null;
				return;
			}
			if (_selectedNavNode.Type == "CUST")
			{
				// ┌───────────────────────────────────────────────────────────────┐
				// │ Customer node - do nothing                                    │
				// └───────────────────────────────────────────────────────────────┘
			}
			else if (_selectedNavNode.Type == "JOB")
			{
				// ┌───────────────────────────────────────────────────────────────┐
				// │ A variety of child information for a job is unwound into      │
				// │ branches of child nodes under the job.                        │
				// └───────────────────────────────────────────────────────────────┘
				var accdata = (AccountData)_selectedNavNode.Parent.Data;
				SelectedAccount = accdata;
				var jobdata = (ContainerData)_selectedNavNode.Data;
				SelectedJob = jobdata;
				SelectedVartreeName = null;
				SelectedAxisTreeName = null;
				ObsVartreeNodes = null;
				if (!OpenJob()) return;
				if (!_selectedNavNode.AnyChildren)
				{
					// TODO Load the new TOC
					if (_tocNewRootGenNodes != null)
					{
						var tocnode = new BindNode("TOCNEW", "TOC", 3, null, _selectedNavNode);
						GenToBindNodes(_tocNewRootGenNodes, tocnode);
						_selectedNavNode.AddChild(tocnode);
					}
					if (_tocOldRootGenNodes != null)
					{
						var tocnode = new BindNode("TOCOLD", "TOC (Legacy)", 3, null, _selectedNavNode);
						GenToBindNodes(_tocOldRootGenNodes, tocnode);
						_selectedNavNode.AddChild(tocnode);
					}
					if (_vartreeNames?.Length > 0)
					{
						var vtsnode = new BindNode("VTS", "Vartrees", 3, null, _selectedNavNode);
						var vtnodes = _vartreeNames.Select(v => new BindNode("VT", v, 4, v, _selectedNavNode)).ToArray();
						vtsnode.AddChildRange(vtnodes);
						_selectedNavNode.AddChild(vtsnode);
					}
					if (_axisTreeNames?.Length > 0)
					{
						var axsnode = new BindNode("AXS", "Axis Trees", 3, null, _selectedNavNode);
						var axnodes = _axisTreeNames.Select(v => new BindNode("AX", v, 4, v, _selectedNavNode)).ToArray();
						axsnode.AddChildRange(axnodes);
						_selectedNavNode.AddChild(axsnode);
					}
					if (_jobIniRootNodes?.Length > 0)
					{
						var jininode = new BindNode("JOBINI", "Job INI", 3, null, _selectedNavNode);
						GenToBindNodes(_jobIniRootNodes, jininode);
						_selectedNavNode.AddChild(jininode);
					}
					_selectedNavNode.IsExpanded = true;
				}
			}
			else if (_selectedNavNode.Type == "VT")
			{
				// ┌─────────────────────────────────────────────────────────────────┐
				// │ A named vartree is loaded and unwound into the variables tree.  │
				// └─────────────────────────────────────────────────────────────────┘
				var accdata = (AccountData)_selectedNavNode.Parent.Parent.Data;
				var jobdata = (ContainerData)_selectedNavNode.Parent.Data;
				bool jobchange = SelectedAccount.Name != accdata.Name || SelectedJob.Name != jobdata.Name;
				SelectedAccount = accdata;
				SelectedJob = jobdata;
				if (jobchange)
				{
					if (!OpenJob()) return;
				}
				SelectedVartreeName = (string)_selectedNavNode.Data;
				SelectedAxisTreeName = null;
				LoadVartree();
			}
			else if (_selectedNavNode.Type == "AX")
			{
				// ┌───────────────────────────────────────────────────────────────────┐
				// │ A named axis tree is loaded and unwound into the variables tree.  │
				// └───────────────────────────────────────────────────────────────────┘
				var accdata = (AccountData)_selectedNavNode.Parent.Parent.Data;
				var jobdata = (ContainerData)_selectedNavNode.Parent.Data;
				bool jobchange = SelectedAccount.Name != accdata.Name || SelectedJob.Name != jobdata.Name;
				SelectedAccount = accdata;
				SelectedJob = jobdata;
				if (jobchange)
				{
					if (!OpenJob()) return;
				}
				SelectedAxisTreeName = (string)_selectedNavNode.Data;
				SelectedVartreeName = null;
				LoadAxisTree();
			}
			else if (_selectedNavNode.Type == "File")
			{
				// ┌───────────────────────────────────────────────────────────────────┐
				// │ Clicking an arbitrary 'File' with no specific meaning under the   │
				// │ legacy TOC now simply loads the contents of the file/blob as a .  │
				// │ string into the report display window.                            │
				// └───────────────────────────────────────────────────────────────────┘
				GenTabLines = ReadFileAsLines(_selectedNavNode.Text);
				SelectedReportTabIndex = 0;
				Trace.WriteLine($"#### {_selectedNavNode.Text} -> {_genTabLines?.Length}");
			}
			else if (_selectedNavNode.Type == "Table" || _selectedNavNode.Type == "Chart")
			{
				// ┌───────────────────────────────────────────────────────────────────┐
				// │ Legacy TOC file is also a simple lines display.                   │
				// └───────────────────────────────────────────────────────────────────┘
				string fullname = Path.Combine(_selectedNavNode.Description, Path.ChangeExtension(_selectedNavNode.Text, ".rpt"));
				GenTabLines = ReadFileAsLines(fullname);
				SelectedReportTabIndex = 0;
				Trace.WriteLine($"#### {_selectedNavNode.Text} -> {_genTabLines?.Length}");
			}
		}

		void PostVartreeSelect()
		{
			//if (_selectedVartreeNode?.Type == "Variable")
			//{
			//	if (_selectedVartreeNode.Children == null || _selectedVartreeNode.Children.Count == 0)
			//	{
			//		if (!await LoadVarmeta()) return;
			//		if (VMeta.Nodes.FirstOrDefault()?.Children == null) return;     // There might be no children (eg Weights)
			//		if (VMeta.Metadata.Any(m => m.Key == "Type" && m.Value == "Hierarchic"))
			//		{
			//			GenToBindNodes(VMeta.Nodes, _selectedVartreeNode);
			//			_selectedVartreeNode.IsExpanded = true;
			//		}
			//		else
			//		{
			//			var codenodes = GenToBindNodes(VMeta.Nodes[0].Children, null);
			//			_selectedVartreeNode.AddChildRange(codenodes);
			//			_selectedVartreeNode.IsExpanded = true;
			//		}
			//	}
			//}
		}

		public bool OpenJob()
		{
			return WrapCall($"Open cloud job {_selectedAccount.Name}:{_selectedJob.Name}", () =>
			{
				_engine.OpenJob(_selectedAccount.Name, _selectedJob.Name);
				DProps = _engine.GetProps();
				VartreeNames = _engine.ListVartreeNames().ToArray();
				AxisTreeNames = _engine.ListAxisNames().ToArray();
				TocNewRootGenNodes = _engine.ListSavedReports();
				TocOldRootGenNodes = _engine.GetLegacyTocAsNodes();
				JobIniRootNodes = _engine.GetJobIniAsNodes();
				string accjobKey = $"{_selectedAccount.Name}:{_selectedJob.Name}";
				Trace.WriteLine($"Opened job {accjobKey}");
				OpenError = null;
				CommandManager.InvalidateRequerySuggested();    // Makes the GenTab Run button enable
			}, (ex) =>
			{
				DProps = null;
				TocOldRootGenNodes = null;
				AxisTreeNames = null;
				OpenError = ex;
			});
		}

		public bool LoadVartree()
		{
			return WrapCall($"Load vartree {_selectedVartreeName}", () =>
			{
				var gnodes = _engine.GetVartreeAsNodes(_selectedVartreeName);
				var broots = GenToBindNodes(gnodes, null).ToArray();
				ObsVartreeNodes = new ObservableCollection<BindNode>(broots);
				VtError = null;
				Trace.WriteLine($"Vartree root node count -> {ObsVartreeNodes.Count}");
			}, (ex) =>
			{
				VtError = ex;
				ObsVartreeNodes = null;
			});
		}

		public bool LoadAxisTree()
		{
			return WrapCall($"Load axis tree {_selectedAxisTreeName}", () =>
			{
				var gnodes = _engine.GetAxisAsNodes(_selectedAxisTreeName);
				var broots = GenToBindNodes(gnodes, null).ToArray();
				ObsVartreeNodes = new ObservableCollection<BindNode>(broots);
				VtError = null;
				Trace.WriteLine($"Axis tree root node count -> {ObsVartreeNodes.Count}");
			}, (ex) =>
			{
				VtError = ex;
				ObsVartreeNodes = null;
			});
		}

		public bool RunSpec()
		{
			return false;
			//return await WrapCall("Run Report", async () =>
			//{
			//	DProps.Output.Format = _selectedOutputFormat;
			//	Settings.Put(null, nameof(ReportTop), _reportTop);
			//	Settings.Put(null, nameof(ReportSide), _reportSide);
			//	Settings.Put(null, nameof(UseFilter), _useFilter);
			//	Settings.Put(null, nameof(UseWeight), _useWeight);
			//	GenTabLines = await _client.GenTab(null, ReportTop, ReportSide, _useFilter ? ReportFilter : null, _useWeight ? ReportWeight : null, _sProps, _dProps);
			//	GenTabError = null;
			//	switch (_selectedOutputFormat)
			//	{
			//		case XOutputFormat.HTML:
			//			SelectedReportTabIndex = 1;
			//			break;
			//		default:
			//			SelectedReportTabIndex = 0;
			//			break;
			//	}
			//	CommandManager.InvalidateRequerySuggested();
			//}, (ex) =>
			//{
			//	GenTabLines = null;
			//	GenTabError = ex;
			//});
		}

		public bool ListSavedReports()
		{
			return false;
			//return await WrapCall($"List Saved Reports", async () =>
			//{
			//	// The list of saved reports may be useful in some future save UI
			//	SavedReportList = await _client.ListReports();
			//	ListSavedReportsError = null;
			//}, (ex) =>
			//{
			//	SavedReportList = null;
			//	ListSavedReportsError = ex;
			//});
		}

		public bool SaveReport(string path, string name)
		{
			return false;
			//return await WrapCall($"Save Report", async () =>
			//{
			//	await _client.SaveReport(path, name);
			//}, (ex) =>
			//{
			//});
		}

		/// <summary>
		/// Plain API call to read a file as a string.
		/// </summary>
		public string[] ReadFileAsLines(string name)
		{
			return _engine.ReadFileAsLines(name);
		}

		/// <summary>
		/// Needs to return an array or an enumerable won't be evaluated.
		/// </summary>
		BindNode[] GenToBindNodes(IEnumerable<GenNode> nodes, BindNode parent)
		{
			var nodelist = new List<BindNode>();
			foreach (var node in nodes)
			{
				var bnode = new BindNode(node, null, parent);
				if (parent != null)
				{
					parent.AddChild(bnode);
				}
				if (node.Children?.Count > 0)
				{
					GenToBindNodes(node.Children, bnode);
				}
				nodelist.Add(bnode);
			}
			return nodelist.ToArray();
		}

		bool WrapCall(string title, Action callback, Action<Exception> errorCallback = null)
		{
			try
			{
				BusyMessage = $"{title}\u2026";
				callback();
				return true;
			}
			catch (CarbonException ex)
			{
				Trace.WriteLine(ex.ToString());
				ErrorTitle = title;
				ErrorMessage = ex.Message;
				errorCallback?.Invoke(ex);
				return false;
			}
			finally
			{
				BusyMessage = null;
			}
		}
	}
}
