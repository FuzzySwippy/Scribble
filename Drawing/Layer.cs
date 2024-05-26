using System;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Serialization;
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

	public Layer(Canvas canvas, BackgroundType backgroundType = BackgroundType.Transparent)
	{
		ID = GenerateID(canvas);
		Name = GetName(canvas);
		Size = canvas.Size;
		Colors = new Color[Size.X, Size.Y];
		if (backgroundType != BackgroundType.Transparent)
			FillBackground(backgroundType == BackgroundType.White ? new(1, 1, 1, 1) : new(0, 0, 0, 1));

		PreviewImage = Image.CreateFromData(Size.X, Size.Y, false, Image.Format.Rgba8, ColorsToByteArray(false));
		Preview = ImageTexture.CreateFromImage(PreviewImage);
	}

	/// <summary>
	/// Returns a duplicate layer of the given one with a unique ID
	/// </summary>
	public Layer(Canvas canvas, Layer layer)
	{
		ID = GenerateID(canvas);
		Name = layer.Name;
		Size = layer.Size;
		Colors = layer.Colors.Clone() as Color[,];
		Opacity = layer.Opacity;
		Visible = layer.Visible;

		PreviewImage = Image.CreateFromData(Size.X, Size.Y, false,
			Image.Format.Rgba8, ColorsToByteArray(false));
		Preview = ImageTexture.CreateFromImage(PreviewImage);
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
				Colors[x, y] = BlendColors(layer.GetPixel(x, y), Colors[x, y]);
	}

	public void UpdatePreview()
	{
		PreviewImage.SetData(Size.X, Size.Y, false,
			Image.Format.Rgba8, ColorsToByteArray(false));
		Preview.Update(PreviewImage);
	}

	public void SetPixel(Vector2I position, Color color) => Colors[position.X, position.Y] = color;
	public void SetPixel(int x, int y, Color color) => Colors[x, y] = color;
	public Color GetPixel(Vector2I position) => Colors[position.X, position.Y].MultiplyA(Opacity);
	public Color GetPixel(int x, int y) => Colors[x, y].MultiplyA(Opacity);
	public Color GetPixelNoOpacity(int x, int y) => Colors[x, y];

	public static Color BlendColors(Color topColor, Color bottomColor)
	{
		if (topColor.A == 1)
			return topColor;
		return bottomColor.Blend(topColor);
	}

	#region Serialization
	private byte[] ColorsToByteArray(bool noOpacity)
	{
		byte[] output = new byte[Colors.Length * 4];
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				int index = (y * Size.Y + x) * 4;
				Color color = noOpacity ? GetPixelNoOpacity(x, y) : GetPixel(x, y);

				output[index] = (byte)color.R8;
				output[index + 1] = (byte)color.G8;
				output[index + 2] = (byte)color.B8;
				output[index + 3] = (byte)color.A8;
			}
		}

		return output;
	}

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
				colors[x, y] = new Color(data[index] / 255f, data[index + 1] / 255f, data[index + 2] / 255f, data[index + 3] / 255f);
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
		serializer.Write(ColorsToByteArray(true), "colors");

		return serializer.Finalize();
	}
	#endregion
}