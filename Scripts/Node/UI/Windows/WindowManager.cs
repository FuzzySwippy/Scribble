using System.Collections.Generic;
using Godot;
using ScribbleLib.Input;

namespace Scribble;

public partial class WindowManager : CanvasLayer
{
    static WindowManager current;

    [Export] public float TransitionTime { get; set; }

    Dictionary<string, PanelWindow> windows;
    WindowVisualizer visualizer;

    public override void _Ready()
    {
        current = this;
        visualizer = new(this, TransitionTime);

        windows = new()
        {
            { Global.PalettesWindow.Name, Global.PalettesWindow }
        };

        //Debugging
        /*Keyboard.KeyDown += (KeyCombination c) =>
        {
            if (c.key == Key.S)
                Show("Palettes");
        };*/
    }

    public override void _Process(double delta) => visualizer.Update((float)delta);

    public static void Show(string windowName) => current.visualizer.Show(current.windows[windowName]);

    public static void Hide(PanelWindow window) => current.visualizer.Hide(window);
}
