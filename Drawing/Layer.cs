using Godot;
using Scribble.Drawing.Visualization;

namespace Scribble.Drawing;

public class Layer
{
	private readonly Canvas canvas;
	private readonly Color[,] colors;

	private float opacity;
	public float Opacity
	{
		get => opacity;
		set => opacity = Mathf.Clamp(value, 0, 1);
	}

	private bool visible;
	public bool Visible
	{
		get => visible;
		set => visible = value;
	}

	public Layer(Canvas canvas)
	{
		colors = new Color[canvas.Size.X, canvas.Size.Y];
		Opacity = 1;
		Visible = true;
	}

	/// <summary>
	/// Gets the color of the pixel at the given coordinates, taking into account the layer's opacity and visibility
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	public Color GetPixel(Vector2I pos) => colors[pos.X, pos.Y] * Opacity * (visible ? 1 : 0);

	/// <summary>
	/// Gets the true color of the pixel on this layer, ignoring opacity and visibility
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	public Color GetDataPixel(Vector2I pos) => colors[pos.X, pos.Y];

	/// <summary>
	/// Sets the color of the pixel at the given coordinates. To update the canvas, call <see cref="Canvas.UpdatePixel(int, int)"/>
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <param name="color">The color to set the pixel to</param>
	public void SetPixel(Vector2I pos, Color color)
	{
		colors[pos.X, pos.Y] = color;
		canvas.ImagePixelUpdated(pos);
	}
}