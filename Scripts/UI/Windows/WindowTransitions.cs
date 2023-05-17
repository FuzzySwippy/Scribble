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

	float transitionValue;
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
				Window.Visible = false;

				Hidden?.Invoke();
			}

			if (fade != null)
				fade.Modulate = new(1, 1, 1, transitionValue);
			Panel.Position = Window.PanelStartPosition.Lerp(Window.PanelTargetPosition, transitionValue);
		}
	}

	Window Window { get; }
	readonly TextureRect fade;
	Panel Panel => Window.Panel;

	public event Action Hidden;

	public WindowTransitions(Window window)
	{
		Window = window;
		if (Window.WindowType != Window.Type.Popup && Window.ShowFadeBackground)
			fade = window.GetChild<TextureRect>(0);
	}

	public void Update(float deltaTime) => TransitionValue += deltaTime / TransitionTime * (toVisible ? 1 : -1);

	public void Show()
	{
		Panel.Position = Window.PanelStartPosition;
		Window.Visible = true;
		toVisible = true;
		State = VisibilityState.InTransition;
	}

	public void Hide()
	{
		Panel.Position = Window.PanelTargetPosition;
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
				Window.ProcessMode = Node.ProcessModeEnum.Inherit;
				Panel.ProcessMode = Node.ProcessModeEnum.Inherit;
				break;
			case VisibilityState.Hidden:
				Window.ProcessMode = Node.ProcessModeEnum.Disabled;
				Panel.ProcessMode = Node.ProcessModeEnum.Inherit;
				break;
		}
	}
}
