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
    [Export] ColorController colorController;
    public static ColorController ColorController { get => current.colorController; }

    [Export] ColorBox colorBox;
    public static ColorBox ColorBox { get => current.colorBox; }

    [Export] HueSlider hueSlider;
    public static HueSlider HueSlider { get => current.hueSlider; }

    [Export] GradientTexture2D hueSliderTexture;
    public static GradientTexture2D HueSliderTexture { get => current.hueSliderTexture; }

    

    


    public override void _Ready() => current = this;
}
