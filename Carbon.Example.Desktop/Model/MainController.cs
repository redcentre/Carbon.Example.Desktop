﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
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
		BindNode lastOpenedJobNode;

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
				AppError = null;
				LoadNavigationTree();
				return true;
			}
			catch (Exception ex)
			{
				ErrorTitle = "Login Error";
				AppError = ex;
				Trace.WriteLine(ex.Message);
				BusyMessage = null;
				return false;
			}
		}

		public void DismissError()
		{
			ErrorTitle = null;
			AppError = null;
		}

		/// <summary>
		/// Opens a Cloud job identified by customer and job names. A successful open then calls for
		/// more information related to the job so that all available information about the job can be
		/// added as branches under the job node. This helps illustrate what full information is
		/// aavailable about a job.
		/// </summary>
		public async Task<bool> OpenJob()
		{
			BusyMessage = $"Open cloud job {_selectedCust.Name}:{_selectedJob.Name}";
			try
			{
				await Task.Run(() => Engine.OpenJob(_selectedCust.Name, _selectedJob.Name));
				var t1 = Task.Run(() => Engine.GetProps());
				var t2 = Task.Run(() => Engine.ListVartreeNames().ToArray());
				var t3 = Task.Run(() => Engine.ListAxisNames().ToArray());
				var t4 = Task.Run(() => Engine.ListSavedReports());
				var t5 = Task.Run(() => Engine.GetLegacyTocAsNodes());
				var t6 = Task.Run(() => Engine.GetJobIniAsNodes());
				await Task.WhenAll(t1, t2, t3, t4, t5, t6);
				DProps = t1.Result;
				VartreeNames = t2.Result;
				AxisTreeNames = t3.Result;
				TocNewRootGenNodes = t4.Result;
				TocOldRootGenNodes = t5.Result;
				JobIniRootNodes = t6.Result;
				StatusMessage = $"{_selectedCust.Name} + {_selectedJob.Name}";
				AppError = null;
				CommandManager.InvalidateRequerySuggested();    // Makes the GenTab Run button enable
				return true;
			}
			catch (CarbonException ex)
			{
				DProps = null;
				TocOldRootGenNodes = null;
				AxisTreeNames = null;
				ErrorTitle = "Open cloud job failed";
				AppError = ex;
				return false;
			}
			finally
			{
				BusyMessage = null;
			}
		}

		/// <summary>
		/// A names variable tree is returned as a 'tree' of generic nodes
		/// which are converted into bindable nodes for display.
		/// </summary>
		public async Task<bool> LoadVartree()
		{
			BusyMessage = $"Load vartree {_selectedVartreeName}";
			try
			{
				var gnodes = await Task.Run(() => Engine.GetVartreeAsNodes(_selectedVartreeName));
				var broots = GenToBindNodes(gnodes, null).ToArray();
				ObsVartreeNodes = new ObservableCollection<BindNode>(broots);
				AppError = null;
				return true;
			}
			catch (CarbonException ex)
			{
				ErrorTitle = "Load variable tree failed";
				AppError = ex;
				ObsVartreeNodes = null;
				return false;
			}
			finally
			{
				BusyMessage = null;
			}
		}

		/// <summary>
		/// A named axis collection of variables is returned as a 'tree' of generic nodes
		/// which are converted into bindable nodes for display.
		/// </summary>
		public async Task<bool> LoadAxisTree()
		{
			BusyMessage = $"Load axis tree {_selectedAxisTreeName}";
			try
			{
				var gnodes = await Task.Run(() => Engine.GetAxisAsNodes(_selectedAxisTreeName));
				var broots = GenToBindNodes(gnodes, null).ToArray();
				ObsVartreeNodes = new ObservableCollection<BindNode>(broots);
				AppError = null;
				return true;
			}
			catch (CarbonException ex)
			{
				ErrorTitle = "Load axis tree failed";
				AppError = ex;
				ObsVartreeNodes = null;
				return false;
			}
			finally
			{
				BusyMessage = null;
			}
		}

		/// <summary>
		/// Variable metadata and child variables and codes are loaded for the selected variable.
		/// </summary>
		public async Task<bool> LoadVarmeta()
		{
			if (_selectedVartreeNode?.Type != "Variable") return false;
			BusyMessage = $"Loading varmeta {_selectedVartreeNode.Text}";
			try
			{
				VMeta = await Task.Run(() => Engine.GetVarMetaParsed(_selectedVartreeNode.Text));
				AppError = null;
				return true;
			}
			catch (CarbonException ex)
			{
				ErrorTitle = "Load variable metadata failed";
				VMeta = null;
				AppError = ex;
				return false;
			}
			finally
			{
				BusyMessage = null;
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		public async Task<bool> RunSpecAsync()
		{
			BusyMessage = "Generate Report";
			try
			{
				DProps.Output.Format = _selectedOutputFormat;
				Settings.Put(null, nameof(ReportTop), _reportTop);
				Settings.Put(null, nameof(ReportSide), _reportSide);
				Settings.Put(null, nameof(UseFilter), _useFilter);
				Settings.Put(null, nameof(UseWeight), _useWeight);
				string reportBody = null;
				string filter = _useFilter ? ReportFilter : null;
				string weight = _useWeight ? ReportWeight : null;
				ReportTitle = $"{lastOpenedJobNode.Text} {_dProps.Output.Format}";
				switch (_selectedOutputFormat)
				{
					case XOutputFormat.XML:
						XDocument doc = await Task.Run(() => Engine.GenTabAsXML(null, _reportTop, _reportSide, filter, weight, _sProps, _dProps));
						reportBody = doc.ToString();
						GenTabLines = TextToLines(reportBody).ToArray();
						break;
					case XOutputFormat.XLSX:
						byte[] workbook = await Task.Run(() => Engine.GenTabAsXLSX(null, _reportTop, _reportSide, filter, weight, _sProps, _dProps));
						LastXlsxSaveFile = new FileInfo(Path.Combine(Path.GetTempPath(), "_carbon_example_desktop.xlsx"));
						File.WriteAllBytes(LastXlsxSaveFile.FullName, workbook);
						ReportTitle = $"{lastOpenedJobNode.Text} Excel Workbook ({workbook.Length} bytes)";
						GenTabLines = HexToLines(workbook).ToArray();
						break;
					default:
						reportBody = await Task.Run(() => Engine.GenTab(null, _reportTop, _reportSide, filter, weight, _sProps, _dProps));
						GenTabLines = TextToLines(reportBody).ToArray();
						break;
				}
				AppError = null;
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
				return true;
			}
			catch (CarbonException ex)
			{
				GenTabLines = null;
				ErrorTitle = "Generate report failed";
				AppError = ex;
				return false;
			}
			finally
			{
				BusyMessage = null;
			}
		}

		/// <summary>
		/// NOT IMPLEMENTED YET
		/// </summary>
		public BindNode[] ListSavedReports()
		{
			var tocnode = lastOpenedJobNode?.Children.FirstOrDefault(n => n.Type == BindNode.TypeTocNew);
			return null;
		}

		/// <summary>
		/// The last run report is saved. It can be a simple name like 'My Report' or
		/// it can be path qualified like 'January/Invoices/My Report'.
		/// </summary>
		public async Task<bool> SaveReport(string name)
		{
			BusyMessage = "Save Report";
			try
			{
				await Task.Run(() => Engine.TableSaveCBT(name));
				TocNewRootGenNodes = Engine.ListSavedReports();
				RefreshOpenJobToc();
				return true;
			}
			catch (CarbonException ex)
			{
				ErrorTitle = "Save report failed";
				AppError = ex;
				return false;
			}
			finally
			{
				BusyMessage = null;
			}
		}

		/// <summary>
		/// The name of a saved report as it appears in the navigation tree is deleted.
		/// It can be a simple name like 'My Report' or it can be path qualified like
		/// 'January/Invoices/My Report'.
		/// </summary>
		public void DeleteReport(string name)
		{
			Engine.DeleteCBT(name);
			TocNewRootGenNodes = Engine.ListSavedReports();
			RefreshOpenJobToc();
		}

		/// <summary>
		/// Plain API call to read a file as a string.
		/// </summary>
		public string[] ReadFileAsLines(string name)
		{
			return Engine.ReadFileAsLines(name);
		}

		#endregion

		#region Node Selection Handlers

		/// <summary>
		/// A variety of processing in response to clicks on certain navigation tree node types.
		/// This method is unfortunately very long because it has to 'crack' the navigation
		/// node that was selected and take appropriate action. The comments within the method
		/// hopefully provide a summary of what happens for different nodes.
		/// </summary>
		async Task<bool> PostNavSelect()
		{
			if (_selectedNavNode == null)
			{
				SelectedCust = null;
				SelectedJob = null;
				SelectedVartreeName = null;
				SelectedAxisTreeName = null;
				ObsVartreeNodes = null;
				return false;
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
				if (!await OpenJob()) return false;
				if (!_selectedNavNode.AnyChildren)
				{
					RefreshOpenJobToc();
					if (_tocOldRootGenNodes != null)
					{
						var tocnode = new BindNode(BindNode.TypeTocOld, "TOC (Legacy)", null, _selectedNavNode);
						GenToBindNodes(_tocOldRootGenNodes, tocnode);
						_selectedNavNode.AddChild(tocnode);
					}
					if (_vartreeNames?.Length > 0)
					{
						var vtsnode = new BindNode(BindNode.TypeVartees, "Vartrees", null, _selectedNavNode);
						var vtnodes = _vartreeNames.Select(v => new BindNode(BindNode.TypeVt, v, v, _selectedNavNode)).ToArray();
						vtsnode.AddChildRange(vtnodes);
						_selectedNavNode.AddChild(vtsnode);
					}
					if (_axisTreeNames?.Length > 0)
					{
						var axsnode = new BindNode(BindNode.TypeAxTrees, "Axis Trees", null, _selectedNavNode);
						var axnodes = _axisTreeNames.Select(v => new BindNode(BindNode.TypeAx, v, v, _selectedNavNode)).ToArray();
						axsnode.AddChildRange(axnodes);
						_selectedNavNode.AddChild(axsnode);
					}
					if (_jobIniRootNodes?.Length > 0)
					{
						var jininode = new BindNode(BindNode.TypeIni, "Job INI", null, _selectedNavNode);
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
					if (!await OpenJob()) return false;
				}
				SelectedVartreeName = (string)_selectedNavNode.Data;
				SelectedAxisTreeName = null;
				await LoadVartree();
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
					if (!await OpenJob()) return false;
				}
				SelectedAxisTreeName = (string)_selectedNavNode.Data;
				SelectedVartreeName = null;
				await LoadAxisTree();
			}
			else if (_selectedNavNode.Type == "File")
			{
				// ┌───────────────────────────────────────────────────────────────────┐
				// │ Clicking an arbitrary 'File' with no specific meaning under the   │
				// │ legacy TOC now simply loads the contents of the file/blob as a    │
				// │ string into the report display window.                            │
				// └───────────────────────────────────────────────────────────────────┘
				ReportTitle = _selectedNavNode.Text;
				GenTabLines = ReadFileAsLines(_selectedNavNode.Text);
				SelectedReportTabIndex = 0;
			}
			else if (_selectedNavNode.Type == "Table" || _selectedNavNode.Type == "Chart")
			{
				// ┌───────────────────────────────────────────────────────────────────┐
				// │ Legacy TOC file is also a simple lines display.                   │
				// └───────────────────────────────────────────────────────────────────┘
				ReportTitle = Path.Combine(_selectedNavNode.Description, Path.ChangeExtension(_selectedNavNode.Text, ".rpt"));
				GenTabLines = ReadFileAsLines(ReportTitle);
				SelectedReportTabIndex = 0;
			}
			return true;
		}

		/// <summary>
		/// Create or refill the TOC node under the currently open job's navigation node.
		/// This can happen when then job is first opened, or if the set of saved reports
		/// is changed and the tree branch much refresh to match the data.
		/// </summary>
		void RefreshOpenJobToc()
		{
			var tocnode = lastOpenedJobNode?.Children.FirstOrDefault(n => n.Type == BindNode.TypeTocNew);
			if (tocnode == null)
			{
				tocnode = new BindNode(BindNode.TypeTocNew, "TOC", null, _selectedNavNode);
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
		/// Selecting a variable in the variable tree for the first time causes the meta data and codes for
		/// that variable to be loaded and added a child branch under the variable. Note that there is a
		/// small complication because there are two types of variable nodes and they have different child
		/// node tree shapes. See the comments below.
		/// </summary>
		async Task<bool> PostVartreeSelect()
		{
			if (_selectedVartreeNode?.Type == "Variable")
			{
				if (_selectedVartreeNode.Children == null || _selectedVartreeNode.Children.Count == 0)
				{
					if (!await LoadVarmeta()) return false;
					if (VMeta.RootNodes.FirstOrDefault()?.Children == null) return false;     // There might be no children (eg Weights)
					if (VMeta.Metadata.Any(m => m.Name == "Type" && m.Value == "Hierarchic"))
					{
						// The varmeta returned for a 'hierarchic' variable has child
						// variables which are added as children of the selected node.
						GenToBindNodes(VMeta.RootNodes, _selectedVartreeNode);
						_selectedVartreeNode.IsExpanded = true;
					}
					else
					{
						// The varmeta returned for a 'simple' variables has a single variable child
						// which has child codes. We only want the 2nd level codea to be added as
						// children of the selected node.
						var codenodes = GenToBindNodes(VMeta.RootNodes[0].Children, null);
						_selectedVartreeNode.AddChildRange(codenodes);
						_selectedVartreeNode.IsExpanded = true;
					}
				}
				return true;
			}
			return false;
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
			var rnode = new BindNode(BindNode.TypeCloud, "Cloud", null, null);
			foreach (var custelem in doc.Root.Elements("customer"))
			{
				var cust = new CustData() { Name = (string)custelem.Attribute("name") };
				var cnode = new BindNode(BindNode.TypeCust, cust.Name, cust, rnode);
				rnode.AddChild(cnode);
				foreach (var jelem in custelem.Elements("job"))
				{
					var job = new JobData((string)jelem.Attribute("name"), null, null, null);
					var jnode = new BindNode(BindNode.TypeJob, job.Name, job, cnode);
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
			const int Len = 64;
			if (buffer != null)
			{
				int count = buffer.Length / Len;
				int rem = buffer.Length % Len;
				for (int i = 0; i < count; i++)
				{
					int off = i * Len;
					string hex = BitConverter.ToString(buffer, off, 64).Replace("-", "");
					string line = $"{off:X6} {hex}";
					yield return line;
				}
				if (rem > 0)
				{
					int off = count * Len;
					string hex = BitConverter.ToString(buffer, off, rem).Replace("-", "");
					string line = $"{off:X6} {hex}";
					yield return line;
				}
			}
		}

		#endregion

		#region Properties

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