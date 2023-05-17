using System.Collections.Generic;
using Godot;

namespace ScribbleLib;

public enum ColorComponent
{
	R,
	G,
	B,
	A
}

public static class ScribbleColors
{
	public static readonly Color White = new(1, 1, 1);
	public static readonly Color Black = new(0, 0, 0);
	public static readonly Color Red = new(1, 0, 0);
	public static readonly Color Yellow = new(1, 1, 0);
	public static readonly Color Green = new(0, 1, 0);
	public static readonly Color Cyan = new(0, 1, 1);
	public static readonly Color Blue = new(0, 0, 1);
	public static readonly Color Magenta = new(0, 1, 1);

	public static readonly Color Transparent = new(0, 0, 0, 0);

	public static Dictionary<ColorComponent, Color> ComponentMap { get; } = new()
	{
		{ ColorComponent.R, Red },
		{ ColorComponent.G, Green },
		{ ColorComponent.B, Blue },
		{ ColorComponent.A, Black },
	};
}