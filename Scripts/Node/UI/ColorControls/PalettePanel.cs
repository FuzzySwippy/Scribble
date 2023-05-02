using Godot;

namespace Scribble;

public partial class PalettePanel : Node
{
    Label emptyPaletteLabel;
    Button editButton;
    OptionButton paletteSelectionDropdown;
    readonly PaletteColorSelector[] selectors = new PaletteColorSelector[Palette.MaxColors];
    int selectedColorIndex = -1;

    static Palette Palette => Main.Artist.CurrentPalette;


    public override void _Ready()
    {
        emptyPaletteLabel = GetChild(0).GetChild(0).GetChild(1).GetChild<Label>(1);
        paletteSelectionDropdown = GetChild(0).GetChild(0).GetChild(2).GetChild<OptionButton>(0);
        editButton = GetChild(0).GetChild(0).GetChild(2).GetChild<Button>(1);
        editButton.Pressed += () => WindowManager.Show("palettes");

        Main.Ready += MainReady;
    }

    void MainReady()
    {
        GenerateColorSelectors();
        UpdatedSelectors();

        Global.MainColorInput.ColorUpdated += Deselect;
    }

    void GenerateColorSelectors()
    {
        Texture2D backgroundTexture = TextureGenerator.NewBackgroundTexture(new(7, 7));
        Node selectorParent = GetChild(0).GetChild(0).GetChild(1).GetChild(0);
        Node baseColorSelector = selectorParent.GetChild(0);

        for (int i = 0; i < Palette.MaxColors; i++)
        {
            if (i == 0)
            {
                baseColorSelector = selectorParent.GetChild(0);
                selectors[i] = new((Button)baseColorSelector);
            }
            else
            {
                selectors[i] = new((Button)baseColorSelector.Duplicate());
                selectorParent.AddChild(selectors[i].Button);
            }

            int index = i;
            selectors[i].Button.Pressed += () => ColorSelectorPressed(index);
            selectors[i].Button.GetChild<TextureRect>(0).Texture = backgroundTexture;
        }
    }

    void ColorSelectorPressed(int index)
    {
        Select(index);
        GD.Print($"Color selector pressed: {index}");
    }

    public void Select(int index)
    {
        if (Palette == null || Palette.Colors[index].HasValue)
        {
            Deselect();
            return;
        }

        selectedColorIndex = index;
        Global.MainColorInput.SetColorFromGodotColor(Palette.Colors[index].Value);
        UpdatedSelectorIndicators();
    }

    public void Deselect()
    {
        selectedColorIndex = -1;
        UpdatedSelectorIndicators();
    }

    void UpdatedSelectorIndicators()
    {
        for (int i = 0; i < Palette.MaxColors; i++)
        {
            if (i == selectedColorIndex)
            {
                selectors[i].SelectionIndicator.Show();
                continue;
            }
            selectors[i].SelectionIndicator.Hide();
        }
    }

    void UpdatedSelectors()
    {
        bool emptyPalette = true;
        for (int i = 0; i < Palette.MaxColors; i++)
        {
            selectors[i].SelectionIndicator.Hide();

            if (Palette != null && Palette.Colors[i].HasValue)
            {
                //If the color hasn't changed, keep showing the selection indicator
                if (selectors[i].ColorRect.Color == Palette.Colors[i].Value)
                    selectors[i].SelectionIndicator.Show();

                selectors[i].ColorRect.Color = Palette.Colors[i].Value;
                selectors[i].Button.Show();
                emptyPalette = false;
                continue;
            }
            selectors[i].Button.Hide();
        }

        paletteSelectionDropdown.Disabled = emptyPalette;
        emptyPaletteLabel.Visible = emptyPalette;
    }
}

public class PaletteColorSelector
{
    public Button Button { get; set; }
    public ColorRect ColorRect { get; set; }
    public Control SelectionIndicator { get; set; }

    public PaletteColorSelector(Button selector)
    {
        Button = selector;
        ColorRect = selector.GetChild<ColorRect>(1);
        SelectionIndicator = selector.GetChild(1).GetChild<Control>(0);
    }
}