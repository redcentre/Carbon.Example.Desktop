using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Carbon.Example.Desktop.Model;

internal static partial class AppUtility
{
	const int GWL_STYLE = -16;
	const int WS_MAXIMIZEBOX = 0x10000;
	const int WS_MINIMIZEBOX = 0x20000;

	[LibraryImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongPtrW")]
	internal static partial int GetWindowLong(IntPtr hwnd, int index);

	[LibraryImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtrW")]
	internal static partial int SetWindowLong(IntPtr hwnd, int index, int value);

	public static void HideMinimizeAndMaximizeButtons(Window window)
	{
		IntPtr hwnd = new WindowInteropHelper(window).Handle;
		var currentStyle = GetWindowLong(hwnd, GWL_STYLE);
		SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
	}

	public static T FindAncestorNodeByType<T>(AppNode node, AppNodeType type) where T : AppNode
	{
		AppNode findnode = node;
		while (findnode.Parent != null)
		{
			if (findnode.Type == type) return (T)findnode;
			findnode = findnode.Parent;
		}
		throw new Exception($"FindParentByType {node} -> {type} not found");
	}

	public static IEnumerable<AppNode> WalkNodes(IEnumerable<AppNode> nodes)
	{
		if (nodes != null)
		{
			foreach (var node in nodes)
			{
				yield return node;
				if (node.Children != null)
				{
					foreach (var child in WalkNodes(node.Children))
					{
						yield return child;
					}
				}
			}
		}
	}

	/// <summary>
	/// See: https://gfkeogh.blogspot.com/2016/07/windows-universal-gethashcode.html
	/// </summary>
	public static long StableHash64(string value)
	{
		ulong hash = 14695981039346656037UL;
		for (int i = 0; i < value.Length; i++)
		{
			hash ^= value[i];
			hash *= 1099511628211UL;
			hash = (2770643476691UL * hash) + 4354685564936844689UL;
		}
		return (long)hash;
	}

	public static string UserRelativeName(string path)
	{
		string[] segments = path.Split(Constants.PathSeparators);
		string[] usersegs = segments[3..^1];
		string name = Path.GetFileNameWithoutExtension(segments[^1]);
		return string.Join('/', usersegs.Concat([name]));
	}
}
