using Godot;

namespace Scribble.Drawing;

public class Layer
{
	readonly Color[,] colors;

	float opacity;
	public float Opacity
	{
		get => opacity;
		set => opacity = Mathf.Clamp(value, 0, 1);
	}

	bool visible;
	public bool Visible
	{
		get => visible;
		set => visible = value;
	}

	public Layer(Vector2I size)
	{
		colors = new Color[size.X, size.Y];
		Opacity = 1;
		Visible = true;
	}

	/// <summary>
	/// Gets the color of the pixel at the given coordinates, taking into account the layer's opacity and visibility
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	public Color GetPixel(int x, int y) => colors[x, y] * Opacity * (visible ? 1 : 0);

	/// <summary>
	/// Gets the true color of the pixel on this layer, ignoring opacity and visibility
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	public Color GetColor(int x, int y) => colors[x, y];

	/// <summary>
	/// Sets the color of the pixel at the given coordinates
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <param name="color">The color to set the pixel to</param>
	public void SetColor(int x, int y, Color color) => colors[x, y] = color;
}