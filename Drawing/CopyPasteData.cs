using System;
using Godot;
using Newtonsoft.Json;

namespace Scribble.Drawing;

public class CopyPasteData
{
	public Vector2I Position { get; set; }
	public string ImageData { get; set; }

	public CopyPasteData()
	{ }

	public CopyPasteData(Vector2I position, Image image)
	{
		Position = position;
		ImageData = Convert.ToBase64String(image.SavePngToBuffer());
	}

	public string ToJson() =>
		JsonConvert.SerializeObject(this);

	public static CopyPasteData FromJson(string json) =>
		JsonConvert.DeserializeObject<CopyPasteData>(json);
}
