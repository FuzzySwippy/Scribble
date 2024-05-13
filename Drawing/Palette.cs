using System;
using Godot;
using Newtonsoft.Json;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Interfaces;

namespace Scribble.Drawing;

public class Palette : UniqueObject<Palette>, IDuplicatable<Palette>
{
	public const int MaxColors = 16;

	public string Name { get; set; }
	public bool Locked { get; set; }

	[JsonProperty]
	readonly SimpleColor[] colors = new SimpleColor[MaxColors];

	public SimpleColor this[int index]
	{
		get => GetColor(index);
		set => SetColor(value, index);
	}

	public Palette() { }

	public Palette(string name, Color?[] colorArray)
	{
		Name = name;

		if (colorArray == null)
			return;

		if (colorArray.Length > MaxColors)
			throw new ArgumentOutOfRangeException(nameof(colorArray), $"Palette can only have {MaxColors} colors.");

		for (int i = 0; i < colorArray.Length; i++)
			if (colorArray[i] != null)
				colors[i] = new(colorArray[i].Value);
	}

	public Palette(string name) : this(name, null) { }

	/// <summary>
	/// Gets the color at the specified index
	/// </summary>
	/// <param name="index">Index from which the color should be retrieved</param>
	/// <returns>The color at a given index or <see langword="null"/> if it doesn't have a value</returns>
	public SimpleColor GetColor(int index)
	{
		if (index < 0 || index >= colors.Length)
			throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {colors.Length - 1}.");

		return colors[index];
	}

	/// <summary>
	/// Sets the color at the specified index. Returns true if the color was different from the previous color.
	/// </summary>
	/// <param name="color">Color to be set</param>
	/// <param name="index">Index at which the specified color should be set at</param>
	/// <returns><see langword="true"/> if the color was modified and <see langword="false"/> if the color hasn't changed</returns>
	public bool SetColor(SimpleColor color, int index)
	{
		if (index < 0 || index >= colors.Length)
			throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {colors.Length - 1}.");

		if (colors[index] == color)
			return false;

		colors[index] = color;
		return true;
	}

	public Palette Duplicate()
	{
		Palette palette = new(Name) { Locked = Locked };

		for (int i = 0; i < colors.Length; i++)
			palette.SetColor(colors[i], i);

		return palette;
	}
}
