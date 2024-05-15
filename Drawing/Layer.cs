using Godot;
using Scribble.Drawing;

namespace Scribble.Drawing;

public class Layer
{
	private readonly Canvas canvas;
	private readonly Color[,] colors;
	private readonly float opacity;
	private readonly bool visible;

	public Layer(Canvas canvas)
	{
		this.canvas = canvas;
		colors = new Color[canvas.Size.X, canvas.Size.Y];
		opacity = 1;
		visible = true;
	}

	public void SetPixel(Vector2I position, Color color) => colors[position.X, position.Y] = color;
	public void SetPixel(int x, int y, Color color) => colors[x, y] = color;
	public Color GetPixel(Vector2I position) => colors[position.X, position.Y];
	public Color GetPixel(int x, int y) => colors[x, y];
}