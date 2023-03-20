using Godot;
using ScribbleLib.Input;

namespace ScribbleLib;
public partial class InputController : Control
{
    Mouse mouse;
    Keyboard keyboard;

    //Mouse events
    InputEventMouseButton mouseButtonEvent;
    InputEventMouseMotion mouseMotionEvent;

    //Keyboard events
    InputEventKey keyEvent;


    public override void _Ready()
    {
        mouse = new(GetViewport());
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
            mouse.HandleMotion(mouseMotionEvent.ButtonMask, mouseMotionEvent.Position, GetGlobalMousePosition(), mouseMotionEvent.Velocity);
        }
        //Keyboard
        else if (inputEvent is InputEventKey)
        {
            keyEvent = (InputEventKey)inputEvent;
            keyboard.HandleKey(keyEvent.PhysicalKeycode, keyEvent.Echo, keyEvent.Pressed);
            mouse.HandleMotion(mouseMotionEvent.ButtonMask, mouseMotionEvent.Position, GetGlobalMousePosition(), mouseMotionEvent.Velocity);
        }
    }
}
