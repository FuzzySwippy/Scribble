using Godot;
using ScribbleLib;
using Input = ScribbleLib.Input;

public partial class InputController : Node
{
    Input input;

    //Mouse
    InputEventMouse mouseEvent;

    public override void _Ready() => input = new();

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouse)
        {
            mouseEvent = (InputEventMouse)inputEvent;

			//Mouse drag
            input.mouseIsDragging = (mouseEvent.IsPressed() && mouseEvent.ButtonMask.HasFlag(MouseButtonMask.Left));

        }

        base._Input(@inputEvent);
    }

	
}
