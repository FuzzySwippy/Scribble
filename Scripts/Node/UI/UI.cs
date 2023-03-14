using Godot;
using System;
using ScribbleLib.Input;

namespace Scribble;

public partial class UI : Node
{
    public override void _Ready()
    {
        GD.Print($"EEE: {Global.DebugInfo == null}");
        Global.DebugInfo.Labels["scale"].Text = $"Scale: {GetWindow().ContentScaleFactor}";
        Keyboard.KeyDown += KeyDown;
    }

    void KeyDown(Key key)
    {
		//UI Scaling
        if (key == Key.Bracketright)
        {
            GetWindow().ContentScaleFactor += 0.25f;
            Global.DebugInfo.Labels["scale"].Text = $"Scale: {GetWindow().ContentScaleFactor}";
        }

        if (key == Key.Bracketleft)
        {
            GetWindow().ContentScaleFactor -= 0.25f;
            Global.DebugInfo.Labels["scale"].Text = $"Scale: {GetWindow().ContentScaleFactor}";
        }
    }
}
