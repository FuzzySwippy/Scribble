using Godot;
using System;
using ScribbleLib.Input;

namespace Scribble;

public partial class UI : Node
{
    public override void _Ready()
    {
        GetNode<DebugInfo>("/root/main/UI/Debug_Canvas/DebugInfo").Labels["scale"].Text = $"Scale: {GetWindow().ContentScaleFactor}";
        Keyboard.KeyDown += KeyDown;
    }

    void KeyDown(Key key)
    {
		//UI Scaling
        if (key == Key.Bracketright)
        {
            GetWindow().ContentScaleFactor += 0.25f;
            GetNode<DebugInfo>("/root/main/UI/Debug_Canvas/DebugInfo").Labels["scale"].Text = $"Scale: {GetWindow().ContentScaleFactor}";
        }

        if (key == Key.Bracketleft)
        {
            GetWindow().ContentScaleFactor -= 0.25f;
            GetNode<DebugInfo>("/root/main/UI/Debug_Canvas/DebugInfo").Labels["scale"].Text = $"Scale: {GetWindow().ContentScaleFactor}";
        }
    }
}
