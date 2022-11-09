using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop.Model
{
	static class MainUtility
	{
		public static T FindVisualParent<T>(object source) where T : DependencyObject
		{
			var dep = (DependencyObject)source;
			while (dep != null && !(dep is T))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}
			return dep == null ? default : (T)dep;
		}

		public static IEnumerable<string> TextToLines(string value)
		{
			if (value != null)
			{
				using (var reader = new StringReader(value))
				{
					string line = reader.ReadLine();
					while (line != null)
					{
						yield return line;
						line = reader.ReadLine();
					}
				}
			}
		}

		public static IEnumerable<string> HexToLines(byte[] buffer)
		{
			const int Len = 64;
			if (buffer != null)
			{
				int count = buffer.Length / Len;
				int rem = buffer.Length % Len;
				for (int i = 0; i < count; i++)
				{
					int off = i * Len;
					string hex = BitConverter.ToString(buffer, off, 64).Replace("-", "");
					string line = $"{off:X6} {hex}";
					yield return line;
				}
				if (rem > 0)
				{
					int off = count * Len;
					string hex = BitConverter.ToString(buffer, off, rem).Replace("-", "");
					string line = $"{off:X6} {hex}";
					yield return line;
				}
			}
		}

		public static void PrintNodes(IEnumerable<GenNode> nodes, string title)
		{
			Trace.WriteLine($"---------- {title} ----------");
			if (nodes != null)
			{
				foreach (var node in GenNode.WalkNodes(nodes))
				{
					string pfx = string.Join("", Enumerable.Repeat("|  ", node.Level));
					Trace.WriteLine($"{pfx}{node}");
				}
			}
		}
	}
}
