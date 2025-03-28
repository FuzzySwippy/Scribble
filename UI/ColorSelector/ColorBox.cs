using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class ColorBox : Control
{
	public ColorInput ColorInput { get; set; }

	private Control target;
	private Control selector;
	private TextureRect selectorTextureRect;
	private Gradient baseColorGradient;

	private new Vector2 Position
	{
		get => selector.Position;
		set => selector.Position = value;
	}

	private Vector2 MinPosition => target.Position;

	private Vector2 MaxPosition => target.Position + target.Size;

	private new Vector2 Size => MaxPosition - MinPosition;

	private Vector2 NormalizedPos => Position - MinPosition;

	private Vector2 NormalizedMaxPos => MaxPosition - MinPosition;

	public float SValue => NormalizedPos.X / NormalizedMaxPos.X;
	public float VValue => 1f - NormalizedPos.Y / NormalizedMaxPos.Y;

	private bool UpdateVisualizationOnDraw { get; set; }

	public override void _Ready()
	{
		target = GetChild<Control>(0);
		selector = GetChild<Control>(2);
		selectorTextureRect = selector.GetChild<TextureRect>(0);
		SetUpGradient();

		Main.Ready += UpdateHue;
	}

	public override void _Draw()
	{
		if (UpdateVisualizationOnDraw)
		{
			UpdateVisualizationOnDraw = false;
			UpdateVisualization();
		}
	}

	private void SetUpGradient()
	{
		GradientTexture2D gradientTexture = (GradientTexture2D)Global.ColorBoxGradientTexture.Duplicate(true);
		GetChild<TextureRect>(0).Texture = gradientTexture;
		baseColorGradient = gradientTexture.Gradient;
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motionEvent)
		{
			if (motionEvent.ButtonMask == MouseButtonMask.Left)
			{
				Position = motionEvent.Position;
				Position = Position.Clamp(MinPosition, MaxPosition);

				ColorInput.SetColorFromHueAndColorBox();
			}
		}
		else if (@event is InputEventMouseButton buttonEvent)
		{
			if (buttonEvent.ButtonMask == MouseButtonMask.Left)
			{
				Position = buttonEvent.Position;
				Position = Position.Clamp(MinPosition, MaxPosition);

				ColorInput.SetColorFromHueAndColorBox();
			}
		}
	}

	public void UpdateVisualization()
	{
		if (Size == Vector2.Zero)
		{
			UpdateVisualizationOnDraw = true;
			return;
		}

		UpdateHue();
		UpdateSelectorPosition();
	}

	private void UpdateSelectorPosition() =>
		Position = new(ColorInput.Color.S * Size.X + MinPosition.X, MaxPosition.Y - ColorInput.Color.V * Size.Y);

	public void UpdateHue() => baseColorGradient.SetColor(1, Color.FromHsv(ColorInput.Color.H, 1, 1, 1));
}
