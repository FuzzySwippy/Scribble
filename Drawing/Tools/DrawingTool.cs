using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing.Tools;

public abstract class DrawingTool
{
	protected Artist Artist => Global.Canvas.Drawing.Artist;
	protected Vector2I MousePixelPos => Global.Canvas.Drawing.MousePixelPos;
	protected Vector2I OldMousePixelPos => Global.Canvas.Drawing.OldMousePixelPos;
	public Dictionary<MouseCombination, QuickPencilType> MouseColorInputMap => Global.Canvas.Drawing.MouseColorInputMap;

	public virtual void Update() { }
	public virtual void MouseDown(MouseCombination combination, Vector2 position) { }
}
