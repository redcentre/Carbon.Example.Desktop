﻿//================================================================================================
// <auto-generated>
// This code was generated by a tool on machine HERON at local time 10/09/2022 16:30:15.
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//================================================================================================
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using RCS.Carbon.Shared;
using RCS.Carbon.Tables;
using RCS.Carbon.Variables;

namespace Carbon.Example.Desktop
{
	partial class MainController
	{
		int _appFontSize = 13;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public int AppFontSize
		{
			get => _appFontSize;
			set
			{
				if (_appFontSize != value)
				{
					_appFontSize = value;
					OnPropertyChanged(nameof(AppFontSize));
					Settings.Put(null, nameof(AppFontSize), _appFontSize);
				}
			}
		}

		string _statusMessage = "Loading...";
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string StatusMessage
		{
			get => _statusMessage;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_statusMessage != newval)
				{
					_statusMessage = newval;
					OnPropertyChanged(nameof(StatusMessage));
				}
			}
		}

		string _statusTime = "Loading...";
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string StatusTime
		{
			get => _statusTime;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_statusTime != newval)
				{
					_statusTime = newval;
					OnPropertyChanged(nameof(StatusTime));
				}
			}
		}

		string _busyMessage;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string BusyMessage
		{
			get => _busyMessage;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_busyMessage != newval)
				{
					_busyMessage = newval;
					OnPropertyChanged(nameof(BusyMessage));
				}
			}
		}

		CrossTabEngine _engine;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public CrossTabEngine Engine
		{
			get => _engine;
			set
			{
				if (_engine != value)
				{
					_engine = value;
					OnPropertyChanged(nameof(Engine));
				}
			}
		}

		string _loginId;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string LoginId
		{
			get => _loginId;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_loginId != newval)
				{
					_loginId = newval;
					OnPropertyChanged(nameof(LoginId));
					Settings.Put(null, nameof(LoginId), _loginId);
				}
			}
		}

		string _loginPassword;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string LoginPassword
		{
			get => _loginPassword;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_loginPassword != newval)
				{
					_loginPassword = newval;
					OnPropertyChanged(nameof(LoginPassword));
					Settings.Put(null, nameof(LoginPassword), _loginPassword);
				}
			}
		}

		Licence _lic;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public Licence Lic
		{
			get => _lic;
			set
			{
				if (_lic != value)
				{
					_lic = value;
					OnPropertyChanged(nameof(Lic));
				}
			}
		}

		ObservableCollection<BindNode> _obsAccNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public ObservableCollection<BindNode> ObsAccNodes
		{
			get => _obsAccNodes;
			set
			{
				if (_obsAccNodes != value)
				{
					_obsAccNodes = value;
					OnPropertyChanged(nameof(ObsAccNodes));
				}
			}
		}

		BindNode _selectedNavNode;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public BindNode SelectedNavNode
		{
			get => _selectedNavNode;
			set
			{
				if (_selectedNavNode != value)
				{
					_selectedNavNode = value;
					OnPropertyChanged(nameof(SelectedNavNode));
					PostNavSelect();
				}
			}
		}

		AccountData _selectedAccount;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public AccountData SelectedAccount
		{
			get => _selectedAccount;
			set
			{
				if (_selectedAccount != value)
				{
					_selectedAccount = value;
					OnPropertyChanged(nameof(SelectedAccount));
				}
			}
		}

		ContainerData _selectedJob;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public ContainerData SelectedJob
		{
			get => _selectedJob;
			set
			{
				if (_selectedJob != value)
				{
					_selectedJob = value;
					OnPropertyChanged(nameof(SelectedJob));
				}
			}
		}

		BindNode _selectedVartreeNode;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public BindNode SelectedVartreeNode
		{
			get => _selectedVartreeNode;
			set
			{
				if (_selectedVartreeNode != value)
				{
					_selectedVartreeNode = value;
					OnPropertyChanged(nameof(SelectedVartreeNode));
					PostVartreeSelect();
				}
			}
		}

		string _selectedAxisTreeName;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string SelectedAxisTreeName
		{
			get => _selectedAxisTreeName;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_selectedAxisTreeName != newval)
				{
					_selectedAxisTreeName = newval;
					OnPropertyChanged(nameof(SelectedAxisTreeName));
				}
			}
		}

		string _selectedVartreeName;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string SelectedVartreeName
		{
			get => _selectedVartreeName;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_selectedVartreeName != newval)
				{
					_selectedVartreeName = newval;
					OnPropertyChanged(nameof(SelectedVartreeName));
				}
			}
		}

		ObservableCollection<BindNode> _obsVartreeNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public ObservableCollection<BindNode> ObsVartreeNodes
		{
			get => _obsVartreeNodes;
			set
			{
				if (_obsVartreeNodes != value)
				{
					_obsVartreeNodes = value;
					OnPropertyChanged(nameof(ObsVartreeNodes));
				}
			}
		}

		ContainerData[] _conJobs;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public ContainerData[] ConJobs
		{
			get => _conJobs;
			set
			{
				if (_conJobs != value)
				{
					_conJobs = value;
					OnPropertyChanged(nameof(ConJobs));
				}
			}
		}

		GenNode[] _tocNewRootGenNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public GenNode[] TocNewRootGenNodes
		{
			get => _tocNewRootGenNodes;
			set
			{
				if (_tocNewRootGenNodes != value)
				{
					_tocNewRootGenNodes = value;
					OnPropertyChanged(nameof(TocNewRootGenNodes));
				}
			}
		}

		GenNode[] _tocOldRootGenNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public GenNode[] TocOldRootGenNodes
		{
			get => _tocOldRootGenNodes;
			set
			{
				if (_tocOldRootGenNodes != value)
				{
					_tocOldRootGenNodes = value;
					OnPropertyChanged(nameof(TocOldRootGenNodes));
				}
			}
		}

		string[] _vartreeNames;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string[] VartreeNames
		{
			get => _vartreeNames;
			set
			{
				if (_vartreeNames != value)
				{
					_vartreeNames = value;
					OnPropertyChanged(nameof(VartreeNames));
				}
			}
		}

		string[] _axisTreeNames;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string[] AxisTreeNames
		{
			get => _axisTreeNames;
			set
			{
				if (_axisTreeNames != value)
				{
					_axisTreeNames = value;
					OnPropertyChanged(nameof(AxisTreeNames));
				}
			}
		}

		GenNode[] _jobIniRootNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public GenNode[] JobIniRootNodes
		{
			get => _jobIniRootNodes;
			set
			{
				if (_jobIniRootNodes != value)
				{
					_jobIniRootNodes = value;
					OnPropertyChanged(nameof(JobIniRootNodes));
				}
			}
		}

		int _selectedReportTabIndex;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public int SelectedReportTabIndex
		{
			get => _selectedReportTabIndex;
			set
			{
				if (_selectedReportTabIndex != value)
				{
					_selectedReportTabIndex = value;
					OnPropertyChanged(nameof(SelectedReportTabIndex));
				}
			}
		}

		string[] _genTabLines;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string[] GenTabLines
		{
			get => _genTabLines;
			set
			{
				if (_genTabLines != value)
				{
					_genTabLines = value;
					OnPropertyChanged(nameof(GenTabLines));
				}
			}
		}

		string _errorTitle;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string ErrorTitle
		{
			get => _errorTitle;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_errorTitle != newval)
				{
					_errorTitle = newval;
					OnPropertyChanged(nameof(ErrorTitle));
				}
			}
		}

		string _errorMessage;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_errorMessage != newval)
				{
					_errorMessage = newval;
					OnPropertyChanged(nameof(ErrorMessage));
				}
			}
		}

		XDisplayProperties _dProps;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public XDisplayProperties DProps
		{
			get => _dProps;
			set
			{
				if (_dProps != value)
				{
					_dProps = value;
					OnPropertyChanged(nameof(DProps));
				}
			}
		}

		Exception _loginError;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public Exception LoginError
		{
			get => _loginError;
			set
			{
				if (_loginError != value)
				{
					_loginError = value;
					OnPropertyChanged(nameof(LoginError));
				}
			}
		}

		Exception _openError;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public Exception OpenError
		{
			get => _openError;
			set
			{
				if (_openError != value)
				{
					_openError = value;
					OnPropertyChanged(nameof(OpenError));
				}
			}
		}

		Exception _vtError;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public Exception VtError
		{
			get => _vtError;
			set
			{
				if (_vtError != value)
				{
					_vtError = value;
					OnPropertyChanged(nameof(VtError));
				}
			}
		}

		Exception _vtsError;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public Exception VtsError
		{
			get => _vtsError;
			set
			{
				if (_vtsError != value)
				{
					_vtsError = value;
					OnPropertyChanged(nameof(VtsError));
				}
			}
		}

		int _reportFontSize = 12;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public int ReportFontSize
		{
			get => _reportFontSize;
			set
			{
				if (_reportFontSize != value)
				{
					_reportFontSize = value;
					OnPropertyChanged(nameof(ReportFontSize));
					Settings.Put(null, nameof(ReportFontSize), _reportFontSize);
				}
			}
		}

	}
}