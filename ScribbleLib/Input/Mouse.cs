using System.Collections.Generic;
using System;
using Godot;

namespace Scribble.ScribbleLib.Input;
public class Mouse
{
	public delegate void MouseButtonEvent(MouseCombination combination, Vector2 position);
	public delegate void MouseDragEvent(MouseCombination combination, Vector2 position, Vector2 positionChange, Vector2 velocity);
	public delegate void MouseScrollEvent(KeyModifierMask modifiers, int delta);

	private static Mouse current;

	public static Vector2 Position { get; private set; }
	public static Vector2 GlobalPosition { get; private set; }

	//Dragging
	public static float DragVelocityThreshold { get; set; } = 10;

	public static event MouseDragEvent DragStart;
	public static event MouseDragEvent DragEnd;
	public static event MouseDragEvent Drag;

	//Button actuation
	public static event MouseButtonEvent ButtonDown;
	public static event MouseButtonEvent ButtonUp;

	//Scrolling
	public static event MouseScrollEvent Scroll;

	//Button presses
	private readonly Dictionary<MouseButton, bool> mouseButtonIsPressed = new();
	private readonly Dictionary<MouseButton, KeyModifierMask> mouseButtonPressModifiers = new();
	private readonly Dictionary<MouseButton, bool> mouseButtonIsDragging = new();
	private readonly Dictionary<MouseButton, KeyModifierMask> mouseButtonDragModifiers = new();
	private Vector2 lastDragPosition;

	//Warp
	[Obsolete("Broken in Wayland", true)]
	public static Rect2 WarpBorder { get; set; } = new();

	//Objects
	private Viewport Viewport { get; }

	public Mouse(Viewport viewport)
	{
		current = this;
		Viewport = viewport;

		//Fill the button dictionary with values
		foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
		{
			mouseButtonIsPressed.Add(button, false);
			mouseButtonIsDragging.Add(button, false);
			mouseButtonPressModifiers.Add(button, 0);
			mouseButtonDragModifiers.Add(button, 0);
		}
	}

	//Internal
	public void HandleButton(MouseCombination combination, bool pressed, Vector2 position)
	{
		//Actuation events
		if (mouseButtonIsPressed[combination.button] != pressed) //State change
			(pressed ? ButtonDown : ButtonUp)?.Invoke(combination, position);
		else if (mouseButtonIsPressed[combination.button] && mouseButtonPressModifiers[combination.button] != 0 && mouseButtonPressModifiers[combination.button] != combination.modifiers)
		{
			//Update pressed modifiers
			ButtonUp?.Invoke(new(combination.button, mouseButtonPressModifiers[combination.button]), position);
			mouseButtonPressModifiers[combination.button] = combination.modifiers;
			ButtonDown?.Invoke(combination, position);
		}

		//Set button states
		mouseButtonIsPressed[combination.button] = pressed;
		mouseButtonPressModifiers[combination.button] = combination.modifiers;

		//Scrolling
		if (pressed)
		{
			if (combination.button == MouseButton.WheelUp)
				Scroll?.Invoke(combination.modifiers, 1);
			else if (combination.button == MouseButton.WheelDown)
				Scroll?.Invoke(combination.modifiers, -1);
		}

		//End drag
		if (mouseButtonIsDragging[combination.button] && (!pressed || mouseButtonDragModifiers[combination.button] != combination.modifiers))
			EndDrag(combination.button, position);
	}

	private void EndDrag(MouseButton button, Vector2 position)
	{
		mouseButtonIsDragging[button] = false;
		DragEnd?.Invoke(new(button, mouseButtonDragModifiers[button]), position, Vector2.Zero, Vector2.Zero);
		mouseButtonDragModifiers[button] = 0;
	}

