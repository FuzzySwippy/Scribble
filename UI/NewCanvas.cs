using System;
using Godot;
using Scribble;
using Scribble.Application;
using Scribble.Drawing;

public partial class NewCanvas : Control
{
	private const BackgroundType DefaultBackground = BackgroundType.Transparent;

	private LineEdit ResolutionX_LineEdit { get; set; }
	private LineEdit ResolutionY_LineEdit { get; set; }
	private MenuButton Background_MenuButton { get; set; }
	private Button New_Button { get; set; }
	private Button Cancel_Button { get; set; }

	private int ResolutionX { get; set; } = Canvas.DefaultResolution;
	private int ResolutionY { get; set; } = Canvas.DefaultResolution;
	private BackgroundType Background { get; set; } = DefaultBackground;

	public override void _Ready()
	{
		Control lineContainer = GetChild<Control>(0);
		ResolutionX_LineEdit = lineContainer.GetChild(0).GetChild<LineEdit>(1);
		ResolutionY_LineEdit = lineContainer.GetChild(0).GetChild<LineEdit>(3);
		Background_MenuButton = lineContainer.GetChild(1).GetChild<MenuButton>(1);
		New_Button = lineContainer.GetChild(2).GetChild<Button>(0);
		Cancel_Button = lineContainer.GetChild(2).GetChild<Button>(1);

		ResolutionX_LineEdit.TextChanged += text => ResolutionSet(text, true);
		ResolutionY_LineEdit.TextChanged += text => ResolutionSet(text, false);
		Background_MenuButton.GetPopup().IndexPressed += BackgroundMenuButtonIndexPressed;
		New_Button.Pressed += CreateNewCanvas;
		Cancel_Button.Pressed += Close;

		Main.Ready += () => WindowManager.Get("new_canvas").WindowShow += WindowShow;
	}

	private void WindowShow()
	{
		ResolutionX_LineEdit.Text = Canvas.DefaultResolution.ToString();
		ResolutionY_LineEdit.Text = Canvas.DefaultResolution.ToString();
		Background_MenuButton.Text = DefaultBackground.ToString();

		ResolutionX = ResolutionY = Canvas.DefaultResolution;
		Background = DefaultBackground;
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

	private void BackgroundMenuButtonIndexPressed(long index)
	{
		Background = Enum.Parse<BackgroundType>(Background_MenuButton.GetPopup().GetItemText((int)index));
		Background_MenuButton.Text = Background.ToString();
	}

	private void CreateNewCanvas()
	{
		Global.Canvas.CreateNew(new Vector2I(ResolutionX, ResolutionY), Background);
		WindowManager.Get("new_canvas").Hide();
	}

	private void Close()
	{
		//Later check if there is a canvas open, if not, close the application
		//Also the button text should be set accordingly
		WindowManager.Get("new_canvas").Hide();
	}
}
