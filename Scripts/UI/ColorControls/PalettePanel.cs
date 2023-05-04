using Godot;

namespace Scribble;

public partial class PalettePanel : Node
{
    Label emptyPaletteLabel;
    Button editButton;
    OptionButton paletteSelectionDropdown;
    readonly PaletteColorSelector[] selectors = new PaletteColorSelector[Palette.MaxColors];
    int selectedColorIndex = -1;
    bool ignoreColorUpdate;

    static Palette Palette => new Palette("e", new Color?[]
    {
        new (1, 0, 0, 1),
        new (0, 1, 0, 1),
        new (0, 0, 1, 1),
        null,
        new (1, 1, 0, 0),
        new (1, 0, 1, 1)
    });//Main.Artist.CurrentPalette;


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

        Global.MainColorInput.ColorUpdated += ColorUpdated;
    }

    void GenerateColorSelectors()
    {
        Texture2D backgroundTexture = TextureGenerator.NewBackgroundTexture(new(5, 5));
        Node selectorParent = GetChild(0).GetChild(0).GetChild(1).GetChild(0);
        Control baseColorSelector = selectorParent.GetChild<Control>(0);

        for (int i = 0; i < Palette.MaxColors; i++)
        {
            if (i == 0)
                selectors[i] = new(baseColorSelector);
            else
            {
                selectors[i] = new((Control)baseColorSelector.Duplicate());
                selectorParent.AddChild(selectors[i].Control);
            }

            int index = i;
            selectors[i].Button.Pressed += () => Select(index);
            selectors[i].Button.GetChild<TextureRect>(0).Texture = backgroundTexture;
        }
    }

    void ColorUpdated()
    {
        if(ignoreColorUpdate)
        {
            ignoreColorUpdate = false;
            return;
        }
        Deselect();
    }

    public void Select(int index)
    {
        if (Palette == null || !Palette.GetColor(index, out Color? color))
        {
            Deselect();
            return;
        }

        selectedColorIndex = index;
        ignoreColorUpdate = true;
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
                selectors[i].Show();
                emptyPalette = false;
                continue;
            }
            selectors[i].Hide();
        }

        paletteSelectionDropdown.Disabled = emptyPalette;
        emptyPaletteLabel.Visible = emptyPalette;
    }
}

public class PaletteColorSelector
{
    public Control Control { get; }
    public Button Button { get; }
    public ColorRect ColorRect { get; }
    public Control SelectionIndicator { get; }

    public PaletteColorSelector(Control control)
    {
        Control = control;
        Button = control.GetChild<Button>(0);
        ColorRect = Button.GetChild<ColorRect>(1);
        SelectionIndicator = Button.GetChild(1).GetChild<Control>(0);
    }

    public void Show() => Button.Show();
    public void Hide() => Button.Hide();
}