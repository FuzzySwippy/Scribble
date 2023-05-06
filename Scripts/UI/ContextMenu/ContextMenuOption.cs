using System;

namespace Scribble;

public class ContextMenuOption
{
    public string Text { get; }
    public Action Action { get; }
    
    public ContextMenuOption(string text, Action action)
    {
        Text = text;
        Action = action;
    }   
}