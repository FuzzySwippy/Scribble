namespace Scribble.Settings;

public class MainSettings
{
	public CanvasSettings Canvas { get; } = new();
	public UISettings UI { get; } = new();
}
