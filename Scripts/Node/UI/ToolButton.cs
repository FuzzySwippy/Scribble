using Godot;
using System;

namespace Scribble;

public partial class ToolButton : Button
{
    public override void _Toggled(bool buttonPressed)
    {
        if (!buttonPressed)
            return;

        GD.Print(Name);
    }
}
