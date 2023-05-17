using Godot;

namespace Scribble;

public class Palette
{
	public const int MaxColors = 16;


	public string Name { get; set; }
	public bool Locked { get; set; }
	public Color?[] Colors { get; } = new Color?[MaxColors];

	public Palette(string name, Color?[] colors)
	{
		Name = name;

		if (colors == null)
			return;

		if (colors.Length > MaxColors)
			throw new System.ArgumentOutOfRangeException(nameof(colors), $"Palette can only have {MaxColors} colors.");

		colors?.CopyTo(Colors, 0);
	}

	public Palette(string name) : this(name, null) { }

	/// <summary>
	/// Gets the color at the specified index
	/// </summary>
	/// <param name="index">Index from which the color should be retrieved</param>
	/// <returns>The color at a given index or <see langword="null"/> if it doesn't have a value</returns>
	public Color? GetColor(int index)
	{
		if (index < 0 || index >= Colors.Length)
			throw new System.ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Colors.Length - 1}.");

		return Colors[index];
	}

	/// <summary>
	/// Sets the color at the specified index. Returns true if the color was different from the previous color.
	/// </summary>
	/// <param name="color">Color to be set</param>
	/// <param name="index">Index at which the specified color should be set at</param>
	/// <returns><see langword="true"/> if the color was modified and <see langword="false"/> if the color hasn't changed</returns>
	public bool SetColor(Color? color, int index)
	{
		if (index < 0 || index >= Colors.Length)
			throw new System.ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Colors.Length - 1}.");

		if (Colors[index] == color)
			return false;

		Colors[index] = color;
		return true;
	}
}
