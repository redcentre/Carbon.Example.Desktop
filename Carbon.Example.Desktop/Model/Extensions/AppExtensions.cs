namespace Carbon.Example.Desktop.Model.Extensions;

/// <summary>
/// The compiler nags if extension methods aren't put in a separate namespace and matching folder.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// This is for convenience when formatting resource strings.
	/// </summary>
	public static string Format(this string message, params object[] args)
	{
		return string.Format(message, args).Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
	}
}
