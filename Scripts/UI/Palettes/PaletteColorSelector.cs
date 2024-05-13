using Godot;

namespace Scribble;
public class PaletteColorSelector
{
	public Control Control { get; }
	public ColorRect ColorRect { get; }
	public Control SelectionIndicator { get; }

	public Button ColorButton { get; }
	public Button AddButton { get; }

	private bool visible;

	public PaletteColorSelector(Control control)
	{
		Control = control;

		ColorButton = control.GetChild<Button>(1);
		AddButton = control.GetChild<Button>(2);

		ColorRect = ColorButton.GetChild<ColorRect>(1);
		SelectionIndicator = ColorButton.GetChild(1).GetChild<Control>(0);
	}

	public void Show()
	{
		ColorButton.Show();
		AddButton.Hide();
		visible = true;
	}

	public void Hide()
	{
		ColorButton.Hide();
		SelectionIndicator.Hide();
		visible = false;
	}

	public void UpdateAddButton(bool paletteLocked) => AddButton.Visible = !paletteLocked && !visible;
}