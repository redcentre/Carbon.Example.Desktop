﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop
{
	/// <summary>
	/// A WPF Bindable hierarchical 'node' that wraps the general 'node' returned by some Carbon APIs.
	/// </summary>
	public sealed class BindNode : INotifyPropertyChanged
	{
		static int _id;

		public BindNode(string type, string text, int level, object data, BindNode parent)
		{
			Id = ++_id;
			Type = type;
			Text = text;
			Level = level;
			Data = data;
			Parent = parent;
		}

		public BindNode(GenNode node, object data, BindNode parent)
		{
			Id = node.Id;
			Type = node.Type;
			Text = node.Name;
			Description = node.Description;
			Data = data;
			Parent = parent;
		}

		public string Type { get; }
		public int Id { get; }
		public BindNode Parent { get; set; }

		public void AddChild(BindNode child)
		{
			if (Children == null)
			{
				Children = new ObservableCollection<BindNode>();
			}
			Children.Add(child);
		}

		public void AddChildRange(IEnumerable<BindNode> children)
		{
			if (Children == null)
			{
				Children = new ObservableCollection<BindNode>();
			}
			foreach (var child in children)
			{
				_children.Add(child);
			}
		}

		public override string ToString()
		{
			return $"BindNode({Id},{Type},{Text},{Description},{Level},{Data},{Parent?.Id})";
		}

		static ObservableCollection<BindNode> EmptyChildren = new ObservableCollection<BindNode>();

		ObservableCollection<BindNode> _children = new ObservableCollection<BindNode>();
		public ObservableCollection<BindNode> Children
		{
			get => _children ?? EmptyChildren;
			set
			{
				if (_children != value)
				{
					_children = value;
					OnPropertyChanged(nameof(Children));
				}
			}
		}

		public bool AnyChildren => _children?.Count > 0;

		int _level;
		public int Level
		{
			get => _level;
			set
			{
				if (_level != value)
				{
					_level = value;
					OnPropertyChanged(nameof(Level));
				}
			}
		}

		string _text;
		public string Text
		{
			get => _text;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_text != newval)
				{
					_text = newval;
					OnPropertyChanged(nameof(Text));
				}
			}
		}

		string _description;
		public string Description
		{
			get => _description;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_description != newval)
				{
					_description = newval;
					OnPropertyChanged(nameof(Description));
				}
			}
		}

		object _data;
		public object Data
		{
			get => _data;
			set
			{
				if (_data != value)
				{
					_data = value;
					OnPropertyChanged(nameof(Data));
				}
			}
		}

		bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (_isExpanded != value)
				{
					_isExpanded = value;
					OnPropertyChanged(nameof(IsExpanded));
				}
			}
		}

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}