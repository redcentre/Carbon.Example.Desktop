using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Carbon.Example.Desktop
{
	internal class MainMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			string p = (string)parameter;
			if (p == "NavIcon")
			{
				var type = values[0] as string;
				var exp = values[1] as bool?;
				if (type == BindNode.TypeCloud) return Images.IconCloud;
				if (type == BindNode.TypeCust) return Images.IconCust;
				if (type == BindNode.TypeJob) return Images.IconJob;
				if (type == BindNode.TypeFolder || type == BindNode.TypeTocNew || type == BindNode.TypeTocOld || type == BindNode.TypeVartees || type == BindNode.TypeAxTrees || type == BindNode.TypeIni) return exp == true ? Images.IconFolderOpen : Images.IconFolderClosed;
				if (type == BindNode.TypeVt) return Images.IconVartree;
				if (type == BindNode.TypeAx) return Images.IconAxisTree;
				if (type == "Folder") return exp == true ? Images.IconFolderOpen : Images.IconFolderClosed;
				if (type == "User") return Images.IconUser;
				if (type == "Table") return Images.IconTable;
				if (type == "Chart") return Images.IconChart;
				if (type == "File") return Images.IconFile;
				if (type == "Node") return Images.IconNode;
				if (type == "Data") return Images.IconData;
				if (type == "Dir") return exp == true ? Images.IconDirOpen : Images.IconDirClosed;
				return Images.IconVartree;
			}
			if (p == "VartreeIcon")
			{
				var type = values[0] as string;
				var exp = values[1] as bool?;
				if (type == "Folder") return exp == true ? Images.IconFolderOpen : Images.IconFolderClosed;
				if (type == "Variable") return Images.IconVariable;
				if (type == "Axis") return Images.IconAxisTree;
				return Images.IconCode;
			}
			if (p == "JobHelpTextVisible")
			{
				// The job help text is only visible if the navigation
				// tree is loaded but no job has been opened yet.
				return values[0] != null && values[1] == null ? Visibility.Visible : Visibility.Collapsed;
			}
			if (p == "VartreeTitle")
			{
				var vtname = values[0] as string;
				var axname = values[1] as string;
				if (vtname != null) return $"Vartree \u2192 {vtname}";
				if (axname != null) return $"Axis \u2192 {axname}";
				return "Variables";
			}
			if (p == "AnySome")
			{
				return values.Any(v => v != null);
			}
			if (p == "BindNodeTip")
			{
				var node = values[0] as BindNode;
				var exp = values[1] as bool?;
				var sel = values[2] as bool?;
				var ccount = values[3] as int?;
				if (node == null) return null;
				return string.Format("{0} • {1} • {2}\nLevel {3} • {4} • {5}\nChildren: {6}",
					node.Id, node.Type, node.Text,
					node.Level,
					exp == true ? "Expanded" : "Collapsed",
					sel == true ? "Selected" : "Unselected",
					ccount == null ? "NULL" : ccount.ToString()
				);
			}
			throw new NotImplementedException($"MainMultiConverter Convert {parameter}");
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			string p = (string)parameter;
			throw new NotImplementedException($"MainMultiConverter ConvertBack {parameter}");
		}
	}
}
