using Godot;
using Scribble.Application;
using System.Collections.Generic;

namespace Scribble.UI;

public partial class Status : Node
{
	private Dictionary<string, InfoLabel> Labels { get; } = new()
	{
		//Shown at the end
		{"rotation_angle", new("Rotation angle", "°")},
		{"selection_pos", new("Selection position")},
		{"Selection_size", new("Selection size")},

		//Shown at the start
		{"pixel_pos", new("Pixel")},
		{"canvas_size", new("Resolution")},
		{"brush_size", new("Brush size")},
	};

	private bool LabelUpdated { get; set; }

	public override void _Ready()
	{
		Global.Status = this;
		GenerateLabels();
	}

	public override void _Process(double delta)
	{
		if (!LabelUpdated)
			return;

		foreach (string name in Labels.Keys)
			Labels[name].SetLabelValue();
		LabelUpdated = false;
	}

	private void GenerateLabels()
	{
		foreach (string name in Labels.Keys)
		{
			Labels[name].Label = new()
			{
				LabelSettings = Global.LabelSettings
			};
			AddChild(Labels[name].Label);
		}
	}

	public static void Set(string label, object value)
	{
		Global.Status.Labels[label].Set(value);
		Global.Status.LabelUpdated = true;
	}
}
