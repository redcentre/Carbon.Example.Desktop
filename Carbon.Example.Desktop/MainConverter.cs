using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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
			if (convarg == "VartreeNodeText")
			{
				var node = (BindNode)value;
				if (node == null) return null;
				if (node.Type == "Folder" || node.Type == "Variable") return node.Text;
				if (node.Type == "Dir" || node.Type == "File") return Path.GetFileNameWithoutExtension(node.Text);
				if (node.Description == null) return node.Text;
				return $"{node.Text} = {node.Description}";
			}
			var m = Regex.Match(convarg, @"Int(>=|==|<=|!=|<|>)(\d+)$", RegexOptions.IgnoreCase);
			if (m.Success)
			{
				int val = (int)value;
				string op = m.Groups[1].Value;
				int num = int.Parse(m.Groups[2].Value);
				switch (op)
				{
					case ">=": return val >= num;
					case "==": return val == num;
					case "<=": return val <= num;
					case "!=": return val != num;
					case "<": return val < num;
					case ">": return val > num;
				}
				return false;
			}
			if (convarg == "TreeSomeBack")
			{
				return value != null ? SystemColors.WindowBrush : Brushes.WhiteSmoke;
			}
			if (convarg == "LinesToBody")
			{
				var lines = value as string[];
				return lines == null ? null : string.Join(Environment.NewLine, lines);
			}
			if (convarg == "ReportTitle")
			{
				var title = (string)value;
				return title == null ? "Report (None)" : $"Report - {title}";
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
