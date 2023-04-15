using Godot;

namespace Scribble;

public partial class HexInput : Node
{
    LineEdit input;
    Label label;

	bool ignoreUpdate = false;
    bool ignoreVisualizationUpdate = false;

    [Export] Color invalidColor;

    public Color? Color { get; private set; }


    public override void _Ready()
    {
        label = GetChild<Label>(0);
        input = GetChild<LineEdit>(1);

		input.TextChanged += InputTextChanged;
    }

    void InputTextChanged(string newText)
    {
        if (ignoreUpdate)
        { 
			ignoreUpdate = false;
			return;
		}

        try
        {
            Color = Godot.Color.FromHtml(newText);

            ignoreVisualizationUpdate = true;
            Global.ColorController.SetColorFromHexInput();

            input.RemoveThemeColorOverride("font_color");
        }
        catch (System.Exception)
        {
            Color = null;
            input.AddThemeColorOverride("font_color", invalidColor);
        }
    }

    public void UpdateVisualizations()
    {
        if (ignoreVisualizationUpdate)
        { 
            ignoreVisualizationUpdate = false;
            return;
        }

        ignoreUpdate = true;
        input.Text = $"#{Global.ColorController.Color.Color.ToHtml()}";
    }
}
