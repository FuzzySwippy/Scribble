using Godot;
using Scribble.Drawing.Visualization;

namespace Scribble.Application;

public partial class Global : Node
{
	private static Global current;

	#region Global values
	#region Main
	private Settings settings;
	public static Settings Settings
	{
		get => current.settings;
		set => current.settings ??= value;
	}
	#endregion

	#region Debug
	private DebugInfo debugInfo;

	public static DebugInfo DebugInfo
	{
		get => current.debugInfo;
		set => current.debugInfo ??= value;
	}
	#endregion

	#region Scene
	private Camera2D camera;
	public static Camera2D Camera
	{
		get => current.camera;
		set => current.camera ??= value;
	}
	#endregion

	#region UI
	private Status status;
	public static Status Status
	{
		get => current.status;
		set => current.status ??= value;
	}

	private Spacer spacer;
	public static Spacer Spacer
	{
		get => current.spacer;
		set => current.spacer ??= value;
	}

	private ContextMenu contextMenu;
	public static ContextMenu ContextMenu
	{
		get => current.contextMenu;
		set => current.contextMenu ??= value;
	}
	#endregion

	#region Drawing
	private Canvas canvas;
	public static Canvas Canvas
	{
		get => current.canvas;
		set => current.canvas ??= value;
	}
	#endregion

	#region Color
	private QuickPencils quickPencils;
	public static QuickPencils QuickPencils
	{
		get => current.quickPencils;
		set => current.quickPencils ??= value;
	}

	private PalettePanel palettePanel;
	public static PalettePanel PalettePanel
	{
		get => current.palettePanel;
		set => current.palettePanel ??= value;
	}
	#endregion

	#region Windows
	private WindowManager windowManager;
	public static WindowManager WindowManager
	{
		get => current.windowManager;
		set => current.windowManager ??= value;
	}
	#endregion
	#endregion

	#region Editor Values
	[ExportCategory("Global Values")]

	[ExportGroup("UI")]
	[Export] private LabelSettings labelSettings;
	public static LabelSettings LabelSettings => current.labelSettings;

	[Export] private StyleBoxTexture backgroundStyle;
	public static StyleBoxTexture BackgroundStyle => current.backgroundStyle;

	[ExportSubgroup("Icons")]
	[Export] private Texture2D addIconTexture;
	public static Texture2D AddIconTexture => current.addIconTexture;

	[Export] private Texture2D removeIconTexture;
	public static Texture2D RemoveIconTexture => current.removeIconTexture;

	[Export] private Texture2D lockIconTexture;
	public static Texture2D LockIconTexture => current.lockIconTexture;


	[ExportGroup("Colors")]
	[Export] private ColorInput mainColorInput;
	public static ColorInput MainColorInput => current.mainColorInput;

	[Export] private StyleBoxTexture hueSliderStyleBox;
	public static StyleBoxTexture HueSliderStyleBox => current.hueSliderStyleBox;

	[Export] private StyleBoxTexture colorComponentStyleBox;
	public static StyleBoxTexture ColorComponentStyleBox => current.colorComponentStyleBox;

	[Export] private GradientTexture2D colorBoxGradientTexture;
	public static GradientTexture2D ColorBoxGradientTexture => current.colorBoxGradientTexture;


	[ExportSubgroup("Windows")]
	[Export] private PackedScene modalPrefab;
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
