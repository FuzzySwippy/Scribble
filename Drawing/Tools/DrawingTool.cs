using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing.Tools;

public abstract class DrawingTool
{
	protected Canvas Canvas => Global.Canvas;
	protected Selection Selection => Global.Canvas.Selection;
	protected Artist Artist => Global.Canvas.Drawing.Artist;
	protected Vector2I MousePixelPos => Global.Canvas?.Drawing?.MousePixelPos ?? Vector2I.Zero;
	protected Vector2I OldMousePixelPos => Global.Canvas.Drawing.OldMousePixelPos;

	public bool ResetOnSelection { get; set; } = true;
	public bool SelectionTool { get; set; } = false;

	//Input
	public Dictionary<MouseCombination, QuickPencilType> MouseColorInputMap =>
		Global.Canvas.Drawing.MouseColorInputMap;
	public Key[] CancelKeys => Global.Canvas.Drawing.CancelKeys;

	public virtual void Selected() { }
	public virtual void Deselected() { }
	public virtual void SizeChanged(int size) { }
	public virtual void Reset() { }
	public virtual void Update() { }
	public virtual void MouseMoveUpdate() { }
	public virtual void MouseDown(MouseCombination combination, Vector2 position) { }
	public virtual void MouseUp(MouseCombination combination, Vector2 position) { }

	public virtual void MouseDrag(MouseCombination combination, Vector2 position, Vector2 positionChange,
		Vector2 velocity)
	{ }

	public virtual void MouseDragStart(MouseCombination combination, Vector2 position,
		Vector2 positionChange, Vector2 velocity)
	{ }

	public virtual void MouseDragEnd(MouseCombination combination, Vector2 position,
		Vector2 positionChange, Vector2 velocity)
	{ }

	public virtual void KeyDown(KeyCombination combination) { }
	public virtual void KeyUp(KeyCombination combination) { }
}
