using Godot;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing;

public class Artist
{
	public Color EffectAreaOverlayColor { get; set; } = new(0, 0.5f, 1, 0.6f);
	public Palettes Palettes { get; } = new();
	private ScribbleColor[] QuickPencilColors { get; } = new ScribbleColor[4];

	public Artist()
	{
		Keyboard.KeyDown += KeyDown;
		Mouse.Scroll += Scroll;

		for (int i = 0; i < 4; i++)
			QuickPencilColors[i] = new(1, 1, 1);

		//Set default colors
		GetQuickPencilColor(QuickPencilType.Secondary).SetRGBA(0, 0, 0, 0);
		GetQuickPencilColor(QuickPencilType.AltPrimary).SetRGB(0, 0, 0);
		GetQuickPencilColor(QuickPencilType.AltSecondary).SetRGB(0, 0, 1);

		Status.Set("brush_size", Brush.Size);
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

	public ScribbleColor GetQuickPencilColor(QuickPencilType type) => QuickPencilColors[(int)type];
}
