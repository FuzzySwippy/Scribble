using Godot;

namespace ScribbleLib.Input;
public class Keyboard
{
    static Keyboard current;
    
    public Keyboard()
    {
        current = this;
    }
}