using Godot;

namespace Scribble.ScribbleLib.Input;
public partial class InputController : Control
{
	private Mouse mouse;
	private Keyboard keyboard;


	public override void _Ready()
	{
		mouse = new(GetViewport());
		keyboard = new();
	}

	public override void _Input(InputEvent @event)
	{
		//Mouse
		if (@event is InputEventMouseButton mouseButtonEvent)
			mouse.HandleButton(new(mouseButtonEvent.ButtonIndex, mouseButtonEvent.GetModifiersMask()), mouseButtonEvent.Pressed, mouseButtonEvent.Position);
		else if (@event is InputEventMouseMotion mouseMotionEvent)
			mouse.HandleMotion(mouseMotionEvent.ButtonMask, mouseMotionEvent.GetModifiersMask(), mouseMotionEvent.Position, GetGlobalMousePosition(), mouseMotionEvent.Velocity);
		//Keyboard
		else if (@event is InputEventKey keyEvent)
			keyboard.HandleKey(keyEvent.PhysicalKeycode, keyEvent.GetModifiersMask(), keyEvent.Echo, keyEvent.Pressed);
	}
}
