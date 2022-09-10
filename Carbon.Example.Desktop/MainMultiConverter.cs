using System;
using System.Globalization;
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
				if (type == "CLOUD") return Images.IconCloud;
				if (type == "CUST") return Images.IconAccount;
				if (type == "JOB") return Images.IconJob;
				if (type == "Folder" || type == "TOCNEW" || type == "TOCOLD" || type == "VTS" || type == "AXS" || type == "JOBINI") return exp == true ? Images.IconFolderOpen : Images.IconFolderClosed;
				if (type == "VT") return Images.IconVartree;
				if (type == "AX") return Images.IconAxisTree;
				if (type == "User") return Images.IconUser;
				if (type == "Table") return Images.IconTable;
				if (type == "Chart") return Images.IconChart;
				if (type == "File") return Images.IconFile;
				if (type == "Node") return Images.IconNode;
				if (type == "Data") return Images.IconData;
				if (type == "Dir") return exp == true ? Images.IconDirOpen : Images.IconDirClosed;
				return Images.IconVartree;
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
