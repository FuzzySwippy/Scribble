using Godot;
using Scribble.Application;
using Scribble.Drawing;

namespace Scribble.UI;

public partial class QuickPencilSelector : Control
{
	private TextureRect selectorRect;
	private ColorRect colorRect;


	[Export]
	public QuickPencilType Type { get; set; }

	[Export(PropertyHint.MultilineText)]
	private string ToolTip { get; set; }

	public new bool Visible
	{
		get => selectorRect.Visible;
		set => selectorRect.Visible = value;
	}

	public override void _Ready()
	{
		selectorRect = GetChild(0).GetChild(1).GetChild<TextureRect>(0);
		selectorRect.TooltipText = ToolTip;
		Visible = false;

		colorRect = GetChild(0).GetChild<ColorRect>(1);
		colorRect.GuiInput += GuiInputEvent;
		colorRect.TooltipText = ToolTip;

		Main.Ready += UpdateColor;
	}

	private void GuiInputEvent(InputEvent e)
	{
		if (e is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonMask != MouseButtonMask.Left)
				return;

			if (!Visible)
				Global.QuickPencils.SelectedType = Type;
			ShowColorInput();
		}
	}

	public void UpdateColor() => colorRect.Color = Main.Artist.GetQuickPencilColor(Type).GodotColor;

	public void SetBackground(Texture2D texture) => GetChild(0).GetChild<TextureRect>(0).Texture = texture;

	private void ShowColorInput()
	{
		GD.Print($"ShowColorInput: {Global.QuickPencils.GetColor()}");
		Global.FloatingColorInput.Show(GetGlobalMousePosition(), Global.QuickPencils.GetColor());
		Global.FloatingColorInput.ColorChanged += OnColorChanged;
		Global.FloatingColorInput.Closed += CleanupColorInput;
	}

	private void CleanupColorInput()
	{
		Global.FloatingColorInput.ColorChanged -= OnColorChanged;
		Global.FloatingColorInput.Closed -= CleanupColorInput;
	}

	private void OnColorChanged(Color color) => Global.QuickPencils.SetColor(color);
}
