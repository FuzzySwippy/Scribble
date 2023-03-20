using System.Collections.Generic;
using System;
using Godot;

namespace ScribbleLib.Input;
public class Mouse
{
    public delegate void MouseButtonEvent(MouseButton button, Vector2 position);
    public delegate void MouseDragEvent(MouseButton button, Vector2 position, Vector2 positionChange, Vector2 velocity);


    static Mouse current;

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
    public delegate void MouseScrollEvent(int delta);
    public static event MouseScrollEvent Scroll;

    //Button presses
    Dictionary<MouseButton, bool> mouseButtonIsPressed = new();
    Dictionary<MouseButton, bool> mouseButtonIsDragging = new();
    Vector2 lastDragPosition;

    //Warp
    public static Rect2 WarpBorder { get; set; } = new();

    //Objects
    Viewport Viewport { get; }

    public Mouse(Viewport viewport)
    {
        current = this;
        Viewport = viewport;

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
            DragEnd?.Invoke(button, position, Vector2.Zero, Vector2.Zero);
        }
    }

    public void HandleMotion(MouseButtonMask buttons, Vector2 position, Vector2 globalPosition, Vector2 velocity)
    {
        Position = position;
        GlobalPosition = globalPosition;

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
            if (button != MouseButton.None)
            {
                mouseButtonIsDragging[button] = true;
                lastDragPosition = position;
                DragStart?.Invoke(button, position, position - lastDragPosition, velocity);
            }
        }

        //Calls drag event for the pressed buttons
        foreach (MouseButton button in mouseButtonIsDragging.Keys)
            if (mouseButtonIsDragging[button])
                Drag?.Invoke(button, position, position - lastDragPosition, velocity);
        lastDragPosition = position;

        //Warp border
        if (WarpBorder.HasArea())
        {
            if (position.X < WarpBorder.Position.X)
                WarpMouse(new Vector2(position.X + WarpBorder.Size.X, position.Y));
            if (position.X > WarpBorder.End.X)
                WarpMouse(new Vector2(position.X - WarpBorder.Size.X, position.Y));

            if (position.Y < WarpBorder.Position.Y)
                WarpMouse(new Vector2(position.X, position.Y + WarpBorder.End.Y));
            if (position.Y > WarpBorder.End.Y)
                WarpMouse(new Vector2(position.X, position.Y - WarpBorder.Size.Y));
        }
    }

    void WarpMouse(Vector2 newPosition)
    {
        Viewport.WarpMouse(newPosition);
        lastDragPosition = newPosition;
    }

    //Static
    public static bool IsPressed(MouseButton button)
    {
        if (!current.mouseButtonIsPressed.ContainsKey(button))
            return false;
        return current.mouseButtonIsPressed[button];
    }
}
