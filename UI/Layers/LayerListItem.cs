using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class LayerListItem : Control
{
	[Export] private Texture2D visibilityCheckboxCheckedIcon;
	[Export] private Texture2D visibilityCheckboxUncheckedIcon;

	public ulong LayerID { get; private set; }
	public int Index { get; private set; }

	private Button MainButton { get; set; }
	private Label IndexLabel { get; set; }
	private TextureRect PreviewBackground { get; set; }
	private TextureRect Preview { get; set; }
	private Label NameLabel { get; set; }
	private Label OpacityLabel { get; set; }
	private CheckBox VisibilityCheckbox { get; set; }
	private TextureRect VisibilityCheckboxIcon { get; set; }

	public bool IsSelected => MainButton.ButtonPressed;

	public override void _Ready()
	{
		MainButton = GetChild<Button>(0);

		Control nodeParent = MainButton.GetChild(0).GetChild<Control>(0);
		IndexLabel = nodeParent.GetChild<Label>(0);
		PreviewBackground = nodeParent.GetChild<TextureRect>(1);
		Preview = PreviewBackground.GetChild<TextureRect>(0);
		NameLabel = nodeParent.GetChild(2).GetChild<Label>(0);
		OpacityLabel = nodeParent.GetChild(2).GetChild<Label>(1);
		VisibilityCheckbox = nodeParent.GetChild<CheckBox>(3);
		VisibilityCheckboxIcon = VisibilityCheckbox.GetGrandChild<TextureRect>(2);

		MainButton.Pressed += () =>
		{
			Global.Canvas.CurrentLayerIndex = Index;
			Global.LayerEditor.LayerSelected(Index);
		};
		VisibilityCheckbox.Pressed += () =>
			Global.Canvas.SetLayerVisibility(LayerID, VisibilityCheckbox.ButtonPressed);
		VisibilityCheckbox.Toggled += t =>
			VisibilityCheckboxIcon.Texture = t ?
				visibilityCheckboxCheckedIcon : visibilityCheckboxUncheckedIcon;
	}

	public override void _GuiInput(InputEvent e)
	{
		//Right click
		if (e is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && !mouseEvent.Pressed)
		{
			ContextMenu.ShowMenu(mouseEvent.GlobalPosition, new ContextMenuItem[]
			{
				Global.Canvas.Layers.Count > 1 ? new("Move Up", () => Global.Canvas.MoveLayerUp(Index)) : null,
				Global.Canvas.Layers.Count > 1 ? new("Move Down", () => Global.Canvas.MoveLayerDown(Index)) : null,
				Index < Global.Canvas.Layers.Count - 1 ? new("Merge Down", () => Global.Canvas.MergeDown(Index)) : null,
				new("Duplicate", () => Global.Canvas.DuplicateLayer(Index)),
				Global.Canvas.Layers.Count > 1 ? new("Delete", () => Global.Canvas.DeleteLayer(Index)) : null,
				new("Settings", () =>
				{
					Global.LayerEditor.SettingsLayerIndex = Index;
					WindowManager.Show("layer_settings");
				})
			});
		}
	}

	public void Init(ulong layerID, int index, string name, float opacity, bool visible, ImageTexture preview)
	{
		LayerID = layerID;
		Index = index;

		IndexLabel.Text = $"{index + 1}.";
		SetName(name);
		SetOpacity(opacity);
		SetVisibilityCheckboxNoSignal(visible);
		PreviewBackground.Texture = Global.BackgroundStyle.Texture;
		Preview.Texture = preview;

		Visible = true;
	}

	public void Select() =>
		MainButton.ButtonPressed = true;

	public void SetName(string name) =>
		NameLabel.Text = name;

	public void SetOpacity(float opacity) =>
		OpacityLabel.Text = $"Opacity: {(int)(opacity * 100)}%";

	public void SetVisibilityCheckboxNoSignal(bool visible)
	{
		VisibilityCheckbox.SetPressedNoSignal(visible);
		VisibilityCheckboxIcon.Texture = visible ?
			visibilityCheckboxCheckedIcon : visibilityCheckboxUncheckedIcon;
	}
}
