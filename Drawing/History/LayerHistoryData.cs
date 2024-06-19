using Godot;

namespace Scribble.Drawing;

public class LayerHistoryData
{
	public ulong LayerId { get; }
	public Color[,] Colors { get; }

	public LayerHistoryData(ulong layerId, Color[,] colors)
	{
		LayerId = layerId;
		Colors = colors;
	}
}
