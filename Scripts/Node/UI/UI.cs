using Godot;
using ScribbleLib.Input;

namespace Scribble;

public partial class UI : Node
{
    public static float ContentScale { 
        get => Settings.UI.ContentScale; 
        private set
        {
            Vector2 zoomAmount = CameraController.ZoomAmount;
            Vector2 relativePosition = CameraController.RelativePosition;

            Settings.UI.ContentScale = value;
            if (Settings.UI.ContentScale < MinContentScale)
                Settings.UI.ContentScale = MinContentScale;
            else if (Settings.UI.ContentScale > MaxContentScale)
                Settings.UI.ContentScale = MaxContentScale;

            Main.Window.ContentScaleFactor = Settings.UI.ContentScale;

            CameraController.ZoomAmount = zoomAmount;
            CameraController.RelativePosition = relativePosition;
            
            DebugInfo.Set("ui_scale", Main.Window.ContentScaleFactor);
        }
    }
    public static float MaxContentScale { get; } = 2;
    public static float MinContentScale { get; } = 0.5f;
    public static float ContentScaleIncrement { get; } = 0.25f;

    public override void _Ready()
    {
        Main.Ready += () => ContentScale = Main.Window.ContentScaleFactor;
        Keyboard.KeyDown += KeyDown;

        HueSlider.GradientSetup();
        ShowAllCanvasLayers();
    }

    void KeyDown(KeyCombination combination)
    {
        //UI Scaling
        if (combination.modifiers == KeyModifierMask.MaskCtrl)
        {
            if (combination.key == Key.Equal)
                ContentScale += ContentScaleIncrement;

            if (combination.key == Key.Minus)
                ContentScale -= ContentScaleIncrement;
        }

        //Debug
        /*if (combination.key == Key.M)
            WindowManager.ShowModal("Test", new ModalButton[] 
            { 
                new("Yes", ModalButtonType.Confirm, () => GD.Print("Yes")),
                new("Maybe", ModalButtonType.Normal, () => GD.Print("Maybe")),
                new("No", ModalButtonType.Cancel, () => GD.Print("No")),
            });*/
    }

    void ShowAllCanvasLayers()
    {
        foreach (Node node in GetChildren())
            if (node is CanvasLayer layer)
                layer.Show();
    }
}