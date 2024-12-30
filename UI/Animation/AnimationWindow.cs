using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class AnimationWindow : Control
{
	[ExportGroup("Settings")]
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
		loopCheckButton.Toggled += OnLoopCheckButtonToggled;
		frameTimeSpinBox.ValueChanged += OnFrameTimeSpinBoxValueChanged;
	}

	private void SetupWindow()
	{
		//loopCheckButton.ButtonPressed = ...;
		//frameTimeSpinBox.Value = ...;
	}

	private void OnLoopCheckButtonToggled(bool toggledOn)
	{
		//...
	}

	private void OnFrameTimeSpinBoxValueChanged(double value)
	{
		//...
	}
}
