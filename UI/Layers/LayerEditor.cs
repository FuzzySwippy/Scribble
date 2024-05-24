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

	//Buttons
	private Control LayerContextButtons { get; set; }

	private Button MoveLayerUpButton { get; set; }
	private Button MoveLayerDownButton { get; set; }
	private Button MergeDownButton { get; set; }
	private Button DuplicateLayerButton { get; set; }
	private Button DeleteLayerButton { get; set; }
	private Button ShowLayerSettingsButton { get; set; }

	public int SettingsLayerIndex { get; set; }

	public override void _Ready()
	{
		LayerListItemPool = new ObjectPool<LayerListItem>(
			this.GetGrandChild(2, 0).GetChild(1).GetGrandChild(3, 0), Global.LayerListItemPrefab, 10, 5,
			l => l.Visible = false);

		SetupButtons();
	}

	private void SetupButtons()
	{
		Control buttonContainer = GetChild(0).GetChild(0).GetChild<Control>(2);

		//NewLayerButton
		buttonContainer.GetChild(0).GetChild<Button>(0).Pressed += () => Global.Canvas.NewLayer();

		//Layer context buttons
		LayerContextButtons = buttonContainer.GetChild<Control>(1);

		MoveLayerUpButton = LayerContextButtons.GetChild<Button>(0);
		MoveLayerDownButton = LayerContextButtons.GetChild<Button>(1);
		MergeDownButton = LayerContextButtons.GetChild<Button>(2);
		DuplicateLayerButton = LayerContextButtons.GetChild<Button>(3);
		DeleteLayerButton = LayerContextButtons.GetChild<Button>(4);
		ShowLayerSettingsButton = LayerContextButtons.GetChild<Button>(5);

		MoveLayerUpButton.Pressed += () => Global.Canvas.MoveLayerUp(Global.Canvas.CurrentLayerIndex);
		MoveLayerDownButton.Pressed += () => Global.Canvas.MoveLayerDown(Global.Canvas.CurrentLayerIndex);
		MergeDownButton.Pressed += () => Global.Canvas.MergeDown(Global.Canvas.CurrentLayerIndex);
		DuplicateLayerButton.Pressed += () => Global.Canvas.DuplicateLayer(Global.Canvas.CurrentLayerIndex);
		DeleteLayerButton.Pressed += () => Global.Canvas.DeleteLayer(Global.Canvas.CurrentLayerIndex);
		ShowLayerSettingsButton.Pressed += () =>
		{
			SettingsLayerIndex = Global.Canvas.CurrentLayerIndex;
			WindowManager.Show("layer_settings");
		};
	}

	public void SetMultiLayerButtonEnableState(bool enabled)
	{
		MoveLayerUpButton.Disabled = !enabled;
		MoveLayerDownButton.Disabled = !enabled;
		DeleteLayerButton.Disabled = !enabled;
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

	private void CreateLayerListItem(ulong layerID, int index, string name, float opacity, bool visible, ImageTexture preview)
	{
		LayerListItem item = LayerListItemPool.Get();

		item.Init(layerID, index, name, opacity, visible, preview);
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
			CreateLayerListItem(layer.ID, i, layer.Name, layer.Opacity, layer.Visible, layer.Preview);
		}

		SetMultiLayerButtonEnableState(Global.Canvas.Layers.Count > 1);
		LayerSelected(Global.Canvas.CurrentLayerIndex);
	}

	public void SetLayerName(int index, string name) =>
		LayerListItems[index].SetName(name);

	public void SetLayerOpacity(int index, float opacity)
	{
		LayerListItems[index].SetOpacity(opacity);
		Global.Canvas.SetLayerOpacity(index, opacity);
	}

	public void LayerSelected(int index) =>
		MergeDownButton.Disabled = index == Global.Canvas.Layers.Count - 1;
}
