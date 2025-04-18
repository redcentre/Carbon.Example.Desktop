using System.IO;

namespace Carbon.Example.Desktop.Model;

public sealed class TocLeafNode(AppNodeType type, string value1, string value2) : AppNode(type, "TocLeaf", Path.GetFileNameWithoutExtension(value1), $"{value2}/{value1}")
{
}
