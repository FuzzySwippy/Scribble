using Godot;
using System;

public partial class Main : Node2D
{
	public override void _Ready()
	{
        GetWindow().MinSize = new Vector2I(800, 500);
	}
}
