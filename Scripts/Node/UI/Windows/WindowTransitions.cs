using System;
using Godot;

namespace Scribble;

public class WindowTransitions
{
    public enum VisibilityState
    {
        InTransition,
        Visible,
        Hidden
    }

    VisibilityState state;
    VisibilityState State
    { 
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                UpdateProcessingMode();
            }
        }
    }
    bool toVisible;

    static float TransitionTime => Global.WindowManager.TransitionTime;

    float transitionValue = 0;
    float TransitionValue
    {
        get => transitionValue;
        set
        {
            transitionValue = value;

            if (transitionValue > 1)
            {
                transitionValue = 1;
                State = VisibilityState.Visible;
            }
            else if (transitionValue < 0)
            {
                transitionValue = 0;
                State = VisibilityState.Hidden;
            }

            fade.Modulate = new(1, 1, 1, transitionValue);
            panel.Position = window.PanelStartPosition.Lerp(window.PanelTargetPosition, transitionValue);
        }
    }

    readonly Window window;
    readonly TextureRect fade;
    readonly Control panel;

    public WindowTransitions(Window window)
    {
        this.window = window;
        fade = window.GetChild<TextureRect>(0);
        panel = window.GetChild<Control>(1);
    }

    public void Update(float deltaTime) => TransitionValue += deltaTime / TransitionTime * (toVisible ? 1 : -1);

    public Window Show()
    {
        panel.Position = window.PanelStartPosition;
        toVisible = true;
        State = VisibilityState.InTransition;
        return window;
    }

    public void Hide()
    { 
        panel.Position = window.PanelTargetPosition;
        toVisible = false;
        State = VisibilityState.InTransition;
    }

    public void InstaHide()
    {
        toVisible = false;
        TransitionValue = 0;
    }

    void UpdateProcessingMode()
    {
        switch (State)
        {
            case VisibilityState.Visible:
            case VisibilityState.InTransition:
                window.ProcessMode = Node.ProcessModeEnum.Inherit;
                panel.ProcessMode = Node.ProcessModeEnum.Inherit;
                WindowManager.CanvasLayerVisible = true;
                break;
            case VisibilityState.Hidden:
                window.ProcessMode = Node.ProcessModeEnum.Disabled;
                panel.ProcessMode = Node.ProcessModeEnum.Inherit;
                WindowManager.CanvasLayerVisible = false;
                break;
        }
    }
}
