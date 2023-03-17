using Godot;
using Scribble;
using System.Collections.Generic;

namespace Scribble;

public partial class Status : Node
{
    static Status current;

    Node labelParent;
    Dictionary<string, InfoLabel> Labels { get; } = new()
    {
        {"pixel_pos", new("Pixel")},
        {"canvas_size", new("Size")}
    };

    public override void _Ready()
    {
        current = this;

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

    public static void Set(string label, object value) => current.Labels[label].Set(value);
}
