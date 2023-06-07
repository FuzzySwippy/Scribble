using Godot;

namespace Scribble.Drawing;

public class Layer
{
	readonly Canvas canvas;

	readonly Color[,] colors;
	float opacity;
	public bool Visible { get; set; }

	public Layer(Canvas canvas)
	{
		this.canvas = canvas;
		colors = new Color[canvas.size.X, canvas.size.Y];
		opacity = 1;
		Visible = true;
	}

	/// <summary>
	/// Gets the color of the pixel at the given coordinates, taking into account the layer's opacity and visibility
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	public Color GetPixel(int x, int y) => colors[x, y] * opacity * (Visible ? 1 : 0);

	/// <summary>
	/// Gets the true color of the pixel on this layer, ignoring opacity and visibility
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	public Color GetColor(int x, int y) => colors[x, y];

	/// <summary>
	/// Sets the true color of the pixel at the given coordinates. 
	/// The true color value can be retrieved with <see cref="GetColor(int, int)"/> 
	/// and the color that will be drawn can be retrieved with <see cref="GetPixel(int, int)"/>
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <param name="color">Color to set</param>
	public void SetColor(int x, int y, Color color)
	{
		colors[x, y] = color;
		canvas.SetDirty(x, y);
	}

	/// <summary>
	/// Sets the true color of the pixel at the given coordinates without setting the canvas pixel as dirty. After setting a batch of pixels, call <see cref="Canvas.SetAllDirty()"/> to mark the canvas for update.
	/// The true color value can be retrieved with <see cref="GetColor(int, int)"/> 
	/// and the color that will be drawn can be retrieved with <see cref="GetPixel(int, int)"/>
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <param name="color">Color to set</param>
	public void FastSetColor(int x, int y, Color color) => colors[x, y] = color;
}