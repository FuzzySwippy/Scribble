using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class LayerSettings : Node
{
	private LineEdit NameLineEdit { get; set; }
	private HSlider OpacitySlider { get; set; }
	private Label OpacityPercentageLabel { get; set; }

	public override void _Ready()
	{
		NameLineEdit = this.GetGrandChild(2).GetChild<LineEdit>(1);
		OpacitySlider = GetChild(0).GetGrandChild<HSlider>(2, 1);
		OpacityPercentageLabel = GetChild(0).GetChild(1).GetChild<Label>(2);

		Main.Ready += () => WindowManager.Get("layer_settings").WindowShow += WindowShow;
		NameLineEdit.TextChanged += (text) =>
		{
			Global.Canvas.CurrentLayer.Name = text;
			Global.LayerEditor.SetLayerName(Global.LayerEditor.SettingsLayerIndex, text);
		};
		OpacitySlider.ValueChanged += (value) =>
		{
			Global.LayerEditor.SetLayerOpacity(Global.LayerEditor.SettingsLayerIndex, (float)(value / 100));
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
