using System.Threading.Tasks;
using Godot;
using ProcessMode = Godot.Node.ProcessModeEnum;

namespace Scribble;

public class WindowVisualizer
{
    public enum TransitionState
    { 
        None,
        Show,
        Hide
    }

    public static float OriginMargin { get; } = 100;

    TransitionState state;
    TransitionState State
    {
        get => state;
        set
        {
            state = value;
            manager.Visible = state != TransitionState.None || transitionValue > 0;

            if (!manager.Visible)
                activeWindow = null;

            manager.ProcessMode = state != TransitionState.None || manager.Visible ? ProcessMode.Inherit : ProcessMode.Disabled;
        }
    }

    float transitionTime;
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
                State = TransitionState.None;
            }
            else if (transitionValue < 0)
            {
                transitionValue = 0;
                State = TransitionState.None;
            }

            fade.Modulate = new(1, 1, 1, transitionValue);
            if (activeWindow != null)
                activeWindow.Position = windowOriginPosition.Lerp(windowTargetPosition, transitionValue);
        }
    }

    WindowManager manager;
    TextureRect fade;
    PanelWindow activeWindow;
    Vector2 windowOriginPosition, windowTargetPosition;

    public WindowVisualizer(WindowManager manager, float transitionTime)
    {
        this.manager = manager;
        this.transitionTime = transitionTime;

        fade = manager.GetChild<TextureRect>(0);
        manager.Visible = false;
    }

    public void Update(float deltaTime)
    {
        switch (State)
        {
            case TransitionState.Show:
                TransitionValue += (float)deltaTime / transitionTime;
                break;
            case TransitionState.Hide:
                TransitionValue -= (float)deltaTime / transitionTime;
                break;
        }
    }

    public void Show(PanelWindow window) => Task.Run(() => ShowAsync(window));

    async Task ShowAsync(PanelWindow window)
    {
        if (activeWindow != window)
        {
            //Hide the currently active window
            if (activeWindow != null)
                Hide(activeWindow);

            while (activeWindow != null)
                await Task.Delay(10);

            //Show the new window
            activeWindow = window;
            windowOriginPosition = window.OriginPosition;
            windowTargetPosition = window.TargetPosition;
            window.Position = windowOriginPosition;
        }

        State = TransitionState.Show;
    }

    public void Hide(PanelWindow window)
    {
        if (activeWindow == window)
            State = TransitionState.Hide;
    }
}
