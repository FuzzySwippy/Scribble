using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Input;

namespace Scribble.UI.Animation;

public partial class AnimationFrame : Control
{
	private TextureRect backgroundTextureRect;
	private TextureRect previewTextureRect;
	private Control selectedControl;

	public bool IsSelected => selectedControl.Visible;
	private Frame Frame { get; set; }
	private int FrameIndex { get; set; }
	public AnimationFrameInsertPosition InsertPosition { get; set; }

	private bool IsDragging { get; set; }
	private Vector2 DragOffset { get; set; }

	public void Init(Frame frame, int frameIndex, Texture2D backgroundTexture, Texture2D previewTexture)
	{
		SetupControls();

		Frame = frame;
		FrameIndex = frameIndex;
		backgroundTextureRect.Texture = backgroundTexture;
		previewTextureRect.Texture = previewTexture;

		Mouse.DragStart += DragStart;
		Mouse.Drag += Drag;
		Mouse.DragEnd += DragEnd;
		Mouse.ButtonUp += MouseButtonUp;
	}

	public void UnInit()
	{
		Mouse.DragStart -= DragStart;
		Mouse.Drag -= Drag;
		Mouse.DragEnd -= DragEnd;
		Mouse.ButtonUp -= MouseButtonUp;

		Frame = null;
	}

	private void SetupControls()
	{
		backgroundTextureRect = GetChild(0).GetChild<TextureRect>(0);
		previewTextureRect = GetChild(0).GetChild<TextureRect>(1);
		selectedControl = GetChild<Control>(1);

		Deselect();
	}

	private void MouseButtonUp(MouseCombination combination, Vector2 position)
	{
		if (IsDragging || !GetGlobalRect().HasPoint(position))
			return;

		if (combination.button == MouseButton.Left)
			Global.Canvas.Animation.SelectFrame(Frame.Id);
		else if (combination.button == MouseButton.Right)
			ContextMenu.ShowMenu(position,
			[
				new("Add Frame", Global.AnimationTimeline.AddFrame),
				new("Duplicate", Duplicate),
				(Global.AnimationTimeline.AnimationFrameCount > 1 ? new("Delete", Delete) : null)
			]);
	}

	private void DragStart(MouseCombination combination, Vector2 position, Vector2 positionChange, Vector2 velocity)
	{
		if (combination.button != MouseButton.Left || !GetGlobalRect().HasPoint(position) || Global.Canvas.Animation.Frames.Count <= 1)
			return;

		Global.Canvas.Animation.SelectFrame(Frame.Id);
		Global.AnimationTimeline.FrameStartedDragging(this);
		Global.Canvas.Animation.RemoveFrame(Frame.Id);

		Reparent(Global.DragCanvas);
		DragOffset = position - GlobalPosition;
		IsDragging = true;
		Deselect();
	}

	private void Drag(MouseCombination combination, Vector2 position, Vector2 positionChange, Vector2 velocity)
	{
		if (!IsDragging)
			return;

		Position = position - DragOffset;
	}

	private void DragEnd(MouseCombination combination, Vector2 position, Vector2 positionChange, Vector2 velocity)
	{
		if (!IsDragging)
			return;

		IsDragging = false;
		Hide();
		Global.AnimationTimeline.FrameEndedDragging();

		if (InsertPosition != null)
			Global.Canvas.Animation.InsertFrame(Frame, InsertPosition.InsertIndex);
		else
			Global.Canvas.Animation.InsertFrame(Frame, FrameIndex);

		UnInit();
		QueueFree();
	}

	public void Select() =>
		selectedControl.Show();

	public void Deselect() =>
		selectedControl.Hide();

	public void Duplicate() =>
		Global.Canvas.Animation.DuplicateFrame(Frame.Id);

	public void Delete() =>
		Global.Canvas.Animation.RemoveFrame(Frame.Id);
}
