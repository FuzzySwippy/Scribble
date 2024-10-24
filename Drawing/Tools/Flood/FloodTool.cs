using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class FloodTool : DrawingTool
{
	public float Threshold { get; set; } = 0;

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			DrawHistoryAction historyAction = new(HistoryActionType.DrawFlood, Canvas.CurrentLayer.ID);
			Brush.Flood(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor, Threshold, historyAction,
				BrushPixelType.Normal);
			Canvas.History.AddAction(historyAction);
		}
	}
}
