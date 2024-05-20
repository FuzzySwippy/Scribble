using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;
namespace Scribble.Drawing;

public class Layer
{
	private const string IndexedLayerNameStart = "New_Layer (";

	private Color[,] Colors { get; }
	private Vector2I Size { get; }

	public string Name { get; set; }
	public ulong ID { get; }

	/// <summary>
	/// Layer opacity value ranging from 0 to 1
	/// </summary>
	public float Opacity { get; set; } = 1;
	public bool Visible { get; set; } = true;

	private Image PreviewImage { get; set; }
	public ImageTexture Preview { get; set; }
	public bool PreviewNeedsUpdate { get; set; }

	public Layer(Canvas canvas)
	{
		ID = GenerateID(canvas);
		Name = GetName(canvas);
		Size = canvas.Size;
		Colors = new Color[Size.X, Size.Y];

		PreviewImage = Image.CreateFromData(Size.X, Size.Y, false, Image.Format.Rgba8, ColorsToByteArray());
		Preview = ImageTexture.CreateFromImage(PreviewImage);
	}

	/// <summary>
	/// Returns a duplicate layer of the given one with a unique ID
	/// </summary>
	public Layer(Canvas canvas, Layer layer)
	{
		ID = GenerateID(canvas);
		Name = layer.Name;
		Colors = layer.Colors.Clone() as Color[,];
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

	private string GetName(Canvas canvas)
	{
		string name = "New_Layer";
		if (!canvas.Layers.Any(l => l.Name == name))
			return name;

		ulong maxIndex = 0;
		foreach (Layer layer in canvas.Layers)
		{
			if (!layer.Name.StartsWith(IndexedLayerNameStart))
				continue;
			string indexString = layer.Name.Substring(IndexedLayerNameStart.Length, layer.Name.Length - IndexedLayerNameStart.Length - 1);
			if (ulong.TryParse(indexString, out ulong index) && index > maxIndex)
				maxIndex = index;
		}
		return $"{IndexedLayerNameStart}{maxIndex + 1})";
	}

	public void MergeUnder(Layer layer)
	{
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				Colors[x, y] = BlendColors(layer.Colors[x, y], Colors[x, y]);
	}

	private byte[] ColorsToByteArray()
	{
		byte[] output = new byte[Colors.Length * 4];
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				int index = (y * Size.Y + x) * 4;

				output[index] = (byte)Colors[x, y].R8;
				output[index + 1] = (byte)Colors[x, y].G8;
				output[index + 2] = (byte)Colors[x, y].B8;
				output[index + 3] = (byte)Colors[x, y].A8;
			}
		}

		return output;
	}

	public void UpdatePreview()
	{
		PreviewImage.SetData(Size.X, Size.Y, false, Image.Format.Rgba8, ColorsToByteArray());
		Preview.Update(PreviewImage);
	}

	public void SetPixel(Vector2I position, Color color) => Colors[position.X, position.Y] = color;
	public void SetPixel(int x, int y, Color color) => Colors[x, y] = color;
	public Color GetPixel(Vector2I position) => Colors[position.X, position.Y].MultiplyA(Opacity);
	public Color GetPixel(int x, int y) => Colors[x, y].MultiplyA(Opacity);

	public static Color BlendColors(Color topColor, Color bottomColor)
	{
		if (topColor.A == 1)
			return topColor;
		return bottomColor.Blend(topColor);
	}
}