using Godot;
using Scribble.Application;
using Scribble.Drawing;

namespace Scribble.UI;

public partial class ResizeWindow : Control
{
	private LineEdit ResolutionX_LineEdit { get; set; }
	private LineEdit ResolutionY_LineEdit { get; set; }
	private OptionButton ResizeType_OptionButton { get; set; }
	private Button Resize_Button { get; set; }
	private Button Cancel_Button { get; set; }

	private int ResolutionX { get; set; }
	private int ResolutionY { get; set; }
	private ResizeType ResizeType { get; set; } = ResizeType.Scale;

	public override void _Ready()
	{
		Control lineContainer = GetChild<Control>(0);
		ResolutionX_LineEdit = lineContainer.GetChild(0).GetChild<LineEdit>(1);
		ResolutionY_LineEdit = lineContainer.GetChild(0).GetChild<LineEdit>(3);
		ResizeType_OptionButton = lineContainer.GetChild(1).GetChild<OptionButton>(1);
		Resize_Button = lineContainer.GetChild(2).GetChild<Button>(0);
		Cancel_Button = lineContainer.GetChild(2).GetChild<Button>(1);

		ResolutionX_LineEdit.TextChanged += text => ResolutionSet(text, true);
		ResolutionY_LineEdit.TextChanged += text => ResolutionSet(text, false);
		ResizeType_OptionButton.ItemSelected += (i) => ResizeType = (ResizeType)i;
		Resize_Button.Pressed += ResizeCanvas;
		Cancel_Button.Pressed += Close;

		Main.Ready += () => WindowManager.Get("resize_canvas").WindowShow += WindowShow;
	}

	private void WindowShow()
	{
		ResolutionX_LineEdit.Text = Global.Canvas.Size.X.ToString();
		ResolutionY_LineEdit.Text = Global.Canvas.Size.Y.ToString();
		ResizeType_OptionButton.Selected = (int)ResizeType.Scale;

		ResolutionX = Global.Canvas.Size.X;
		ResolutionY = Global.Canvas.Size.Y;
		ResizeType = ResizeType.Scale;
	}

	private void ResolutionSet(string text, bool isX)
	{
		int value = int.Parse(text);
		bool valueChanged = false;

		if (value > Canvas.MaxResolution)
		{
			value = Canvas.MaxResolution;
			valueChanged = true;
		}
		else if (value < Canvas.MinResolution)
		{
			value = Canvas.MinResolution;
			valueChanged = true;
		}

		if (isX)
		{
			ResolutionX = value;
			if (valueChanged)
			{
				ResolutionX_LineEdit.Text = value.ToString();
				ResolutionX_LineEdit.CaretColumn = value.ToString().Length;
			}
		}
		else
		{
			ResolutionY = value;
			if (valueChanged)
			{
				ResolutionY_LineEdit.Text = value.ToString();
				ResolutionY_LineEdit.CaretColumn = value.ToString().Length;
			}
		}
	}

	private void ResizeCanvas()
	{
		Global.Canvas.Resize(new Vector2I(ResolutionX, ResolutionY), ResizeType);
		WindowManager.Get("resize_canvas").Hide();
	}

	private void Close() =>
		WindowManager.Get("resize_canvas").Hide();
}
