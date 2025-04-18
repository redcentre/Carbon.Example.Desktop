namespace Carbon.Example.Desktop.Model.Extensions;

public static class StringExtensions
{
	public static string Format(this string message, params object[] args)
	{
		return string.Format(message, args).Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
	}
}
