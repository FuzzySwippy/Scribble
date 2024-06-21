namespace Scribble.Drawing;

public abstract class HistoryAction
{
	public HistoryActionType ActionType { get; protected set; }
	public bool HasChanges { get; protected set; }
	public bool TryMerge { get; set; }

	public virtual void Undo() { }
	public virtual void Redo() { }

	public virtual bool Merge(HistoryAction action)
	{
		if (action.ActionType != ActionType)
			return false;
		return true;
	}

	/// <summary>
	/// Optimizes the stored data to take up less memory
	/// </summary>
	public virtual void Build() { }
}
