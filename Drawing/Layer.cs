using System;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Serialization;
using Scribble.UI;
namespace Scribble.Drawing;

public class Layer
{
	private const string IndexedLayerNameStart = "New_Layer (";

	public Color[,] Colors { get; set; }
	private Vector2I Size { get; set; }

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

	public Layer(Canvas canvas, BackgroundType backgroundType = BackgroundType.Transparent)
	{
		ID = GetID();
		Name = GetName(canvas);
		Size = canvas.Size;
		Colors = new Color[Size.X, Size.Y];
		if (backgroundType != BackgroundType.Transparent)
			FillBackground(backgroundType == BackgroundType.White ? new(1, 1, 1, 1) : new(0, 0, 0, 1));

		CreatePreview(Colors.ToByteArray(Opacity));
	}

	public Layer(Canvas canvas, Color[,] colors)
	{
		ID = GetID();
		Name = GetName(canvas);
		Size = canvas.Size;
		Colors = colors;

		CreatePreview(Colors.ToByteArray(Opacity));
	}

	/// <summary>
	/// Returns a duplicate layer of the given one with a unique ID
	/// </summary>
	public Layer(Layer layer)
	{
		ID = GetID();
		Name = layer.Name;
		Size = layer.Size;
		Colors = layer.Colors.Clone() as Color[,];
		Opacity = layer.Opacity;
		Visible = layer.Visible;

		CreatePreview(Colors.ToByteArray(Opacity));
	}

	public Layer(byte[] data)
	{
		Deserializer deserializer = new(data);

		ID = (ulong)deserializer.DeserializedObjects["id"].Value;
		Name = (string)deserializer.DeserializedObjects["name"].Value;
		Opacity = (float)deserializer.DeserializedObjects["opacity"].Value;
		Visible = (bool)deserializer.DeserializedObjects["visible"].Value;
		Size = (Vector2I)deserializer.DeserializedObjects["size"].Value;

		byte[] colorData = (byte[])deserializer.DeserializedObjects["colors"].Value;
		Colors = ByteArrayToColors(colorData);

		CreatePreview(colorData);
	}

	private void CreatePreview(byte[] colorData)
	{
		PreviewImage?.Dispose();
		Preview?.Dispose();

		PreviewImage = Image.CreateFromData(Size.X, Size.Y, false,
			Image.Format.Rgba8, colorData);
		Preview = ImageTexture.CreateFromImage(PreviewImage);
	}

	private void FillBackground(Color color)
	{
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				Colors[x, y] = color;
	}

	private ulong GetID()
	{
		ulong id = (ulong)Global.Random.NextInt64();
		while (Global.Canvas.Layers.Any(l => l.ID == id))
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
				Colors[x, y] = BlendColors(layer.GetPixel(x, y), Colors[x, y]);
	}

	public void UpdatePreview()
	{
		PreviewImage.SetData(Size.X, Size.Y, false,
			Image.Format.Rgba8, Colors.ToByteArray(Opacity));
		Preview.Update(PreviewImage);
	}

	public void SetPixel(Vector2I position, Color color) => Colors[position.X, position.Y] = color;
	public void SetPixel(int x, int y, Color color) => Colors[x, y] = color;
	public Color GetPixel(Vector2I position) => Colors[position.X, position.Y].MultiplyA(Opacity);
	public Color GetPixel(int x, int y) => Colors[x, y].MultiplyA(Opacity);
	public Color GetPixelNoOpacity(Vector2I position) => Colors[position.X, position.Y];
	public Color GetPixelNoOpacity(int x, int y) => Colors[x, y];

	public static Color BlendColors(Color topColor, Color bottomColor)
	{
		if (topColor.A == 1)
			return topColor;
		return bottomColor.Blend(topColor);
	}

	#region ImageOperations
	public void FlipVertically()
	{
		Color[,] newColors = new Color[Size.X, Size.Y];
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				newColors[x, y] = Colors[Size.X - x - 1, y];
		Colors = newColors;
		UpdatePreview();
	}

	public void FlipHorizontally()
	{
		Color[,] newColors = new Color[Size.X, Size.Y];
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				newColors[x, y] = Colors[x, Size.Y - y - 1];
		Colors = newColors;
		UpdatePreview();
	}

	public void RotateClockwise()
	{
		Color[,] newColors = new Color[Size.Y, Size.X];
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				newColors[Size.Y - y - 1, x] = Colors[x, y];
		Colors = newColors;
		Size = new(Size.Y, Size.X);

		CreatePreview(Colors.ToByteArray(Opacity));
	}

	public void RotateCounterClockwise()
	{
		Color[,] newColors = new Color[Size.Y, Size.X];
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				newColors[y, Size.X - x - 1] = Colors[x, y];
		Colors = newColors;
		Size = new(Size.Y, Size.X);

		CreatePreview(Colors.ToByteArray(Opacity));
	}

	public Color[,] Resize(Vector2I newSize, ResizeType type)
	{
		Vector2I oldSize = Size;
		Size = newSize;
		Color[,] oldColors = Colors;
		Colors = new Color[Size.X, Size.Y];

		switch (type)
		{
			case ResizeType.Scale:
				for (int x = 0; x < Size.X; x++)
				{
					for (int y = 0; y < Size.Y; y++)
					{
						float oldX = x / (float)Size.X * oldSize.X;
						float oldY = y / (float)Size.Y * oldSize.Y;
						Colors[x, y] = oldColors[(int)oldX, (int)oldY];
					}
				}
				break;
			case ResizeType.Crop:
				for (int x = 0; x < Size.X; x++)
					for (int y = 0; y < Size.Y; y++)
						Colors[x, y] = x < oldSize.X && y < oldSize.Y ?
							oldColors[x, y] : new Color(0, 0, 0, 0);
				break;
		}

		CreatePreview(Colors.ToByteArray(Opacity));

		return oldColors;
	}

	public void ResizeWithColorData(Vector2I newSize, Color[,] colors)
	{
		Size = newSize;
		Colors = colors.Clone() as Color[,];

		CreatePreview(Colors.ToByteArray(Opacity));
	}

	public Color[,] CropToBounds(Rect2I bounds)
	{
		Size = bounds.Size;
		Color[,] oldColors = Colors;
		Colors = new Color[Size.X, Size.Y];

		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				Colors[x, y] = oldColors[x + bounds.Position.X, y + bounds.Position.Y];

		CreatePreview(Colors.ToByteArray(Opacity));

		return oldColors;
	}
	#endregion

	#region Serialization
	private Color[,] ByteArrayToColors(byte[] data)
	{
		if (data.Length != Size.X * Size.Y * 4)
			throw new Exception("Invalid data length");

		Color[,] colors = new Color[Size.X, Size.Y];
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				int index = (y * Size.Y + x) * 4;
				colors[x, y] = new Color(
					data[index] / 255f, data[index + 1] / 255f, data[index + 2] / 255f, data[index + 3] / 255f);
			}
		}

		return colors;
	}

	public byte[] Serialize()
	{
		Serializer serializer = new();

		serializer.Write(ID, "id");
		serializer.Write(Name, "name");
		serializer.Write(Opacity, "opacity");
		serializer.Write(Visible, "visible");
		serializer.Write(Size, "size");
		serializer.Write(Colors.ToByteArray(), "colors");

		return serializer.Finalize();
	}
	#endregion
}