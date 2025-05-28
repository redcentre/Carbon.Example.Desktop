using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace RCS.Carbon.Example.Desktop.Model;

static class MainUtility
{
	public static T? FindVisualParent<T>(object source) where T : DependencyObject
	{
		var dep = (DependencyObject)source;
		while (dep != null && dep is not T)
		{
			dep = VisualTreeHelper.GetParent(dep);
		}
		return dep == null ? default : (T)dep;
	}

	public static IEnumerable<string> HexToLines(byte[] buffer)
	{
		const int Len = 32;
		if (buffer != null)
		{
			static string GetChars(byte[] buff, int offset, int len) => new([.. buff.Skip(offset).Take(Len).Select(b => b < 0x20 || b >= 0x7f ? ' ' : (char)b)]);
			int count = buffer.Length / Len;
			int rem = buffer.Length % Len;
			for (int i = 0; i < count; i++)
			{
				int off = i * Len;
				string hex = Convert.ToHexString(buffer, off, Len);
				string chars = GetChars(buffer, off, Len);
				string line = $"{off:X6} │{hex}│ │{chars}│";
				yield return line;
			}
			if (rem > 0)
			{
				int off = count * Len;
				string hex = Convert.ToHexString(buffer, off, rem);
				hex = hex.PadRight(2 * Len);
				string chars = GetChars(buffer, off, rem);
				chars = chars.PadRight(Len);
				string line = $"{off:X6} │{hex}│ │{chars}│";
				yield return line;
			}
		}
	}
}
