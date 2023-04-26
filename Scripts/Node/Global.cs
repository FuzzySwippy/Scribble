using Godot;
using System;
using System.Transactions;

namespace Scribble;

public partial class Global : Node
{
    static Global current;

    [ExportCategory("Global Values")]


    [ExportGroup("Debug")]
    [Export] DebugInfo debugInfo;
    public static DebugInfo DebugInfo { get => current.debugInfo; }


    [ExportGroup("Scene")]
    [Export] Camera2D camera;
    public static Camera2D Camera { get => current.camera; }


    [ExportGroup("UI")]
    [Export] Status status;
    public static Status Status { get => current.status; }
    
    [Export] LabelSettings labelSettings;
    public static LabelSettings LabelSettings { get => current.labelSettings; }


    [ExportGroup("Canvas")]
    [Export] Node2D canvasNode;
    public static Node2D CanvasNode { get => current.canvasNode; }

    [Export] Control spacer;
    public static Control Spacer { get => current.spacer; }

    [Export] StyleBoxTexture backgroundStyle;
    public static StyleBoxTexture BackgroundStyle { get => current.backgroundStyle; }

    [ExportGroup("Colors")]
    [Export] ColorInput mainColorInput;
    public static ColorInput MainColorInput { get => current.mainColorInput; }

    [Export] QuickPencils quickPencils;
    public static QuickPencils QuickPencils { get => current.quickPencils; }

    [Export] StyleBoxTexture hueSliderStyleBox;
    public static StyleBoxTexture HueSliderStyleBox { get => current.hueSliderStyleBox; }

    [Export] StyleBoxTexture colorComponentStyleBox;
    public static StyleBoxTexture ColorComponentStyleBox { get => current.colorComponentStyleBox; }



    [ExportSubgroup("Windows")]

    [Export] PanelWindow palettesWindow;
    public static PanelWindow PalettesWindow { get => current.palettesWindow; }



    public override void _Ready() => current = this;
}
