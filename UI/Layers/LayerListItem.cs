using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class LayerListItem : Control
{
	public ulong LayerID { get; private set; }
	public int Index { get; private set; }

	private Button MainButton { get; set; }
	private Label IndexLabel { get; set; }
	private TextureRect Preview { get; set; }
	private Label NameLabel { get; set; }
	private Label OpacityLabel { get; set; }
	private CheckBox VisibilityCheckbox { get; set; }

	public override void _Ready()
	{
		MainButton = GetChild<Button>(0);

		Control nodeParent = MainButton.GetChild(0).GetChild<Control>(0);
		IndexLabel = nodeParent.GetChild<Label>(0);
		Preview = nodeParent.GetChild<TextureRect>(1);
		NameLabel = nodeParent.GetChild(2).GetChild<Label>(0);
		OpacityLabel = nodeParent.GetChild(2).GetChild<Label>(1);
		VisibilityCheckbox = nodeParent.GetChild<CheckBox>(3);

		MainButton.Pressed += () => Global.Canvas.CurrentLayerIndex = Index;
		VisibilityCheckbox.Pressed += () => Global.Canvas.SetLayerVisibility(LayerID, VisibilityCheckbox.ButtonPressed);
	}

	public void Init(ulong layerID, int index, string name, float opacity, bool visible, Texture2D preview)
	{
		LayerID = layerID;
		Index = index;

		IndexLabel.Text = (index + 1).ToString();
		NameLabel.Text = name;
		OpacityLabel.Text = $"Opacity: {opacity * 100}%";
		VisibilityCheckbox.ButtonPressed = visible;
		Preview.Texture = preview;

		Visible = true;
	}

	public void Select() =>
		MainButton.ButtonPressed = true;
}
