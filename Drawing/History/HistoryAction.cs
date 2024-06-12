namespace Scribble.Drawing;

public abstract class HistoryAction
{
	public HistoryActionType ActionType { get; protected set; }
	public bool HasChanges { get; protected set; }
	public virtual void Undo() { }
	public virtual void Redo() { }
}
