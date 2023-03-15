using Godot;
using System;
using System.Transactions;

namespace Scribble;

public partial class Global : Node
{
    static Global current;

    [ExportCategory("Global Values")]
    [ExportGroup("Debug")]
    [Export] DebugInfo debugInfo;
    public static DebugInfo DebugInfo { get => current.debugInfo; }

    [ExportGroup("UI")]
    [Export] Status status;
    public static Status Status { get => current.status; }
    [Export] LabelSettings labelSettings;
    public static LabelSettings LabelSettings { get => current.labelSettings; }

    public override void _Ready() => current = this;
}
