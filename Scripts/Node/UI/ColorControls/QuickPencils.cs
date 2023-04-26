using Godot;

namespace Scribble;

public partial class QuickPencils : Node
{
    ScribbleLib.ScribbleColor Color => Main.Artist.Brush.GetQuickPencilColor(SelectedType);

    QuickPencilSelector[] selectors = new QuickPencilSelector[4];
    QuickPencilType selectedType = QuickPencilType.Primary;
    public QuickPencilType SelectedType
    {
        get => selectedType;
        set
        {
            if (selectedType == value)
                return;

            selectedType = value;
            UpdateSelectorVisibility();
            Global.MainColorInput.Color = Color;
            UpdateSelectorColor();
        }
    }

    public override void _Ready() => Main.Ready += MainReady;

    void MainReady()
    {
        GetSelectors();
        SetSelectorBackgroundTextures();
        UpdateSelectorVisibility();

        Global.MainColorInput.ColorUpdated += ColorUpdated;
        Global.MainColorInput.Color = Color;
    }

    void ColorUpdated()
    {
        Main.Artist.Brush.GetQuickPencilColor(SelectedType).CloneFrom(Global.MainColorInput.Color);
        UpdateSelectorColor();
    }

    void GetSelectors()
    {
        foreach (Node child in GetChildren())
            if (child is QuickPencilSelector selector)
                selectors[(int)selector.Type] = selector;
    }

    void SetSelectorBackgroundTextures()
    {
        Texture2D texture = TextureGenerator.NewBackgroundTexture(new(7, 7));
        for (int i = 0; i < selectors.Length; i++)
            selectors[i].SetBackground(texture);
    }

    void UpdateSelectorVisibility()
    {
        for (int i = 0; i < selectors.Length; i++)
            selectors[i].Visible = selectors[i].Type == SelectedType;
    }

    public void UpdateSelectorColor() => selectors[(int)SelectedType].UpdateColor();
}
