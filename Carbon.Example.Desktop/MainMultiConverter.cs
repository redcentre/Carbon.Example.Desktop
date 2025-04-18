using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Carbon.Example.Desktop.Model;
using RCS.Carbon.Licensing.Shared;
using RCS.Carbon.Tables;

namespace Carbon.Example.Desktop;

internal class MainMultiConverter : IMultiValueConverter
{
	public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string arg = (string)parameter;
		if (arg == "StatusMessage")
		{
			var list = new List<string>();
			if (values[0] is ILicensingProvider prov) list.Add($"{prov.Name} {prov.GetType().Assembly.GetName().Version}");
			if (values[1] is CrossTabEngine engine) list.Add($"Carbon {engine.Version}");
			if (values[2] is CustomerNode custnode) list.Add(custnode.Customer.Name);
			if (values[3] is JobNode jobnode) list.Add(jobnode.Job.Name);
			if (values[4] is string vtname) list.Add(vtname);
			if (list.Count == 0) return "Not authenticated";
			return string.Join(" \u25fe ", list);
		}
		if (arg == "AllNotNull")
		{
			return values.All(v => v != null);
		}
		if (arg == "ReportTitle")
		{
			var isnew = values[0] as bool?;
			if (isnew == true) return "New report";
			if (values[1] is not TocLeafNode node) return "No report selected";
			return node.Label;
		}
		if (arg == "MainNodeIcon")
		{
			AppNodeType[] FolderTypes = [AppNodeType.Folder, AppNodeType.FullToc, AppNodeType.ExecToc, AppNodeType.SimpleToc];
			var type = (AppNodeType)values[0];
			var expand = (bool)values[1];
			if (FolderTypes.Contains(type)) return expand ? Images.NodeFolderOpen : Images.NodeFolderClosed;
			if (type == AppNodeType.Customer) return Images.NodeCustomer;
			if (type == AppNodeType.Job) return Images.NodeJob;
			if (type == AppNodeType.RealVartree) return Images.NodeRealVartree;
			if (type == AppNodeType.Vartree) return Images.NodeVartree;
			if (type == AppNodeType.Codeframe) return Images.NodeCodeframe;
			if (type == AppNodeType.VartreeVariable) return Images.NodeVartreeVariable;
			if (type == AppNodeType.Code) return Images.NodeCode;
			if (type == AppNodeType.Axis || type == AppNodeType.AxisChild) return Images.NodeAxis;
			if (type == AppNodeType.Arith) return Images.NodeArith;
			if (type == AppNodeType.Error) return Images.NodeError;
			if (type == AppNodeType.Table) return Images.NodeTable;
			if (type == AppNodeType.Net) return Images.NodeNet;
			if (type == AppNodeType.Nes) return Images.NodeNes;
			if (type == AppNodeType.Stat) return Images.NodeStat;
			if (type == AppNodeType.Section) return Images.NodeSection;
			if (type == AppNodeType.User) return Images.NodeUser;
			if (type == AppNodeType.Licence) return Images.NodeLicence;
			return Images.NodeUnknown;
		}
		string[] tokens = arg.Split('|');
		if (tokens[0] == "NodeTypeVisible")
		{
			var node = values[0] as AppNode;
			var type = values[1] as AppNodeType?;
			string[] list = tokens[1].Split("+;,".ToCharArray());
			return list.Contains(type?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
		}
		throw new NotImplementedException($"MainMultiConverter.Convert {parameter}");
	}

	public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException($"MainMultiConverter.ConvertBack {parameter}");
	}
}
