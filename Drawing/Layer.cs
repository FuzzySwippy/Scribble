using System.Linq;
using Godot;
using Scribble.Application;
namespace Scribble.Drawing;

public class Layer
{
	private readonly Color[,] colors;

	public string Name { get; set; } = "New_Layer";
	public ulong ID { get; }
	public float Opacity { get; set; } = 1;
	public bool Visible { get; set; } = true;

	public Layer(Canvas canvas)
	{
		ID = GenerateID(canvas);
		colors = new Color[canvas.Size.X, canvas.Size.Y];
	}

	/// <summary>
	/// Returns a duplicate layer of the given one with a unique ID
	/// </summary>
	public Layer(Canvas canvas, Layer layer)
	{
		ID = GenerateID(canvas);
		Name = layer.Name;
		colors = layer.colors.Clone() as Color[,];
		Opacity = layer.Opacity;
		Visible = layer.Visible;
	}

	private ulong GenerateID(Canvas canvas)
	{
		ulong id = (ulong)Global.Random.NextInt64();
		while (canvas.Layers.Any(l => l.ID == id))
			id = (ulong)Global.Random.NextInt64();
		return id;
	}

	public void MergeUnder(Layer layer)
	{
		for (int x = 0; x < colors.GetLength(0); x++)
			for (int y = 0; y < colors.GetLength(1); y++)
				colors[x, y] = BlendColors(layer.colors[x, y], colors[x, y]);
	}

	public void SetPixel(Vector2I position, Color color) => colors[position.X, position.Y] = color;
	public void SetPixel(int x, int y, Color color) => colors[x, y] = color;
	public Color GetPixel(Vector2I position) => colors[position.X, position.Y];
	public Color GetPixel(int x, int y) => colors[x, y];

	public static Color BlendColors(Color topColor, Color bottomColor)
	{
		Color output = new();
		if (topColor.A == 1)
			output = topColor;
		else
			output = bottomColor.Blend(topColor);
		return output;
	}
}