using System.Collections.Generic;
using Godot;
using ScribbleLib.Input;

namespace Scribble;

public class Artist
{
	public Canvas Canvas { get; }
    public Brush Brush { get; }

    public List<Palette> Palettes { get; } = new();

    public Artist(Vector2I canvasSize)
	{
        Canvas = new Canvas(canvasSize, this);
        Brush = new(Canvas);

        Keyboard.KeyDown += KeyDown;
        Mouse.Scroll += Scroll;
    }

    public void Update() => Canvas.Update();

    void Scroll(KeyModifierMask modifiers, int delta)
    {
        if ((KeyModifierMask.MaskCtrl & modifiers) == 0)
            return;

        Brush.Size += ((modifiers & KeyModifierMask.MaskShift) != 0 ? 10 : 1) * Mathf.Sign(delta);
    }

    void KeyDown(KeyCombination combination)
    { 
        if (combination.key == Key.Bracketleft)
            Brush.Size -= SizeAdd(combination.modifiers);
        else if (combination.key == Key.Bracketright)
            Brush.Size += SizeAdd(combination.modifiers);
    }

    static int SizeAdd(KeyModifierMask modifiers)
    { 
        if (modifiers == KeyModifierMask.MaskCtrl)
            return 2;
        else if (modifiers == (KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift))
            return 4;
        return 1;
    }
}
