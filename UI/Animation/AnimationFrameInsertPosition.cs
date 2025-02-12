using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;

namespace Scribble.UI.Animation;

public partial class AnimationFrameInsertPosition : Control
{
	private TextureRect previewTextureRect;
	private Control sensor;

	public int InsertIndex { get; set; }

	public override void _Ready()
	{
		previewTextureRect = GetChild<TextureRect>(0);
		sensor = GetChild<Control>(1);
		previewTextureRect.Hide();

		Mouse.Drag += Drag;
	}

	public override void _ExitTree()
	{
		Mouse.Drag -= Drag;
		if (Global.AnimationTimeline.DraggedFrame?.InsertPosition == this)
			Global.AnimationTimeline.DraggedFrame.InsertPosition = null;
	}

	public void Init(int insertIndex) =>
		InsertIndex = insertIndex;

	private void Drag(MouseCombination combination, Vector2 position, Vector2 positionChange, Vector2 velocity)
	{
		if (Global.AnimationTimeline.DraggedFrame == null)
			return;

		previewTextureRect.Visible = sensor.GetGlobalRect().HasPoint(position);

		if (previewTextureRect.Visible)
			Global.AnimationTimeline.DraggedFrame.InsertPosition = this;
		else if (Global.AnimationTimeline.DraggedFrame.InsertPosition == this)
			Global.AnimationTimeline.DraggedFrame.InsertPosition = null;
	}
}
