using Godot;

namespace Scribble.Drawing;

public partial class HistoryListItemData : GodotObject
{
	public string Name { get; }
	public Texture2D Icon { get; }

	public HistoryListItemData(string name, Texture2D icon)
	{
		Name = name;
		Icon = icon;
	}
}
