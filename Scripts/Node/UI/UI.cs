using Godot;
using System;
using ScribbleLib.Input;

namespace Scribble;

public partial class UI : Node
{
    public static float MaxContentScale { get; } = 4;
    public static float MinContentScale { get; } = 0.25f;
    public static float ContentScaleIncrement { get; } = 0.25f;

    public override void _Ready()
    {
        Main.Ready += () => DebugInfo.Set("ui_scale", Main.Window.ContentScaleFactor);
        Keyboard.KeyDown += KeyDown;
    }

    void KeyDown(Key key)
    {
		//UI Scaling
        if (key == Key.Equal)
        {
            if (Main.Window.ContentScaleFactor + ContentScaleIncrement <= MaxContentScale)
                Main.Window.ContentScaleFactor += ContentScaleIncrement;
            DebugInfo.Set("ui_scale", Main.Window.ContentScaleFactor);
        }

        if (key == Key.Minus)
        {
            if (Main.Window.ContentScaleFactor - ContentScaleIncrement >= MinContentScale)
                Main.Window.ContentScaleFactor -= ContentScaleIncrement;
            DebugInfo.Set("ui_scale", Main.Window.ContentScaleFactor);
        }
    }
}
