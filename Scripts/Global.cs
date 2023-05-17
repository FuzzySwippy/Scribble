using Godot;

namespace Scribble;

public partial class Global : Node
{
    static Global current;

    #region Global values
    #region Main
    public static Settings Settings
    {
        get => current.settings;
        set => current.settings ??= value;
    }
    #endregion

    #region Debug
    public static DebugInfo DebugInfo
    {
        get => current.debugInfo;
        set => current.debugInfo ??= value;
    }
    #endregion

    #region Scene
    public static Camera2D Camera
    {
        get => current.camera;
        set => current.camera ??= value;
    }
    #endregion

    #region UI
    public static Status Status
    {
        get => current.status;
        set => current.status ??= value;
    }

    public static DrawingCanvas DrawingCanvas
    {
        get => current.drawingCanvas;
        set => current.drawingCanvas ??= value;
    }

    public static Spacer Spacer
    {
        get => current.spacer;
        set => current.spacer ??= value;
    }

    public static ContextMenu ContextMenu
    {
        get => current.contextMenu;
        set => current.contextMenu ??= value;
    }
    #endregion

    #region Color
    public static QuickPencils QuickPencils
    {
        get => current.quickPencils;
        set => current.quickPencils ??= value;
    }

    public static PalettePanel PalettePanel
    {
        get => current.palettePanel;
        set => current.palettePanel ??= value;
    }
    #endregion

    #region Windows
    public static WindowManager WindowManager
    {
        get => current.windowManager;
        set => current.windowManager ??= value;
    }
    #endregion
    #endregion


    #region Instance Values
    //Main
    Settings settings;

    //Debug
    DebugInfo debugInfo;

    //Scene
    Camera2D camera;

    //UI
    Status status;
    DrawingCanvas drawingCanvas;
    Spacer spacer;
    ContextMenu contextMenu;

    //Color
    QuickPencils quickPencils;
    PalettePanel palettePanel;

    //Windows
    WindowManager windowManager;
    #endregion

    #region Editor Values
    [ExportCategory("Global Values")]

    [ExportGroup("UI")]
    [Export] LabelSettings labelSettings;
    public static LabelSettings LabelSettings => current.labelSettings;

    [Export] StyleBoxTexture backgroundStyle;
    public static StyleBoxTexture BackgroundStyle => current.backgroundStyle;

    [ExportSubgroup("Icons")]
    [Export] Texture2D addIconTexture;
    public static Texture2D AddIconTexture => current.addIconTexture;

    [Export] Texture2D removeIconTexture;
    public static Texture2D RemoveIconTexture => current.removeIconTexture;

    [Export] Texture2D trashIconTexture;
    public static Texture2D TrashIconTexture => current.trashIconTexture;


    [ExportGroup("Colors")]
    [Export] ColorInput mainColorInput;
    public static ColorInput MainColorInput => current.mainColorInput;

    [Export] StyleBoxTexture hueSliderStyleBox;
    public static StyleBoxTexture HueSliderStyleBox => current.hueSliderStyleBox;

    [Export] StyleBoxTexture colorComponentStyleBox;
    public static StyleBoxTexture ColorComponentStyleBox => current.colorComponentStyleBox;

    [Export] GradientTexture2D colorBoxGradientTexture;
    public static GradientTexture2D ColorBoxGradientTexture => current.colorBoxGradientTexture;


    [ExportSubgroup("Windows")]
    [Export] PackedScene modalPrefab;
    public static PackedScene ModalPrefab => current.modalPrefab;
    #endregion

    public override void _Ready()
    {
        current = this;

        GD.Print("Global ready");

        //Main
        settings = FileManager.LoadSettings();
    }
}
