namespace Scribble.Settings;

public class MainSettings
{
	public CanvasSettings Canvas { get; set; } = new();
	public UISettings UI { get; set; } = new();
}
