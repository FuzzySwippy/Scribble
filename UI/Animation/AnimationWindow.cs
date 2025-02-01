using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class AnimationWindow : Control
{
	[ExportGroup("Settings")]
	[Export] private CheckButton blackIsTransparentCheckButton;
	[Export] private CheckButton loopCheckButton;
	[Export] private SpinBox frameTimeSpinBox;

	[ExportGroup("Buttons")]
	[Export] private Button okButton;

	public override void _Ready()
	{
		SetupButtons();
		SetupControls();
		Main.Ready += () => WindowManager.Get("animation").WindowShow += SetupWindow;
	}

	private void Close() => WindowManager.Get("animation").Hide();

	private void SetupButtons() =>
		okButton.Pressed += Close;

	private void SetupControls()
	{
		blackIsTransparentCheckButton.Toggled += OnBlackIsTransparentCheckButtonToggled;
		loopCheckButton.Toggled += OnLoopCheckButtonToggled;
		frameTimeSpinBox.ValueChanged += OnFrameTimeSpinBoxValueChanged;
	}

	private void SetupWindow()
	{
		blackIsTransparentCheckButton.ButtonPressed = Global.Canvas.Animation.BlackIsTransparent;
		loopCheckButton.ButtonPressed = Global.Canvas.Animation.Loop;
		frameTimeSpinBox.Value = Global.Canvas.Animation.FrameTimeMs;
	}

	private void OnBlackIsTransparentCheckButtonToggled(bool toggledOn) =>
		Global.Canvas.Animation.BlackIsTransparent = toggledOn;

	private void OnLoopCheckButtonToggled(bool toggledOn) =>
		Global.Canvas.Animation.Loop = toggledOn;

	private void OnFrameTimeSpinBoxValueChanged(double value) =>
		Global.Canvas.Animation.FrameTimeMs = (int)value;
}
