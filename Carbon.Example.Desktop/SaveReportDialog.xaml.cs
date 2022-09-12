using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop
{
	public partial class SaveReportDialog : Window, INotifyPropertyChanged
	{
		public SaveReportDialog()
		{
			InitializeComponent();
			Loaded += SaveReportDialog_Loaded;
		}

		void SaveReportDialog_Loaded(object sender, RoutedEventArgs e)
		{
			BoxName.Focus();
		}

		void SaveReportOK_Click(object sender, RoutedEventArgs e)
		{
			SaveCommon();
		}

		string _reportName;
		public string ReportName
		{
			get => _reportName;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_reportName != newval)
				{
					_reportName = newval;
					SendChanged(nameof(ReportName));
					SendChanged(nameof(CanSave));
				}
			}
		}

		public bool CanSave
		{
			get
			{
				if (_reportName == null) return false;
				//if (!CommonUtil.IsValidSimpleFilename(_reportName)) return false;
				//if (_reportPath != null)
				//{
				//	string[] parts = _reportPath.Split('/', '\\');
				//	if (parts.Any(p => !CommonUtil.IsValidSimpleFilename(p))) return false;
				//}
				//return true;
				string[] parts = _reportName.Split('/', '\\');
				if (parts.Any(p => p.Length == 0)) return false;
				if (parts.Any(p => Path.GetInvalidFileNameChars().Intersect(p).Any())) return false;
				return true;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void SendChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		void Save_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				SaveCommon();
			}
		}

		void SaveCommon()
		{
			if (!CanSave) return;
			DialogResult = true;
		}
	}
}
