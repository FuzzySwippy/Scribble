using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing;

public class History
{
	public List<HistoryAction> Actions { get; } = new();
	public int LastActionIndex { get; private set; } = -1;

	public History()
	{
		Keyboard.KeyDown += KeyDown;
	}

	public void AddAction(HistoryAction action)
	{
		if (!action.HasChanges)
			return;

		Actions.RemoveRange(LastActionIndex + 1, Actions.Count - LastActionIndex - 1);

		//Merge with last action if possible
		if (action.TryMerge && LastActionIndex >= 0 && Actions[LastActionIndex].Merge(action))
			return;

		Actions.Add(action);
		LastActionIndex = Actions.Count - 1;
	}

	public void Undo()
	{
		if (LastActionIndex < 0)
			return;

		Actions[LastActionIndex].Undo();
		LastActionIndex--;
	}

	public void Redo()
	{
		if (LastActionIndex >= Actions.Count - 1)
			return;

		LastActionIndex++;
		Actions[LastActionIndex].Redo();
	}

	public void JumpToAction(int index)
	{
		if (index < 0 || index >= Actions.Count)
			return;

		while (LastActionIndex > index)
			Undo();

		while (LastActionIndex < index)
			Redo();
	}

	private void KeyDown(KeyCombination combination)
	{
		if (!Global.WindowManager.WindowOpen && combination.key == Key.Z)
		{
			switch (combination.modifiers)
			{
				case KeyModifierMask.MaskCtrl:
					Undo();
					break;
				case KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift:
					Redo();
					break;
			}
		}
	}
}
