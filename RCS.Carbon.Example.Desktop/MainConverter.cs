using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using RCS.Carbon.Example.Desktop.Model;

namespace RCS.Carbon.Example.Desktop;

internal class MainConverter : IValueConverter
{
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string arg = (string)parameter;
		if (arg == "None")
		{
			return (value is string s && s.Length == 0) || value == null;
		}
		if (arg == "Some")
		{
			return (value is string s && s.Length > 0) || value != null;
		}
		if (arg == "NoneVisible")
		{
			return (value is string s && s.Length == 0) || value == null ? Visibility.Visible : Visibility.Collapsed;
		}
		if (arg == "SomeVisible")
		{
			return (value is string s && s.Length > 0) || value != null ? Visibility.Visible : Visibility.Collapsed;
		}
		if (arg == "TrueVisible")
		{
			return (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}
		if (arg == "FalseVisible")
		{
			return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
		}
		if (arg == "PositiveVisible")
		{
			return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		if (arg == "MainNodeLabel")
		{
			var node = (AppNode)value;
			return node.Label;
		}
		if (arg == "LogTime")
		{
			return ((DateTime)value).ToString("HH:mm:ss.fff");
		}
		if (arg == "MainNodeTip")
		{
			var node = (AppNode)value;
			return $"Id: {node.Id:X16} Parent Id: {node.Parent?.Id}\nType: {node.Type}\nLabel: {node.Label}\nKey: {node.Key}\nProps: {node.Props}";
		}
		if (arg == "LinesToBody")
		{
			return value is not string[] lines ? null : string.Join(Environment.NewLine, lines);
		}
		string[] tokens = arg.Split("|".ToCharArray());
		if (tokens[0] == "EnumBool")
		{
			return tokens[1] == value.ToString();
		}
		if (tokens[0] == "EnumVisible")
		{
			return tokens[1] == value.ToString() ? Visibility.Visible : Visibility.Collapsed;
		}
		throw new NotImplementedException($"MainConverter Convert {parameter}");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string arg = (string)parameter;
		string[] tokens = arg.Split("|".ToCharArray());
		if (tokens[0] == "EnumBool")
		{
			var enumval = Enum.Parse(targetType, tokens[1]);
			return (bool)value ? enumval : Binding.DoNothing;
		}
		throw new NotImplementedException($"MainConverter ConvertBack {parameter}");
	}
}
