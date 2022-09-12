using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using Orthogonal.NSettings;
using RCS.Carbon.Shared;
using RCS.Carbon.Tables;

namespace Carbon.Example.Desktop
{
	/// <summary>
	/// Following the conventions of MVVM style coding, this class is the data context for the WPF UI.
	/// Classical binding properties and methods hold and manage the "state" of the application.
	/// For more information see the <b>Controller</b> section of the GitHub project Wiki.
	/// </summary>
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
		}

		public void Startup()
		{
			appTimer = new DispatcherTimer()
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			appTimer.Tick += AppTimer_Tick;
			appTimer.Start();
			StatusAccount = "Not logged in";
		}

		public void Shutdown()
		{
			_engine?.CloseJob();
			appTimer?.Stop();
		}

		void AppTimer_Tick(object sender, EventArgs e)
		{
			StatusTime = DateTime.Now.ToString("HH:mm:ss");
		}

		/// <summary>
		/// Some of the binding properties have values saved and loaded via a 'settings'
		/// library. The Orthogonal library is simple and public and created as an independent
		/// personal project by one of Red Centre Software's developers. That library can be
		/// replaced with any other 'settings' technique, including the built-in Settings class
		/// that can be added to a VS project.
		/// </summary>
		void ReloadSettings()
		{
			// The login credentials can optionally be stored in user environment variables.
			// This may be useful for repetitive testing on different computers.
			_loginId = Settings.Get(null, nameof(LoginId), Environment.GetEnvironmentVariable("RCSTESTID") ?? "10000858");
			OnPropertyChanged(nameof(LoginId));
			_loginPassword = Settings.Get(null, nameof(LoginPassword), Environment.GetEnvironmentVariable("RCSTESTPASS") ?? "guest");
			OnPropertyChanged(nameof(LoginPassword));

			_appFontSize = Settings.GetInt(null, nameof(AppFontSize), 13);
			OnPropertyChanged(nameof(AppFontSize));
			_reportFontSize = Settings.GetInt(null, nameof(ReportFontSize), 13);
			OnPropertyChanged(nameof(ReportFontSize));
			_selectedOutputFormat = (XOutputFormat)Enum.Parse(typeof(XOutputFormat), Settings.Get(null, nameof(SelectedOutputFormat), XOutputFormat.CSV.ToString()));
			OnPropertyChanged(nameof(SelectedOutputFormat));
			_reportTop = Settings.Get(null, nameof(ReportTop), "Age(*)");
			OnPropertyChanged(nameof(ReportTop));
			_reportSide = Settings.Get(null, nameof(ReportSide), "Region(*)");
			OnPropertyChanged(nameof(ReportSide));
			_reportFilter = Settings.Get(null, nameof(ReportFilter), null);
			OnPropertyChanged(nameof(ReportFilter));
			_reportWeight = Settings.Get(null, nameof(ReportWeight), null);
			OnPropertyChanged(nameof(ReportWeight));
			_useFilter = Settings.GetBool(null, nameof(UseFilter), false);
			OnPropertyChanged(nameof(UseFilter));
			_useWeight = Settings.GetBool(null, nameof(UseWeight), false);
			OnPropertyChanged(nameof(UseWeight));
		}

		void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		#endregion

		#region Controller Public

		/// <summary>
		/// Attempts to authenticate credentials against the Red Centre Software Licensing service.
		/// On success, the navigation tree is loaded with a set of publicly visible Cloud jobs
		/// and the app enters a state where it can continue and do useful work.
		/// </summary>
		public async Task<bool> LoginAsync(string id, string password)
		{
			try
			{
				Lic = await Engine.LoginId(id, password);
				StatusAccount = $"Account {_lic.Id} ({_lic.Name})";
				LoginError = null;
				BusyMessage = null;
				LoadNavigationTree();
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

		public void DismissError()
		{
			ErrorTitle = ErrorMessage = null;
		}

		/// <summary>
		/// Opens a Cloud job identified by customer and job names. A successful open then calls for
		/// more information related to the job so that all available information about the job can be
		/// added as branches under the job node. This helps illustrate what full information is
		/// aavailable about a job.
		/// </summary>
		public bool OpenJob()
		{
			return WrapCall($"Open cloud job {_selectedCust.Name}:{_selectedJob.Name}", () =>
			{
				_engine.OpenJob(_selectedCust.Name, _selectedJob.Name);
				DProps = _engine.GetProps();
				VartreeNames = _engine.ListVartreeNames().ToArray();
				AxisTreeNames = _engine.ListAxisNames().ToArray();
				TocNewRootGenNodes = _engine.ListSavedReports();
				TocOldRootGenNodes = _engine.GetLegacyTocAsNodes();
				JobIniRootNodes = _engine.GetJobIniAsNodes();
				StatusMessage = $"{_selectedCust.Name} + {_selectedJob.Name}";
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

		/// <summary>
		/// A names variable tree is returned as a 'tree' of generic nodes
		/// which are converted into bindable nodes for display.
		/// </summary>
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

		/// <summary>
		/// A named axis collection of variables is returned as a 'tree' of generic nodes
		/// which are converted into bindable nodes for display.
		/// </summary>
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

		/// <summary>
		/// TODO
		/// </summary>
		public bool LoadVarmeta()
		{
			if (_selectedVartreeNode?.Type != "Variable") return false;
			return WrapCall($"Loading varmeta {_selectedVartreeNode.Text}", () =>
			{
				VMeta = _engine.GetVarMetaParsed(_selectedVartreeNode.Text);
				VmetaError = null;
			}, (ex) =>
			{
				VMeta = null;
				VmetaError = ex;
			});
		}

		/// <summary>
		/// TODO
		/// </summary>
		public bool RunSpec()
		{
			return WrapCall("Run Report", () =>
			{
				DProps.Output.Format = _selectedOutputFormat;
				Settings.Put(null, nameof(ReportTop), _reportTop);
				Settings.Put(null, nameof(ReportSide), _reportSide);
				Settings.Put(null, nameof(UseFilter), _useFilter);
				Settings.Put(null, nameof(UseWeight), _useWeight);
				string reportBody = null;
				string filter = _useFilter ? ReportFilter : null;
				string weight = _useWeight ? ReportWeight : null;
				switch (_selectedOutputFormat)
				{
					case XOutputFormat.XML:
						XDocument doc = _engine.GenTabAsXML(null, _reportTop, _reportSide, filter, weight, _sProps, _dProps);
						reportBody = doc.ToString();
						break;
					case XOutputFormat.XLSX:
						byte[] workbook = _engine.GenTabAsXLSX(null, _reportTop, _reportSide, filter, weight, _sProps, _dProps);
						LastXlsxSaveFile = new FileInfo(Path.Combine(Path.GetTempPath(), "_carbon_example_desktop.xlsx"));
						File.WriteAllBytes(LastXlsxSaveFile.FullName, workbook);
						GenTabLines = HexToLines(workbook).ToArray();
						break;
					default:
						reportBody = _engine.GenTab(null, _reportTop, _reportSide, filter, weight, _sProps, _dProps);
						GenTabLines = TextToLines(reportBody).ToArray();
						break;
				}
				GenTabError = null;
				switch (_selectedOutputFormat)
				{
					case XOutputFormat.HTML:
						SelectedReportTabIndex = 1;
						break;
					default:
						SelectedReportTabIndex = 0;
						break;
				}
				CommandManager.InvalidateRequerySuggested();
			}, (ex) =>
			{
				GenTabLines = null;
				GenTabError = ex;
			});
		}

		/// <summary>
		/// TODO
		/// </summary>
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

		/// <summary>
		/// TODO
		/// </summary>
		public bool SaveReport(string path, string name)
		{
			return WrapCall($"Save Report", () =>
			{
				_engine.TableSaveCBT(path, name);
				TocNewRootGenNodes = _engine.ListSavedReports();
				RefreshOpenJobToc();
			}, (ex) =>
			{
			});
		}

		/// <summary>
		/// Plain API call to read a file as a string.
		/// </summary>
		public string[] ReadFileAsLines(string name)
		{
			return _engine.ReadFileAsLines(name);
		}

		#endregion

		#region Node Selection Handlers

		BindNode lastOpenedJobNode;

		/// <summary>
		/// A variety of processing in response to clicks on certain navigation tree node types.
		/// This method is unfortunately very long because it has to 'crack' the navigation
		/// node that was selected and take appropriate action. The comments within the method
		/// hopefully provide a summary of what happens for different nodes.
		/// </summary>
		void PostNavSelect()
		{
			if (_selectedNavNode == null)
			{
				SelectedCust = null;
				SelectedJob = null;
				SelectedVartreeName = null;
				SelectedAxisTreeName = null;
				ObsVartreeNodes = null;
				return;
			}
			if (_selectedNavNode.Type == BindNode.TypeCust)
			{
				// We used to dynamically load the jobs on a customer click,
				// but they are preloaded, so we do nothing now.
			}
			else if (_selectedNavNode.Type == BindNode.TypeJob)
			{
				// ┌───────────────────────────────────────────────────────────────┐
				// │ A variety of child information for a job is unwound into      │
				// │ branches of child nodes under the job.                        │
				// └───────────────────────────────────────────────────────────────┘
				lastOpenedJobNode = _selectedNavNode;
				var accdata = (CustData)_selectedNavNode.Parent.Data;
				SelectedCust = accdata;
				var jobdata = (JobData)_selectedNavNode.Data;
				SelectedJob = jobdata;
				SelectedVartreeName = null;
				SelectedAxisTreeName = null;
				ObsVartreeNodes = null;
				if (!OpenJob()) return;
				if (!_selectedNavNode.AnyChildren)
				{
					RefreshOpenJobToc();
					if (_tocOldRootGenNodes != null)
					{
						var tocnode = new BindNode(BindNode.TypeTocOld, "TOC (Legacy)", 3, null, _selectedNavNode);
						GenToBindNodes(_tocOldRootGenNodes, tocnode);
						_selectedNavNode.AddChild(tocnode);
					}
					if (_vartreeNames?.Length > 0)
					{
						var vtsnode = new BindNode(BindNode.TypeVartees, "Vartrees", 3, null, _selectedNavNode);
						var vtnodes = _vartreeNames.Select(v => new BindNode(BindNode.TypeVt, v, 4, v, _selectedNavNode)).ToArray();
						vtsnode.AddChildRange(vtnodes);
						_selectedNavNode.AddChild(vtsnode);
					}
					if (_axisTreeNames?.Length > 0)
					{
						var axsnode = new BindNode(BindNode.TypeAxTrees, "Axis Trees", 3, null, _selectedNavNode);
						var axnodes = _axisTreeNames.Select(v => new BindNode(BindNode.TypeAx, v, 4, v, _selectedNavNode)).ToArray();
						axsnode.AddChildRange(axnodes);
						_selectedNavNode.AddChild(axsnode);
					}
					if (_jobIniRootNodes?.Length > 0)
					{
						var jininode = new BindNode(BindNode.TypeIni, "Job INI", 3, null, _selectedNavNode);
						GenToBindNodes(_jobIniRootNodes, jininode);
						_selectedNavNode.AddChild(jininode);
					}
					_selectedNavNode.IsExpanded = true;
				}
			}
			else if (_selectedNavNode.Type == BindNode.TypeVt)
			{
				// ┌─────────────────────────────────────────────────────────────────┐
				// │ A named vartree is loaded and unwound into the variables tree.  │
				// └─────────────────────────────────────────────────────────────────┘
				var accdata = (CustData)_selectedNavNode.Parent.Parent.Data;
				var jobdata = (JobData)_selectedNavNode.Parent.Data;
				bool jobchange = SelectedCust.Name != accdata.Name || SelectedJob.Name != jobdata.Name;
				SelectedCust = accdata;
				SelectedJob = jobdata;
				if (jobchange)
				{
					if (!OpenJob()) return;
				}
				SelectedVartreeName = (string)_selectedNavNode.Data;
				SelectedAxisTreeName = null;
				LoadVartree();
			}
			else if (_selectedNavNode.Type == BindNode.TypeAx)
			{
				// ┌───────────────────────────────────────────────────────────────────┐
				// │ A named axis tree is loaded and unwound into the variables tree.  │
				// └───────────────────────────────────────────────────────────────────┘
				var accdata = (CustData)_selectedNavNode.Parent.Parent.Data;
				var jobdata = (JobData)_selectedNavNode.Parent.Data;
				bool jobchange = SelectedCust.Name != accdata.Name || SelectedJob.Name != jobdata.Name;
				SelectedCust = accdata;
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
			}
			else if (_selectedNavNode.Type == "Table" || _selectedNavNode.Type == "Chart")
			{
				// ┌───────────────────────────────────────────────────────────────────┐
				// │ Legacy TOC file is also a simple lines display.                   │
				// └───────────────────────────────────────────────────────────────────┘
				string fullname = Path.Combine(_selectedNavNode.Description, Path.ChangeExtension(_selectedNavNode.Text, ".rpt"));
				GenTabLines = ReadFileAsLines(fullname);
				SelectedReportTabIndex = 0;
			}
		}

		/// <summary>
		/// Create or refill the TOC node under the currently open job's navigation node.
		/// This can happen when then job is first opened, or if the set of saved reports
		/// is changed and the tree branch much refresh to match the data.
		/// </summary>
		void RefreshOpenJobToc()
		{
			var tocnode = lastOpenedJobNode.Children.FirstOrDefault(n => n.Type == BindNode.TypeTocNew);
			if (tocnode == null)
			{
				tocnode = new BindNode(BindNode.TypeTocNew, "TOC", 3, null, _selectedNavNode);
				lastOpenedJobNode.AddChild(tocnode);
			}
			else
			{
				tocnode.Children?.Clear();
				tocnode.IsExpanded = true;
			}
			if (_tocNewRootGenNodes != null)
			{
				GenToBindNodes(_tocNewRootGenNodes, tocnode);
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		void PostVartreeSelect()
		{
			if (_selectedVartreeNode?.Type == "Variable")
			{
				if (_selectedVartreeNode.Children == null || _selectedVartreeNode.Children.Count == 0)
				{
					if (!LoadVarmeta()) return;
					if (VMeta.RootNodes.FirstOrDefault()?.Children == null) return;     // There might be no children (eg Weights)
					if (VMeta.Metadata.Any(m => m.Name == "Type" && m.Value == "Hierarchic"))
					{
						GenToBindNodes(VMeta.RootNodes, _selectedVartreeNode);
						_selectedVartreeNode.IsExpanded = true;
					}
					else
					{
						var codenodes = GenToBindNodes(VMeta.RootNodes[0].Children, null);
						_selectedVartreeNode.AddChildRange(codenodes);
						_selectedVartreeNode.IsExpanded = true;
					}
				}
			}
		}

		#endregion

		#region Private Helpers

		/// <summary>
		/// The navigation tree is initially loaded with some Cloud customers and
		/// jobs that are public and provided by Red Centre Software.
		/// </summary>
		void LoadNavigationTree()
		{
			ObsNavNodes = new ObservableCollection<BindNode>();
			var doc = XDocument.Load("Navigation-Tree.xml");
			var rnode = new BindNode(BindNode.TypeCloud, "Cloud", 0, null, null);
			foreach (var custelem in doc.Root.Elements("customer"))
			{
				var cust = new CustData() { Name = (string)custelem.Attribute("name") };
				var cnode = new BindNode(BindNode.TypeCust, cust.Name, 0, cust, rnode);
				rnode.AddChild(cnode);
				foreach (var jelem in custelem.Elements("job"))
				{
					var job = new JobData((string)jelem.Attribute("name"), null, null, null);
					var jnode = new BindNode(BindNode.TypeJob, job.Name, 1, job, cnode);
					cnode.AddChild(jnode);
				}
				cnode.IsExpanded = true;
			}
			rnode.IsExpanded = true;
			_obsNavNodes.Add(rnode);
		}

		/// <summary>
		/// Converts a 'tree' of Carbon generic nodes into an equivalent tree
		/// of bindable nodes for display in TreeView or similar controls.
		/// Note that it needs to return an array because an enumerable return
		/// may not be evaluated.
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

		/// <summary>
		/// This private helper method wraps around the repetitive processing of making a call and
		/// trapping any failure. The caller provides optional callbacks for success or failure
		/// (this technique is familiar in JavaScript coding). A failure causes some Error properties
		/// to be set, which will cause a panel to visible in the UI.
		/// </summary>
		bool WrapCall(string title, Action callback, Action<Exception> errorCallback = null)
		{
			try
			{
				BusyMessage = $"{title}\u2026";
				callback?.Invoke();
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

		static IEnumerable<string> TextToLines(string value)
		{
			if (value != null)
			{
				using (var reader = new StringReader(value))
				{
					string line = reader.ReadLine();
					while (line != null)
					{
						yield return line;
						line = reader.ReadLine();
					}
				}
			}
		}

		static IEnumerable<string> HexToLines(byte[] buffer)
		{
			if (buffer != null)
			{
				int count = buffer.Length / 64;
				for (int i = 0; i < count; i++)
				{
					string line = Convert.ToBase64String(buffer, i * 64, 64);
					yield return line;
				}
			}
		}

		#endregion

		CrossTabEngine _engine;
		/// <summary>
		/// The Carbon cross-tabulation engine is created on first demand, which we know will be the
		/// first Login call attempt. This happens through a modal dialog in the UI, so after a
		/// successful login this property will be created and all following calls can be made
		/// </summary>
		public CrossTabEngine Engine => LazyInitializer.EnsureInitialized(ref _engine, () =>
		{
			var eng = new CrossTabEngine();
			StatusEngine = $"Carbon {eng.Version}";
			return eng;
		});

		#region Binding Properties

		public FileInfo LastXlsxSaveFile { get; set; }

		/// <summary>
		/// Items source for the report format picker.
		/// </summary>
		public XOutputFormat[] FormatPicks { get; } = new XOutputFormat[]		// NOTE --> Same order as the enum
		{
			XOutputFormat.None,
			XOutputFormat.TSV,
			XOutputFormat.CSV,
			XOutputFormat.SSV,
			XOutputFormat.XLSX,
			XOutputFormat.XML,
			XOutputFormat.HTML,
			XOutputFormat.OXT,
			XOutputFormat.Pandas
		};

		/// <summary>
		/// Item source for the report property significance type picker.
		/// </summary>
		public XSigType[] SigTypePicks { get; } = new XSigType[]
		{
			XSigType.SingleCell,
			XSigType.ColumnGroups,
			XSigType.RefColumn,
			XSigType.RefRow,
			XSigType.RowGroups
		};

		#endregion
	}
}
