using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Carbon.Example.Desktop
{
	internal class MainConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string convarg = (string)parameter;
			if (convarg == "None")
			{
				return value == null;
			}
			if (convarg == "Not")
			{
				return !(bool)value;
			}
			if (convarg == "Some")
			{
				return value != null;
			}
			if (convarg == "SomeVisible")
			{
				return value == null ? Visibility.Hidden : Visibility.Visible;
			}
			if (convarg == "TrueVisible")
			{
				return (bool?)value == true ? Visibility.Visible : Visibility.Hidden;
			}
			if (convarg == "NavNodeText")
			{
				var node = (BindNode)value;
				if (node == null) return null;
				if (node.Type == "Dir") return node.Text.Split('/', '\\').Where(x => x.Length > 0).Last();
				if (node.Type == "File") return Path.GetFileNameWithoutExtension(node.Text);
				if (node.Type == "Data") return $"{node.Text}={node.Description}";
				return node.Text;
			}
			throw new NotImplementedException($"MainConverter Convert {parameter}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string convarg = (string)parameter;
			throw new NotImplementedException($"MainConverter ConvertBack {parameter}");
		}
	}
}
