using System.Collections.Generic;
using System;
using Godot;

namespace Scribble.ScribbleLib.Input;
public class Keyboard
{
	public delegate void KeyboardKeyEvent(KeyCombination combination);

	private static Keyboard current;

	//Key actuation
	public static event KeyboardKeyEvent KeyDown;
	public static event KeyboardKeyEvent KeyUp;

	private readonly Dictionary<Key, bool> pressedKeys = new();
	private readonly Dictionary<Key, KeyModifierMask> pressedKeyModifiers = new();

	public Keyboard()
	{
		current = this;

		//Fill the key dictionary with values
		foreach (Key key in Enum.GetValues(typeof(Key)))
		{
			pressedKeys.Add(key, false);
			pressedKeyModifiers.Add(key, 0);
		}
	}

	//Internal
	public void HandleKey(Key key, KeyModifierMask modifiers, bool echo, bool pressed)
	{
		if (pressed)
		{
			if (!echo)
			{
				pressedKeys[key] = pressed;
				pressedKeyModifiers[key] = modifiers;
				KeyDown?.Invoke(new(key, modifiers));
			}
		}
		else
			EndKeyPress(key);

		if (pressedKeys[key] && pressedKeyModifiers[key] != 0 && pressedKeyModifiers[key] != modifiers)
			EndKeyPress(key);
	}

	private void EndKeyPress(Key key)
	{
		pressedKeys[key] = false;
		KeyUp?.Invoke(new(key, pressedKeyModifiers[key]));
		pressedKeyModifiers[key] = 0;
	}

	//Static
	public static bool IsPressed(Key key) => current.pressedKeys[key] && current.pressedKeyModifiers[key] == 0;
	public static bool IsPressed(KeyCombination combination) => current.pressedKeys[combination.key] && current.pressedKeyModifiers[combination.key] == combination.modifiers;
}

public readonly struct KeyCombination
{
	public readonly Key key;
	public readonly KeyModifierMask modifiers;

	public bool HasModifiers => modifiers != 0;


	public KeyCombination(Key key)
	{
		this.key = key;
		modifiers = 0;
	}

	public KeyCombination(Key key, KeyModifierMask modifiers)
	{
		this.key = key;
		this.modifiers = modifiers;
	}


	public override string ToString() => HasModifiers ? $"({modifiers} - {key})" : key.ToString();

	public override int GetHashCode() => base.GetHashCode();

	public override bool Equals(object obj) =>
		obj is KeyCombination combination && key == combination.key &&
			modifiers == combination.modifiers;


	public static bool operator ==(KeyCombination a, KeyCombination b) => a.Equals(b);
	public static bool operator !=(KeyCombination a, KeyCombination b) => !a.Equals(b);
}