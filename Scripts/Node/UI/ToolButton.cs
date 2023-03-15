using Godot;
using System;

namespace Scribble;

public partial class ToolButton : Button
{
    public override void _Pressed()
    {
        GD.Print(Name);
    }
}
