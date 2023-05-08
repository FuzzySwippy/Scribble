using Godot;

namespace Scribble;

public partial class Global : Node
{
    static Global current;

    #region Global values
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

    //Windows
    WindowManager windowManager;
    #endregion

    #region Editor Values
    [ExportCategory("Global Values")]

    [ExportGroup("UI")]
    [Export] LabelSettings labelSettings;
    public static LabelSettings LabelSettings { get => current.labelSettings; }

    [Export] StyleBoxTexture backgroundStyle;
    public static StyleBoxTexture BackgroundStyle { get => current.backgroundStyle; }

    [ExportSubgroup("Icons")]
    [Export] Texture2D addIconTexture;
    public static Texture2D AddIconTexture { get => current.addIconTexture; }

    [Export] Texture2D removeIconTexture;
    public static Texture2D RemoveIconTexture { get => current.removeIconTexture; }

    [Export] Texture2D trashIconTexture;
    public static Texture2D TrashIconTexture { get => current.trashIconTexture; }


    [ExportGroup("Colors")]
    [Export] ColorInput mainColorInput;
    public static ColorInput MainColorInput { get => current.mainColorInput; }

    [Export] StyleBoxTexture hueSliderStyleBox;
    public static StyleBoxTexture HueSliderStyleBox { get => current.hueSliderStyleBox; }

    [Export] StyleBoxTexture colorComponentStyleBox;
    public static StyleBoxTexture ColorComponentStyleBox { get => current.colorComponentStyleBox; }

    [Export] GradientTexture2D colorBoxGradientTexture;
    public static GradientTexture2D ColorBoxGradientTexture { get => current.colorBoxGradientTexture; }


    [ExportSubgroup("Windows")]
    [Export] PackedScene modalPrefab;
    public static PackedScene ModalPrefab { get => current.modalPrefab; }
    #endregion


    public override void _Ready() => current = this;
}
