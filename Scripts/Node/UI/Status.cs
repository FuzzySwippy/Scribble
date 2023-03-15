using Godot;
using Scribble;
using System.Collections.Generic;

namespace Scribble;

public partial class Status : Node
{
    Node labelParent;
    public Dictionary<string, Label> Labels { get; } = new()
    {
        {"pixel_pos", null}
    };

    public override void _Ready()
    {
        labelParent = GetChild(0).GetChild(0);
        GenerateLabels();
    }

    void GenerateLabels()
    {
        foreach (string name in Labels.Keys)
        {
            Labels[name] = new()
            {
                LabelSettings = Global.LabelSettings
            };
            labelParent.AddChild(Labels[name]);
        }
    }
}
