using System.IO;

namespace Carbon.Example.Desktop.Model;

public sealed class TocLeafNode(AppNodeType type, long id, string value1, string value2) : AppNode(type, id, Path.GetFileNameWithoutExtension(value1), $"{value2}/{value1}")
{
}
