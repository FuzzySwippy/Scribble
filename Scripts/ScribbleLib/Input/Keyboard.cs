using System.Collections.Generic;
using System;
using Godot;

namespace ScribbleLib.Input;
public class Keyboard
{
    public delegate void KeyboardKeyEvent(Key key);


    static Keyboard current;

    //Key actuation
    public static event KeyboardKeyEvent KeyDown;
    public static event KeyboardKeyEvent KeyUp;

    
    Dictionary<Key, bool> pressedKeys = new();

    public Keyboard()
    {
        current = this;

        //Fill the key dictionary with values
        foreach (Key key in Enum.GetValues(typeof(Key)))
            pressedKeys.Add(key, false);
    }

    //Internal
    public void HandleKey(Key key, bool echo, bool pressed)
    {
        if (pressed)
        {
            if (!echo)
            {
                pressedKeys[key] = true;
                KeyDown?.Invoke(key);
            }
        }
        else
            pressedKeys[key] = false;
    }

    //Static
    public static bool IsPressed(Key key) => current.pressedKeys[key];
}