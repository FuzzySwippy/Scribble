using System;
using Godot;
using Scribble.Application.MultiThreading;
using Scribble.Drawing;
using Scribble.UI;
using Scribble.UI.Info;

namespace Scribble.Application;

public partial class Global : Node
{
	private static Global current;

	#region Global values
	#region Main
	private Main main;
	public static Main Main
	{
		get => current.main;
		set => current.main ??= value;
	}

	private Settings settings;
	public static Settings Settings
	{
		get => current.settings;
		set => current.settings ??= value;
	}

	private ThreadManager threadManager;
	public static ThreadManager ThreadManager
	{
		get => current.threadManager;
		set => current.threadManager ??= value;
	}

	public static Random Random { get; } = new();
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

	private QuickInfo quickInfo;
	public static QuickInfo QuickInfo
	{
		get => current.quickInfo;
		set => current.quickInfo ??= value;
	}

	private Notifications notifications;
	public static Notifications Notifications
	{
		get => current.notifications;
		set => current.notifications ??= value;
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

	private FileDialogs fileDialogs;
	public static FileDialogs FileDialogs
	{
		get => current.fileDialogs;
		set => current.fileDialogs ??= value;
	}

	private DrawingToolPanel drawingToolPanel;
	public static DrawingToolPanel DrawingToolPanel
	{
		get => current.drawingToolPanel;
		set => current.drawingToolPanel ??= value;
	}

	private AnimationTimeline animationTimeline;
	public static AnimationTimeline AnimationTimeline
	{
		get => current.animationTimeline;
		set => current.animationTimeline ??= value;
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

	[ExportGroup("Application")]

	[Export] private string version;
	public static string Version => current.version;

	[ExportGroup("UI")]
	[Export] private LabelSettings labelSettings;
	public static LabelSettings LabelSettings => current.labelSettings;

	[Export] private LayerEditor layerEditor;
	public static LayerEditor LayerEditor => current.layerEditor;

	[Export] private HistoryList historyList;
	public static HistoryList HistoryList => current.historyList;

	[Export] private PackedScene layerListItemPrefab;
	public static PackedScene LayerListItemPrefab => current.layerListItemPrefab;

	[Export] private ColorRect interactionBlocker;
	public static ColorRect InteractionBlocker => current.interactionBlocker;

	[Export] private CanvasLayer dragCanvas;
	public static CanvasLayer DragCanvas => current.dragCanvas;

	[ExportSubgroup("Context Menu")]
	[Export] private PackedScene contextMenuButtonPrefab;
	public static PackedScene ContextMenuButtonPrefab => current.contextMenuButtonPrefab;

	[Export] private PackedScene contextMenuSeparatorPrefab;
	public static PackedScene ContextMenuSeparatorPrefab => current.contextMenuSeparatorPrefab;

	[ExportSubgroup("Icons")]
	[Export] private Texture2D addIconTexture;
	public static Texture2D AddIconTexture => current.addIconTexture;

	[Export] private Texture2D removeIconTexture;
	public static Texture2D RemoveIconTexture => current.removeIconTexture;

	[Export] private Texture2D lockIconTexture;
	public static Texture2D LockIconTexture => current.lockIconTexture;

	[Export] private Texture2D warningIconTexture;
	public static Texture2D WarningIconTexture => current.warningIconTexture;

	[Export] private Texture2D errorIconTexture;
	public static Texture2D ErrorIconTexture => current.errorIconTexture;


	[ExportGroup("Colors")]
	[Export] private StyleBoxTexture hueSliderStyleBox;
	public static StyleBoxTexture HueSliderStyleBox => current.hueSliderStyleBox;

	[Export] private StyleBoxTexture colorComponentStyleBox;
	public static StyleBoxTexture ColorComponentStyleBox => current.colorComponentStyleBox;

	[Export] private GradientTexture2D colorBoxGradientTexture;
	public static GradientTexture2D ColorBoxGradientTexture => current.colorBoxGradientTexture;

	[Export] private FloatingColorInput floatingColorInput;
	public static FloatingColorInput FloatingColorInput => current.floatingColorInput;


	[ExportGroup("Windows")]
	[Export] private PackedScene modalPrefab;
	public static PackedScene ModalPrefab => current.modalPrefab;


	[ExportGroup("Drawing")]
	[Export] private Canvas canvas;
	public static Canvas Canvas => current.canvas;

	[Export] private ShaderMaterial canvasChunkMaterial;
	public static ShaderMaterial CanvasChunkMaterial => current.canvasChunkMaterial;

	[Export] private PackedScene canvasChunkPrefab;
	public static PackedScene CanvasChunkPrefab => current.canvasChunkPrefab;

	[Export] private DrawingToolType defaultToolType;
	public static DrawingToolType DefaultToolType => current.defaultToolType;
	#endregion

	public override void _Ready()
	{
		current = this;

		GD.Print("Global ready");

		//Main
		settings = new Settings();
	}
}
