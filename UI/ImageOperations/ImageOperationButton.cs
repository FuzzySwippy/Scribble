using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class ImageOperationButton : Button
{
	[Export] private ImageOperationType imageOperationType;

	public override void _Pressed()
	{
		switch (imageOperationType)
		{
			case ImageOperationType.FlipVertically:
				Global.Canvas.FlipVertically();
				break;
			case ImageOperationType.FlipHorizontally:
				Global.Canvas.FlipHorizontally();
				break;
			case ImageOperationType.RotateClockwise:
				Global.Canvas.RotateClockwise();
				break;
			case ImageOperationType.RotateCounterClockwise:
				Global.Canvas.RotateCounterClockwise();
				break;
			case ImageOperationType.Resize:
				WindowManager.Show("resize_canvas");
				break;
			default:
				return;
		}
	}
}
