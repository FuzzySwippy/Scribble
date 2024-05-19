using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class LayerEditor : Node
{
	private ObjectPool<LayerListItem> LayerListItemPool { get; set; }
	private List<LayerListItem> LayerListItems { get; } = new();
	private Dictionary<ulong, Texture2D> LayerPreviews { get; } = new();

	//Buttons
	private Control LayerContextButtons { get; set; }

	private Button MoveLayerUpButton { get; set; }
	private Button MoveLayerDownButton { get; set; }
	private Button MergeDownButton { get; set; }
	private Button DuplicateLayerButton { get; set; }
	private Button DeleteLayerButton { get; set; }
	private Button ShowLayerSettingsButton { get; set; }

	public override void _Ready()
	{
		LayerListItemPool = new ObjectPool<LayerListItem>(
			this.GetGrandChild(2, 0).GetChild(1).GetGrandChild(3, 0), Global.LayerListItemPrefab, 10, 5,
			l => l.Visible = false);

		Global.Canvas.LayerDeleted += layer => LayerPreviews.Remove(layer.ID);

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
		MergeDownButton = LayerContextButtons.GetChild<Button>(2);
		DuplicateLayerButton = LayerContextButtons.GetChild<Button>(3);
		DeleteLayerButton = LayerContextButtons.GetChild<Button>(4);
		ShowLayerSettingsButton = LayerContextButtons.GetChild<Button>(5);

		MoveLayerUpButton.Pressed += Global.Canvas.MoveLayerUp;
		MoveLayerDownButton.Pressed += Global.Canvas.MoveLayerDown;
		MergeDownButton.Pressed += Global.Canvas.MergeDown;
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

	private void ClearList()
	{
		foreach (LayerListItem item in LayerListItems)
		{
			item.Visible = false;
			LayerListItemPool.Return(item);
		}

		LayerListItems.Clear();
	}

	private void CreateLayerListItem(ulong layerID, int index, string name, float opacity, bool visible)
	{
		LayerListItem item = LayerListItemPool.Get();

		item.Init(layerID, index, name, opacity, visible, null);
		LayerListItems.Add(item);

		Node parent = item.GetParent();
		parent.MoveChild(item, parent.GetChildCount() - 1);

		if (Global.Canvas.CurrentLayerIndex == index)
			item.Select();
	}

	public void UpdateLayerList()
	{
		ClearList();

		for (int i = 0; i < Global.Canvas.Layers.Count; i++)
		{
			Layer layer = Global.Canvas.Layers[i];
			CreateLayerListItem(layer.ID, i, layer.Name, layer.Opacity, layer.Visible);
		}

		SetMultiLayerButtonEnableState(Global.Canvas.Layers.Count > 1);
	}
}
