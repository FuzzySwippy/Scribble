using System.Collections.Generic;
using System;
using Godot;

namespace ScribbleLib.Input;
public class Mouse
{
    public delegate void MouseButtonEvent(MouseButton button, Vector2 position);


    static Mouse current;

    public static Vector2 Position { get; private set; }

    //Dragging
    public static float DragVelocityThreshold { get; set; } = 50;

    public static event MouseButtonEvent DragStart;
    public static event MouseButtonEvent DragEnd;

    //Button actuation
    public static event MouseButtonEvent ButtonDown;
    public static event MouseButtonEvent ButtonUp;

    //Button presses
    Dictionary<MouseButton, bool> mouseButtonIsPressed = new();
    Dictionary<MouseButton, bool> mouseButtonIsDragging = new();

    //Scrolling
    public delegate void MouseScrollEvent(int delta);
    public static event MouseScrollEvent Scroll;

    public Mouse()
    {
        current = this;

        //Fill the button dictionary with values
        foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
        {
            mouseButtonIsPressed.Add(button, false);
            mouseButtonIsDragging.Add(button, false);
        }
    }

    //Internal
    public void HandleButton(MouseButton button, bool pressed, Vector2 position)
    {
        //Actuation
        if (mouseButtonIsPressed[button] != pressed)
        {
            if (pressed)
                ButtonDown?.Invoke(button, position);
            else
                ButtonUp?.Invoke(button, position);
        }

        //Scrolling
        if (button == MouseButton.WheelUp)
            Scroll?.Invoke(1);
        else if (button == MouseButton.WheelDown)
            Scroll?.Invoke(-1);

        //Presses
        mouseButtonIsPressed[button] = pressed;

        //End drag
        if (!pressed && mouseButtonIsDragging[button])
        {
            mouseButtonIsDragging[button] = false;
            DragEnd?.Invoke(button, position);
        }
    }

    public void HandleMotion(MouseButtonMask buttons, Vector2 position, Vector2 velocity)
    {
        Position = position;

        if (buttons != 0 && velocity.Length() > DragVelocityThreshold)
        {
            MouseButton button = MouseButton.None;
            if (buttons.HasFlag(MouseButtonMask.Left) && mouseButtonIsPressed[MouseButton.Left] && !mouseButtonIsDragging[MouseButton.Left])
                button = MouseButton.Left;
            else if (buttons.HasFlag(MouseButtonMask.Middle) && mouseButtonIsPressed[MouseButton.Middle] && !mouseButtonIsDragging[MouseButton.Middle])
                button = MouseButton.Middle;
            else if (buttons.HasFlag(MouseButtonMask.Right) && mouseButtonIsPressed[MouseButton.Right] && !mouseButtonIsDragging[MouseButton.Right])
                button = MouseButton.Right;

            if (button != MouseButton.None)
            {
                mouseButtonIsDragging[button] = true;
                DragStart?.Invoke(button, position);
            }
        }
    }

    //Static
    public static bool IsPressed(MouseButton button)
    {
        if (!current.mouseButtonIsPressed.ContainsKey(button))
            return false;
        return current.mouseButtonIsPressed[button];
    }
}
