using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;

namespace Scribble.UI;

public partial class UserInterface : Node
{
	public static float ContentScale
	{
		get => Global.Settings.ContentScale;
		set
		{
			Vector2 zoomAmount = CameraController.ZoomAmount;
			Vector2 relativePosition = CameraController.RelativePosition;

			Global.Settings.ContentScale = value;
			if (Global.Settings.ContentScale < Settings.MinContentScale)
				Global.Settings.ContentScale = Settings.MinContentScale;
			else if (Global.Settings.ContentScale > Settings.MaxContentScale)
				Global.Settings.ContentScale = Settings.MaxContentScale;
			Global.Settings.Save();

			Main.Window.ContentScaleFactor = Global.Settings.ContentScale;

			CameraController.ZoomAmount = zoomAmount;
			CameraController.RelativePosition = relativePosition;

			DebugInfo.Set("ui_scale", Main.Window.ContentScaleFactor);
		}
	}

	public override void _Ready()
	{
		Main.Ready += () => ContentScale = Main.Window.ContentScaleFactor;
		Keyboard.KeyDown += KeyDown;

		HueSlider.GradientSetup();
		ShowAllCanvasLayers();
	}

	private void KeyDown(KeyCombination combination)
	{
		//UI Scaling
		if (combination.modifiers == KeyModifierMask.MaskCtrl)
		{
			if (combination.key == Key.Equal)
				ContentScale += Settings.ContentScaleStep;

			if (combination.key == Key.Minus)
				ContentScale -= Settings.ContentScaleStep;
		}

		//Debug
		/*if (combination.key == Key.M)
			WindowManager.ShowModal("Test", new ModalButton[] 
			{ 
				new("Yes", ModalButtonType.Confirm, () => GD.Print("Yes")),
				new("Maybe", ModalButtonType.Normal, () => GD.Print("Maybe")),
				new("No", ModalButtonType.Cancel, () => GD.Print("No")),
			});*/
	}

	private void ShowAllCanvasLayers()
	{
		foreach (Node node in GetChildren())
			if (node is CanvasLayer layer)
				layer.Show();
	}
}