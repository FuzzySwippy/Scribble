using Godot;
using Scribble.Application;
using Scribble.Drawing;

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

		NameLineEdit.TextChanged += text =>
			Global.LayerEditor.SetLayerName(Global.LayerEditor.SettingsLayerIndex, text);

		OpacitySlider.ValueChanged += value =>
		{
			Global.LayerEditor.SetLayerOpacity(Global.LayerEditor.SettingsLayerIndex,
				(float)(value / 100));
			OpacityPercentageLabel.Text = $"{(int)value}%";
		};
	}

	private void WindowShow()
	{
		Layer layer = Global.Canvas.Layers[Global.LayerEditor.SettingsLayerIndex];

		NameLineEdit.Text = layer.Name;
		NameLineEdit.GrabFocus();
		NameLineEdit.CaretColumn = NameLineEdit.Text.Length;

		OpacitySlider.Value = layer.Opacity * 100;
		OpacityPercentageLabel.Text = $"{(int)(layer.Opacity * 100)}%";
	}
}
