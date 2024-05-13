using Godot;
using Scribble.Application;
using System.Collections.Generic;

namespace Scribble.UI;

public partial class Status : Node
{
	Node labelParent;
	Dictionary<string, InfoLabel> Labels { get; } = new()
	{
		{"pixel_pos", new("Pixel")},
		{"canvas_size", new("Size")},
		{"brush_size", new("Brush size")},
	};

	public override void _Ready()
	{
		Global.Status = this;

		labelParent = GetChild(0).GetChild(0);
		GenerateLabels();
	}

	void GenerateLabels()
	{
		foreach (string name in Labels.Keys)
		{
			Labels[name].Label = new()
			{
				LabelSettings = Global.LabelSettings
			};
			labelParent.AddChild(Labels[name].Label);
		}
	}

	public static void Set(string label, object value) => Global.Status.Labels[label].Set(value);
}
