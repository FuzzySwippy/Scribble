using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class LayerEditor : Node
{
	private ItemList LayerList { get; set; }
	private Control LayerContextButtons { get; set; }

	//Buttons
	private Button MoveLayerUpButton { get; set; }
	private Button MoveLayerDownButton { get; set; }
	private Button ToggleLayerVisibilityButton { get; set; }
	private Button DuplicateLayerButton { get; set; }
	private Button DeleteLayerButton { get; set; }
	private Button ShowLayerSettingsButton { get; set; }

	public override void _Ready()
	{
		LayerList = GetChild(0).GetChild(0).GetChild(1).GetChild(0)
			.GetChild(0).GetChild<ItemList>(0);

		LayerList.ItemSelected += i => Global.Canvas.CurrentLayerIndex = (int)i;

		SetupButtons();
	}

	private void SetupButtons()
	{
		Control buttonContainer = GetChild(0).GetChild(0).GetChild<Control>(2);

		//NewLayerButton
		buttonContainer.GetChild(0).GetChild<Button>(0).Pressed += Global.Canvas.NewLayer;

		//Layer context buttons
		LayerContextButtons = buttonContainer.GetChild<Control>(1);

		MoveLayerUpButton = LayerContextButtons.GetChild<Button>(0);
		MoveLayerDownButton = LayerContextButtons.GetChild<Button>(1);
		ToggleLayerVisibilityButton = LayerContextButtons.GetChild<Button>(2);
		DuplicateLayerButton = LayerContextButtons.GetChild<Button>(3);
		DeleteLayerButton = LayerContextButtons.GetChild<Button>(4);
		ShowLayerSettingsButton = LayerContextButtons.GetChild<Button>(5);

		MoveLayerUpButton.Pressed += Global.Canvas.MoveLayerUp;
		MoveLayerDownButton.Pressed += Global.Canvas.MoveLayerDown;
		ToggleLayerVisibilityButton.Pressed += Global.Canvas.ToggleLayerVisibility;
		DuplicateLayerButton.Pressed += Global.Canvas.DuplicateLayer;
		DeleteLayerButton.Pressed += Global.Canvas.DeleteLayer;
		ShowLayerSettingsButton.Pressed += ShowLayerSettings;
	}

	public void SetMultiLayerButtonEnableState(bool enabled)
	{
		MoveLayerUpButton.Disabled = !enabled;
		MoveLayerDownButton.Disabled = !enabled;
		DeleteLayerButton.Disabled = !enabled;
	}

	private void ShowLayerSettings()
	{
		GD.Print("ShowLayerSettings");
	}

	public void UpdateLayerList()
	{
		LayerList.Clear();

		for (int i = 0; i < Global.Canvas.Layers.Count; i++)
			LayerList.AddItem($"{i + 1}. {Global.Canvas.Layers[i].Name}");

		LayerList.Select(Global.Canvas.CurrentLayerIndex);

		SetMultiLayerButtonEnableState(Global.Canvas.Layers.Count > 1);
	}
}
