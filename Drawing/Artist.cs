using Godot;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing;

public class Artist
{
	public Brush Brush { get; }

	public Palettes Palettes { get; } = new();

	public Artist()
	{
		Brush = new();

		Keyboard.KeyDown += KeyDown;
		Mouse.Scroll += Scroll;
	}

	private void Scroll(KeyModifierMask modifiers, int delta)
	{
		if ((KeyModifierMask.MaskCtrl & modifiers) == 0)
			return;

		Brush.Size += ((modifiers & KeyModifierMask.MaskShift) != 0 ? 10 : 1) * Mathf.Sign(delta);
	}

	private void KeyDown(KeyCombination combination)
	{
		if (combination.key == Key.Bracketleft)
			Brush.Size -= SizeAdd(combination.modifiers);
		else if (combination.key == Key.Bracketright)
			Brush.Size += SizeAdd(combination.modifiers);
	}

	private static int SizeAdd(KeyModifierMask modifiers)
	{
		if (modifiers == KeyModifierMask.MaskCtrl)
			return 2;
		else if (modifiers == (KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift))
			return 4;
		return 1;
	}
}
