﻿//================================================================================================
// <auto-generated>
// This code was generated by a tool on machine HERON at local time 13/09/2022 06:38:29.
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//================================================================================================
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using RCS.Carbon.Shared;
using RCS.Carbon.Variables;

namespace Carbon.Example.Desktop
{
	partial class MainController
	{
		Exception _appError;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public Exception AppError
		{
			get => _appError;
			set
			{
				if (_appError != value)
				{
					_appError = value;
					OnPropertyChanged(nameof(AppError));
				}
			}
		}

		string _statusAccount;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string StatusAccount
		{
			get => _statusAccount;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_statusAccount != newval)
				{
					_statusAccount = newval;
					OnPropertyChanged(nameof(StatusAccount));
				}
			}
		}

		string _statusEngine;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string StatusEngine
		{
			get => _statusEngine;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_statusEngine != newval)
				{
					_statusEngine = newval;
					OnPropertyChanged(nameof(StatusEngine));
				}
			}
		}

		string _statusMessage;
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

		string _reportTop = "Age(*)";
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string ReportTop
		{
			get => _reportTop;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_reportTop != newval)
				{
					_reportTop = newval;
					OnPropertyChanged(nameof(ReportTop));
				}
			}
		}

		string _reportSide = "Region(*)";
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string ReportSide
		{
			get => _reportSide;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_reportSide != newval)
				{
					_reportSide = newval;
					OnPropertyChanged(nameof(ReportSide));
				}
			}
		}

		string _reportWeight;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string ReportWeight
		{
			get => _reportWeight;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_reportWeight != newval)
				{
					_reportWeight = newval;
					OnPropertyChanged(nameof(ReportWeight));
				}
			}
		}

		string _reportFilter;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string ReportFilter
		{
			get => _reportFilter;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_reportFilter != newval)
				{
					_reportFilter = newval;
					OnPropertyChanged(nameof(ReportFilter));
				}
			}
		}

		string _reportTitle;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public string ReportTitle
		{
			get => _reportTitle;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_reportTitle != newval)
				{
					_reportTitle = newval;
					OnPropertyChanged(nameof(ReportTitle));
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

		bool _useFilter;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public bool UseFilter
		{
			get => _useFilter;
			set
			{
				if (_useFilter != value)
				{
					_useFilter = value;
					OnPropertyChanged(nameof(UseFilter));
				}
			}
		}

		bool _useWeight;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public bool UseWeight
		{
			get => _useWeight;
			set
			{
				if (_useWeight != value)
				{
					_useWeight = value;
					OnPropertyChanged(nameof(UseWeight));
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

		CustData _selectedCust;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public CustData SelectedCust
		{
			get => _selectedCust;
			set
			{
				if (_selectedCust != value)
				{
					_selectedCust = value;
					OnPropertyChanged(nameof(SelectedCust));
				}
			}
		}

		JobData _selectedJob;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public JobData SelectedJob
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

		JobData[] _conJobs;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public JobData[] ConJobs
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
					_ = PostNavSelect();
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
					_ = PostVartreeSelect();
				}
			}
		}

		BindNode[] _topRootNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public BindNode[] TopRootNodes
		{
			get => _topRootNodes;
			set
			{
				if (_topRootNodes != value)
				{
					_topRootNodes = value;
					OnPropertyChanged(nameof(TopRootNodes));
				}
			}
		}

		BindNode[] _sideRootNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public BindNode[] SideRootNodes
		{
			get => _sideRootNodes;
			set
			{
				if (_sideRootNodes != value)
				{
					_sideRootNodes = value;
					OnPropertyChanged(nameof(SideRootNodes));
				}
			}
		}

		ObservableCollection<BindNode> _obsNavNodes;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public ObservableCollection<BindNode> ObsNavNodes
		{
			get => _obsNavNodes;
			set
			{
				if (_obsNavNodes != value)
				{
					_obsNavNodes = value;
					OnPropertyChanged(nameof(ObsNavNodes));
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

		XSpecProperties _sProps = new XSpecProperties();
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public XSpecProperties SProps
		{
			get => _sProps;
			set
			{
				if (_sProps != value)
				{
					_sProps = value;
					OnPropertyChanged(nameof(SProps));
				}
			}
		}

		XOutputFormat _selectedOutputFormat = XOutputFormat.CSV;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public XOutputFormat SelectedOutputFormat
		{
			get => _selectedOutputFormat;
			set
			{
				if (_selectedOutputFormat != value)
				{
					_selectedOutputFormat = value;
					OnPropertyChanged(nameof(SelectedOutputFormat));
					Settings.Put(null, nameof(SelectedOutputFormat), _selectedOutputFormat);
				}
			}
		}

		XSigType _selectedSigType = XSigType.SingleCell;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public XSigType SelectedSigType
		{
			get => _selectedSigType;
			set
			{
				if (_selectedSigType != value)
				{
					_selectedSigType = value;
					OnPropertyChanged(nameof(SelectedSigType));
				}
			}
		}

		VarMetaResult _vMeta;
		[GeneratedCode("TextTemplatingService", "17.0.0.0")]
		public VarMetaResult VMeta
		{
			get => _vMeta;
			set
			{
				if (_vMeta != value)
				{
					_vMeta = value;
					OnPropertyChanged(nameof(VMeta));
				}
			}
		}

	}
}