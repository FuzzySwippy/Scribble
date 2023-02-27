using Godot;

namespace ScribbleLib;
public class Input
{
    static Input current;
    static bool Initialized { get => current != null; }

    //Mouse drag
    public bool mouseIsDragging;
    public static bool MouseIsDragging { get => current.mouseIsDragging; }
    


    public Input()
    {
        current = this;
    }
}