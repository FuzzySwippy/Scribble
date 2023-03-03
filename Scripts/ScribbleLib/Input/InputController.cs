using System;
using Godot;
using ScribbleLib.Input;
using Input = ScribbleLib.Input;

namespace ScribbleLib;
public partial class InputController : Node
{
    Mouse mouse;
    Keyboard keyboard;

    //Mouse
    InputEventMouseButton mouseButtonEvent;
    InputEventMouseMotion mouseMotionEvent;

    //Keyboard
    InputEventKey keyEvent;

    public override void _Ready()
    {
        mouse = new();
        keyboard = new();
    }

    public override void _Input(InputEvent inputEvent)
    {
        //Mouse
        if (inputEvent is InputEventMouseButton)
        {
            mouseButtonEvent = (InputEventMouseButton)inputEvent;
            mouse.HandleButton(mouseButtonEvent.ButtonIndex, mouseButtonEvent.Pressed, mouseButtonEvent.Position);
        }
        else if (inputEvent is InputEventMouseMotion)
        {
            mouseMotionEvent = (InputEventMouseMotion)inputEvent;
            mouse.HandleMotion(mouseMotionEvent.ButtonMask, mouseMotionEvent.Position, mouseMotionEvent.Velocity);
        }
        //Keyboard
        else if (inputEvent is InputEventKey)
        {
            keyEvent = (InputEventKey)inputEvent;
            keyboard.HandleKey(keyEvent.PhysicalKeycode, keyEvent.Echo, keyEvent.Pressed);
            mouse.HandleMotion(mouseMotionEvent.ButtonMask, mouseMotionEvent.Position, mouseMotionEvent.Velocity);
        }
    }
}
