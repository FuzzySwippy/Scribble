using System.Collections.Generic;
using Scribble.Application;

namespace Scribble.Drawing;

public class History
{
	public List<HistoryAction> Actions { get; } = [];
	public int LastActionIndex { get; private set; } = -1;

	public void AddAction(HistoryAction action)
	{
		if (!action.HasChanges)
			return;

		Actions.RemoveRange(LastActionIndex + 1, Actions.Count - LastActionIndex - 1);

		action.Build();

		//Merge with last action if possible
		if (action.TryMerge && LastActionIndex >= 0 && Actions[LastActionIndex].Merge(action))
			return;

		Actions.Add(action);

		//Remove oldest actions if history size is exceeded
		if (Actions.Count >= Global.Settings.HistorySize)
			for (int i = 0; i < Actions.Count - Global.Settings.HistorySize; i++)
				Actions.RemoveAt(0);

		LastActionIndex = Actions.Count - 1;

		Global.HistoryList.Update();
	}

	public void Undo()
	{
		if (LastActionIndex < 0)
			return;

		//Stop playing animation
		Global.AnimationTimeline.Playing = false;

		Actions[LastActionIndex].Undo();
		LastActionIndex--;

		Global.HistoryList.UpdateColorsAndSelection();
	}

	public void Redo()
	{
		if (LastActionIndex >= Actions.Count - 1)
			return;

		//Stop playing animation
		Global.AnimationTimeline.Playing = false;

		LastActionIndex++;
		Actions[LastActionIndex].Redo();

		Global.HistoryList.UpdateColorsAndSelection();
	}

	public void JumpToAction(int index)
	{
		if (index < 0 || index >= Actions.Count || index == LastActionIndex)
			return;

		while (LastActionIndex > index)
			Undo();

		while (LastActionIndex < index)
			Redo();
	}
}
