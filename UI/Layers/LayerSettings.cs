using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class LayerSettings : Node
{
	[Export] private LineEdit NameLineEdit { get; set; }
	[Export] private HSlider OpacitySlider { get; set; }
	[Export] private Label OpacityPercentageLabel { get; set; }
	[Export] private OptionButton BlendModeOptionButton { get; set; }

	//History
	private float OldOpacity { get; set; }

	public override void _Ready()
	{
		Main.Ready += () => WindowManager.Get("layer_settings").WindowShow += WindowShow;

		BlendModeOptionButton.AddEnumOptions<BlendMode>();

		NameLineEdit.TextChanged += NameTextChanged;
		OpacitySlider.ValueChanged += OpacityValueChanged;
		BlendModeOptionButton.ItemSelected += BlendModeItemSelected;
	}

	private void NameTextChanged(string text) =>
		Global.LayerEditor.SetLayerName(Global.LayerEditor.SettingsLayerIndex, text);

	private void OpacityValueChanged(double value)
	{
		Global.LayerEditor.SetLayerOpacity(Global.LayerEditor.SettingsLayerIndex,
				(float)(value / 100));
		OpacityPercentageLabel.Text = $"{(int)value}%";
	}

	private void BlendModeItemSelected(long index) =>
		Global.LayerEditor.SetLayerBlendMode(Global.LayerEditor.SettingsLayerIndex, (BlendMode)index);

	private void WindowShow()
	{
		Layer layer = Global.Canvas.Layers[Global.LayerEditor.SettingsLayerIndex];

		NameLineEdit.Text = layer.Name;
		NameLineEdit.GrabFocus();
		NameLineEdit.CaretColumn = NameLineEdit.Text.Length;

		OpacitySlider.Value = layer.Opacity * 100;
		OpacityPercentageLabel.Text = $"{(int)(layer.Opacity * 100)}%";

		BlendModeOptionButton.Selected = (int)layer.BlendMode;
	}
}
