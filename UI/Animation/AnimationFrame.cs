using Godot;
using Scribble.Application;

namespace Scribble.UI.Animation;

public partial class AnimationFrame : Control
{
	private TextureRect backgroundTextureRect;
	private TextureRect previewTextureRect;
	private Control selectedControl;

	public ulong FrameId { get; set; }

	public void Init(ulong frameId, Texture2D backgroundTexture, Texture2D previewTexture)
	{
		SetupControls();

		FrameId = frameId;
		backgroundTextureRect.Texture = backgroundTexture;
		previewTextureRect.Texture = previewTexture;
	}

	private void SetupControls()
	{
		backgroundTextureRect = GetChild(0).GetChild<TextureRect>(0);
		previewTextureRect = GetChild(0).GetChild<TextureRect>(1);
		selectedControl = GetChild<Control>(1);

		Deselect();
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && !mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Right)
				ContextMenu.ShowMenu(mouseEvent.GlobalPosition,
				[
					new("Add Frame", Global.AnimationTimeline.AddFrame),
				new("Duplicate", Duplicate),
				new("Delete", Delete)
				]);
			else if (mouseEvent.ButtonIndex == MouseButton.Left)
				Global.Canvas.Animation.SelectFrame(FrameId);
		}
	}

	public void Select()
	{
		selectedControl.Show();
	}

	public void Deselect()
	{
		selectedControl.Hide();
	}

	public void Duplicate()
	{
		GD.Print($"Duplicate Frame: {FrameId}");
	}

	public void Delete()
	{
		GD.Print($"Delete Frame: {FrameId}");
	}
}
