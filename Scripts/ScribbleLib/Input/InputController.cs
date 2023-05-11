using Godot;
using ScribbleLib.Input;

namespace ScribbleLib;
public partial class InputController : Control
{
    Mouse mouse;
    Keyboard keyboard;


    public override void _Ready()
    {
        mouse = new(GetViewport());
        keyboard = new();
    }

    public override void _Input(InputEvent inputEvent)
    {
        //Mouse
        if (inputEvent is InputEventMouseButton mouseButtonEvent)
            mouse.HandleButton(new (mouseButtonEvent.ButtonIndex, mouseButtonEvent.GetModifiersMask()), mouseButtonEvent.Pressed, mouseButtonEvent.Position);
        else if (inputEvent is InputEventMouseMotion mouseMotionEvent)
            mouse.HandleMotion(mouseMotionEvent.ButtonMask, mouseMotionEvent.GetModifiersMask(), mouseMotionEvent.Position, GetGlobalMousePosition(), mouseMotionEvent.Velocity);
        //Keyboard
        else if (inputEvent is InputEventKey keyEvent)
            keyboard.HandleKey(keyEvent.PhysicalKeycode, keyEvent.GetModifiersMask(), keyEvent.Echo, keyEvent.Pressed);
    }
}
