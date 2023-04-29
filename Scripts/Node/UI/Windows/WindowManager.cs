using System.Collections.Generic;
using Godot;

namespace Scribble;

public partial class WindowManager : Control
{
    [Export] public float TransitionTime { get; set; }

    CanvasLayer canvasLayer;
    public static bool CanvasLayerVisible
    { 
        get => Global.WindowManager.canvasLayer.Visible;
        set => Global.WindowManager.canvasLayer.Visible = value;
    }

    Dictionary<string, Window> windows;

    public override void _Ready()
    {
        GD.Print("WindowManager Ready");
        canvasLayer = GetParent<CanvasLayer>();
        RegisterWindows();
    }

    public void RegisterWindows()
    {
        windows = new();
        foreach (Node node in GetChildren())
            if (node is Window window && !string.IsNullOrWhiteSpace(window.KeyName))
                windows.Add(window.KeyName, window);
    }

    public static Window Get(string name) => Global.WindowManager.windows.TryGetValue(name, out Window window) ? window : null;
    public static Window Show(string name) => Get(name)?.Show();
}
