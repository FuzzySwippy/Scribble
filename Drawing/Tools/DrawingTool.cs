using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing.Tools;

public abstract class DrawingTool
{
	public Vector2I MousePixelPos => Global.Canvas.Drawing.MousePixelPos;

	public virtual void Update() { }
	public virtual void MouseDown(MouseCombination combination, Vector2 position) { }
}
