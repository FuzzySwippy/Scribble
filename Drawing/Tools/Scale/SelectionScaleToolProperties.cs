using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Pencil;

public partial class SelectionScaleToolProperties : ToolProperties
{
	[Export] private CheckButton useForceFactorCheckButton;
	[Export] private SpinBox forceFactorXSpinBox;
	[Export] private SpinBox forceFactorYSpinBox;

	public override void _Ready()
	{
		useForceFactorCheckButton.Toggled += OnUseForceFactorToggled;
		forceFactorXSpinBox.ValueChanged += OnForceFactorXChanged;
		forceFactorYSpinBox.ValueChanged += OnForceFactorYChanged;
	}

	private void OnUseForceFactorToggled(bool value)
	{
		((SelectionScaleTool)Tool).UseForceFactor = value;
		forceFactorXSpinBox.Editable = value;
		forceFactorYSpinBox.Editable = value;
	}

	private void OnForceFactorXChanged(double value) =>
		((SelectionScaleTool)Tool).ForceFactor = new Vector2((float)value, ((SelectionScaleTool)Tool).ForceFactor.Y);

	private void OnForceFactorYChanged(double value) =>
		((SelectionScaleTool)Tool).ForceFactor = new Vector2(((SelectionScaleTool)Tool).ForceFactor.X, (float)value);

	public override void UpdateProperties()
	{
		OnUseForceFactorToggled(useForceFactorCheckButton.ButtonPressed);
		OnForceFactorXChanged(forceFactorXSpinBox.Value);
		OnForceFactorYChanged(forceFactorYSpinBox.Value);
	}
}
