namespace Scribble.Drawing;

public enum OverlayType
{
	All,

	/// <summary>
	/// Used for drawing the effect area overlay.
	/// </summary>
	EffectArea,

	/// <summary>
	/// Used for drawing the effect area overlay when the primary overlay is already in use.
	/// </summary>
	EffectAreaAlt,
	Selection,
	NoSelection
}
