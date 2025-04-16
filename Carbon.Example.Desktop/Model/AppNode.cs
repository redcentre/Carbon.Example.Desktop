using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Carbon.Example.Desktop.Model;

public enum AppNodeType
{
	Undefined,
	Customer,
	Job,
	RealVartree,
	Vartree,
	Axis,
	AxisChild,
	Folder,
	VartreeVariable,
	Codeframe,
	Code,
	Arith,
	Net,
	Nes,
	Stat,
	Table,
	Section,
	User,
	Licence,
	Error
}

public class AppNode(AppNodeType type, long id, string label, string? key = null) : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
	public AppNodeType Type { get; } = type;
	public long Id { get; } = id;
	public string Label { get; } = label;
	public string? Key { get; } = key;
	public virtual object? Props { get; }
	public override string ToString() => $"({Id},{Type},{Label},{_isExpanded},{_isSelected},{Children.Count},{Parent?.Id})";

	bool _isExpanded;
	public bool IsExpanded
	{
		get => _isExpanded;
		set
		{
			if (_isExpanded != value)
			{
				_isExpanded = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
			}
		}
	}

	bool _isSelected;
	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			if (_isSelected != value)
			{
				_isSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
			}
		}
	}

	public void AddChild(AppNode child)
	{
		if (_children == null)
		{
			_children = [];
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));
		}
		child.Parent = this;
		Children!.Add(child);
	}

	public AppNode? Parent { get; private set; }

	public bool IsLoaded { get; set; }

	static readonly ObservableCollection<AppNode> EmptyChildren = [];

	public ObservableCollection<AppNode>? _children;
	public ObservableCollection<AppNode> Children => _children ?? EmptyChildren;
}
