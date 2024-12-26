using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public partial class HistoryList : ItemList
{
	private History History => Global.Canvas.History;
	private ScrollContainer ScrollContainer { get; set; }
	private ScrollBar VerticalScrollBar { get; set; }

	#region Mappings
	[ExportGroup("Icons")]
	[Export] private Texture2D drawPencilIcon;
	[Export] private Texture2D ditherIcon;
	[Export] private Texture2D drawRectangleIcon;
	[Export] private Texture2D drawLineIcon;
	[Export] private Texture2D drawFloodIcon;
	[Export] private Texture2D selectionChangedIcon;
	[Export] private Texture2D selectionOffsetChangedIcon;
	[Export] private Texture2D selectionClearedIcon;
	[Export] private Texture2D selectionMovedIcon;
	[Export] private Texture2D selectionRotatedIcon;
	[Export] private Texture2D layerDeletedIcon;
	[Export] private Texture2D layerCreatedIcon;
	[Export] private Texture2D layerMovedIcon;
	[Export] private Texture2D layerMergedIcon;
	[Export] private Texture2D layerDuplicatedIcon;
	[Export] private Texture2D layerOpacityChangedIcon;
	[Export] private Texture2D layerNameChangedIcon;
	[Export] private Texture2D layerVisibilityChangedIcon;
	[Export] private Texture2D flipperVerticallyIcon;
	[Export] private Texture2D flipperHorizontallyIcon;
	[Export] private Texture2D rotateClockwiseIcon;
	[Export] private Texture2D rotateCounterClockwiseIcon;
	[Export] private Texture2D resizeCanvasIcon;
	[Export] private Texture2D cropToContentIcon;
	[Export] private Texture2D cutIcon;
	[Export] private Texture2D pasteIcon;
	[Export] private Texture2D clearPixelsIcon;

	private Dictionary<HistoryActionType, HistoryListItemData> HistoryItemDataMap { get; set; }
	#endregion

	public override void _Ready()
	{
		HistoryItemDataMap = new()
		{
			{ HistoryActionType.DrawPencil, new("Draw Pencil", drawPencilIcon) },
			{ HistoryActionType.DrawDither, new("Draw Dither", ditherIcon) },
			{ HistoryActionType.DrawRectangle, new("Draw Rectangle", drawRectangleIcon) },
			{ HistoryActionType.DrawLine, new("Draw Line", drawLineIcon) },
			{ HistoryActionType.DrawFlood, new("Draw Flood", drawFloodIcon) },
			{ HistoryActionType.SelectionChanged, new("Selection Changed", selectionChangedIcon) },
			{ HistoryActionType.SelectionOffsetChanged, new("Selection Offset Changed", selectionOffsetChangedIcon) },
			{ HistoryActionType.SelectionCleared, new("Selection Cleared", selectionClearedIcon) },
			{ HistoryActionType.SelectionMoved, new("Selection Moved", selectionMovedIcon) },
			{ HistoryActionType.SelectionRotated, new("Selection Rotated", selectionRotatedIcon) },
			{ HistoryActionType.LayerDeleted, new("Layer Deleted", layerDeletedIcon) },
			{ HistoryActionType.LayerCreated, new("Layer Created", layerCreatedIcon) },
			{ HistoryActionType.LayerMoved, new("Layer Moved", layerMovedIcon) },
			{ HistoryActionType.LayerMerged, new("Layer Merged", layerMergedIcon) },
			{ HistoryActionType.LayerDuplicated, new("Layer Duplicated", layerDuplicatedIcon) },
			{ HistoryActionType.LayerOpacityChanged, new("Layer Opacity Changed", layerOpacityChangedIcon) },
			{ HistoryActionType.LayerNameChanged, new("Layer Name Changed", layerNameChangedIcon) },
			{ HistoryActionType.LayerVisibilityChanged, new("Layer Visibility Changed", layerVisibilityChangedIcon) },
			{ HistoryActionType.FlippedVertically, new("Flipped Vertically", flipperVerticallyIcon) },
			{ HistoryActionType.FlippedHorizontally, new("Flipped Horizontally", flipperHorizontallyIcon) },
			{ HistoryActionType.RotatedClockwise, new("Rotated Clockwise", rotateClockwiseIcon) },
			{ HistoryActionType.RotatedCounterClockwise, new("Rotated Counter Clockwise", rotateCounterClockwiseIcon) },
			{ HistoryActionType.ResizeCanvas, new("Resized Canvas", resizeCanvasIcon) },
			{ HistoryActionType.CropToContent, new("Cropped To Content", cropToContentIcon) },
			{ HistoryActionType.Cut, new("Cut", cutIcon) },
			{ HistoryActionType.Paste, new("Paste", pasteIcon) },
			{ HistoryActionType.ClearPixels, new("Clear Pixels", clearPixelsIcon) }
		};

		ScrollContainer = GetParent<ScrollContainer>();
		VerticalScrollBar = ScrollContainer.GetVScrollBar();
		VerticalScrollBar.Changed += OnScrollChanged;
		ItemSelected += OnItemSelected;
	}

	private void OnScrollChanged() =>
		ScrollContainer.ScrollVertical = Mathf.CeilToInt(ScrollContainer.GetVScrollBar().MaxValue);

	private void OnItemSelected(long index)
	{
		if (index == -1)
			return;

		History.JumpToAction((int)index);
	}

	public void Update()
	{
		Clear();
		foreach (HistoryAction action in History.Actions)
		{
			HistoryListItemData data = HistoryItemDataMap[action.ActionType];
			AddItem(data.Name, data.Icon);
		}

		UpdateColorsAndSelection();
	}

	public void UpdateColorsAndSelection()
	{
		for (int i = 0; i < History.Actions.Count; i++)
		{
			if (i <= History.LastActionIndex)
				SetItemCustomFgColor(i, new Color(1, 1, 1));
			else
				SetItemCustomFgColor(i, new Color(1, 1, 1, 0.5f));
		}

		if (History.LastActionIndex >= 0)
			Select(History.LastActionIndex);
		else
			DeselectAll();
	}
}