	public void HandleMotion(MouseButtonMask buttons, KeyModifierMask modifiers, Vector2 position, Vector2 globalPosition, Vector2 velocity)
	{
		Position = position;
		GlobalPosition = globalPosition;

		//Update button modifiers
		foreach (MouseButton button in mouseButtonIsPressed.Keys)
		{
			if (mouseButtonIsPressed[button] && mouseButtonPressModifiers[button] != modifiers)
			{
				ButtonUp?.Invoke(new(button, mouseButtonPressModifiers[button]), position);
				mouseButtonPressModifiers[button] = modifiers;
				ButtonDown?.Invoke(new(button, modifiers), position);
			}
		}

		//Start drag
		if (buttons != 0 && velocity.Length() > DragVelocityThreshold)
		{
			MouseButton button = MouseButton.None;
			if (buttons.HasFlag(MouseButtonMask.Left) && mouseButtonIsPressed[MouseButton.Left] && !mouseButtonIsDragging[MouseButton.Left])
				button = MouseButton.Left;
			else if (buttons.HasFlag(MouseButtonMask.Middle) && mouseButtonIsPressed[MouseButton.Middle] && !mouseButtonIsDragging[MouseButton.Middle])
				button = MouseButton.Middle;
			else if (buttons.HasFlag(MouseButtonMask.Right) && mouseButtonIsPressed[MouseButton.Right] && !mouseButtonIsDragging[MouseButton.Right])
				button = MouseButton.Right;

			//Start drag
			if (mouseButtonIsDragging[button] && mouseButtonDragModifiers[button] != modifiers)
				EndDrag(button, position);

			if (button != MouseButton.None)
			{
				mouseButtonIsDragging[button] = true;
				mouseButtonDragModifiers[button] = modifiers;
				lastDragPosition = position;
				DragStart?.Invoke(new(button, modifiers), position, position - lastDragPosition, velocity);
			}
		}

		//Calls drag event for the pressed buttons
		foreach (MouseButton button in mouseButtonIsDragging.Keys)
			if (mouseButtonIsDragging[button])
				Drag?.Invoke(new(button, modifiers), position, position - lastDragPosition, velocity);
		lastDragPosition = position;

		//Warp border
		/*if (WarpBorder.HasArea())
		{
			if (position.X < WarpBorder.Position.X)
				WarpMouse(new Vector2(position.X + WarpBorder.Size.X, position.Y));

			if (position.X > WarpBorder.End.X)
				WarpMouse(new Vector2(position.X - WarpBorder.Size.X, position.Y));

			if (position.Y < WarpBorder.Position.Y)
				WarpMouse(new Vector2(position.X, position.Y + WarpBorder.Size.Y));

			if (position.Y > WarpBorder.End.Y)
				WarpMouse(new Vector2(position.X, position.Y - WarpBorder.Size.Y));
		}*/
	}

	/*private void WarpMouse(Vector2 newPosition)
	{
		Viewport.WarpMouse(newPosition);
		lastDragPosition = newPosition;
	}*/

	//Static
	public static bool IsPressed(MouseButton button) => current.mouseButtonIsPressed[button] && current.mouseButtonPressModifiers[button] == 0;

	public static bool IsPressed(MouseCombination combination) => current.mouseButtonIsPressed[combination.button] && current.mouseButtonPressModifiers[combination.button] == combination.modifiers;
}

public readonly struct MouseCombination
{
	public readonly MouseButton button;
	public readonly KeyModifierMask modifiers;

	public bool HasModifiers => modifiers != 0;

	public MouseCombination(MouseButton button)
	{
		this.button = button;
		modifiers = 0;
	}

	public MouseCombination(MouseButton button, KeyModifierMask modifiers)
	{
		this.button = button;
		this.modifiers = modifiers;
	}

	public override string ToString() => HasModifiers ? $"({modifiers} - {button})" : button.ToString();

	public override bool Equals(object obj) =>
		obj is MouseCombination combination && combination.button == button && combination.modifiers == modifiers;

	public override int GetHashCode() => (button, modifiers).GetHashCode();

	public static bool operator ==(MouseCombination left, MouseCombination right) => left.Equals(right);

	public static bool operator !=(MouseCombination left, MouseCombination right) => !left.Equals(right);
}
