using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class ExportScaledWindow : Node
{
	private LineEdit XSize { get; set; }
	private LineEdit YSize { get; set; }

	private HSlider XScaleSlider { get; set; }
	private Label XScaleLabel { get; set; }

	private HSlider YScaleSlider { get; set; }
	private Label YScaleLabel { get; set; }

	private Button LockScaleButton { get; set; }
	private Button UnlockScaleButton { get; set; }

	private Button ExportButton { get; set; }
	private Button CancelButton { get; set; }


	private Vector2I OriginalSize { get; set; }
	private float ScaleRatio { get; set; }

	private Vector2I CurrentSize { get; set; }
	private bool ScaleLocked { get; set; }
	private bool IgnoreXTextChange { get; set; }
	private bool IgnoreYTextChange { get; set; }
	private bool IgnoreSliderChange { get; set; }

	public override void _Ready()
	{
		GetControls();
		SetEvents();
		SetupButtons();

		Main.Ready += () => WindowManager.Get("export_scaled").WindowShow += SetupControls;
	}

	private void GetControls()
	{
		XSize = this.GetGrandChild(2).GetChild<LineEdit>(1);
		YSize = this.GetGrandChild(2).GetChild<LineEdit>(3);

		XScaleSlider = GetChild(0).GetGrandChild<HSlider>(2, 1);
		XScaleLabel = GetChild(0).GetChild(1).GetChild<Label>(2);

		YScaleSlider = GetChild(0).GetChild(2).GetChild<HSlider>(1);
		YScaleLabel = GetChild(0).GetChild(2).GetChild<Label>(2);

		LockScaleButton = GetChild(0).GetChild(3).GetChild<Button>(1);
		UnlockScaleButton = GetChild(0).GetChild(3).GetChild<Button>(2);

		ExportButton = GetChild(0).GetChild(4).GetChild<Button>(0);
		CancelButton = GetChild(0).GetChild(4).GetChild<Button>(1);
	}

	private void SetEvents()
	{
		XSize.TextChanged += text => OnSizeChanged(text, true);
		YSize.TextChanged += text => OnSizeChanged(text, false);

		XScaleSlider.ValueChanged += OnXScaleChanged;
		YScaleSlider.ValueChanged += OnYScaleChanged;
	}

	private void SetupControls()
	{
		OriginalSize = Global.Canvas.Size;
		CurrentSize = OriginalSize;
		ScaleRatio = (float)OriginalSize.X / OriginalSize.Y;

		//Sliders
		float minXSliderValue = 1f / OriginalSize.X;
		float maxXSliderValue = (float)Canvas.MaxResolution * 2 / OriginalSize.X;
		float minYSliderValue = 1f / OriginalSize.Y;
		float maxYSliderValue = (float)Canvas.MaxResolution * 2 / OriginalSize.Y;

		IgnoreSliderChange = true;
		XScaleSlider.MinValue = minXSliderValue;
		XScaleSlider.MaxValue = maxXSliderValue;
		XScaleSlider.Step = minXSliderValue;
		XScaleSlider.Value = 1;
		XScaleLabel.Text = "1.00";

		YScaleSlider.MinValue = minYSliderValue;
		YScaleSlider.MaxValue = maxYSliderValue;
		YScaleSlider.Step = minYSliderValue;
		YScaleSlider.Value = 1;
		YScaleLabel.Text = "1.00";

		//Size
		XSize.Text = CurrentSize.X.ToString();
		YSize.Text = CurrentSize.Y.ToString();

		LockScale();
		IgnoreSliderChange = false;
	}

	private void SetupButtons()
	{
		LockScaleButton.Pressed += LockScale;
		UnlockScaleButton.Pressed += UnlockScale;
		ExportButton.Pressed += Export;
		CancelButton.Pressed += Close;
	}

	private void LockScale()
	{
		ScaleLocked = true;

		YSize.Editable = false;
		YScaleSlider.Editable = false;

		CurrentSize = new Vector2I(CurrentSize.X, (int)Mathf.Ceil(CurrentSize.X / ScaleRatio));
		YSize.Text = CurrentSize.Y.ToString();
		YScaleSlider.Value = (float)CurrentSize.Y / OriginalSize.Y;

		UnlockScaleButton.Show();
		LockScaleButton.Hide();
	}

	private void UnlockScale()
	{
		ScaleLocked = false;

		YSize.Editable = true;
		YScaleSlider.Editable = true;
		UnlockScaleButton.Hide();
		LockScaleButton.Show();
	}

	private void OnSizeChanged(string text, bool isX)
	{
		int value = int.TryParse(text, out value) ? value : 0;
		bool valueChanged = false;

		if (value > Canvas.MaxResolution * 2)
		{
			value = Canvas.MaxResolution;
			valueChanged = true;
		}
		else if (value < Canvas.MinResolution)
		{
			value = Canvas.MinResolution;
			valueChanged = true;
		}

		if (isX)
		{
			CurrentSize = new Vector2I(value, CurrentSize.Y);
			IgnoreXTextChange = true;
			XScaleSlider.Value = (float)CurrentSize.X / OriginalSize.X;

			if (valueChanged)
			{
				XSize.Text = value.ToString();
				XSize.CaretColumn = value.ToString().Length;
			}
		}
		else
		{
			CurrentSize = new Vector2I(CurrentSize.X, value);
			IgnoreYTextChange = true;
			YScaleSlider.Value = (float)CurrentSize.Y / OriginalSize.Y;

			if (valueChanged)
			{
				YSize.Text = value.ToString();
				YSize.CaretColumn = value.ToString().Length;
			}
		}
	}

	private void OnXScaleChanged(double newValue)
	{
		if (IgnoreSliderChange)
			return;

		CurrentSize = new Vector2I((int)(OriginalSize.X * newValue), CurrentSize.Y);
		XScaleLabel.Text = $"{(float)CurrentSize.X / OriginalSize.X:0.00}";

		if (ScaleLocked)
		{
			if (IgnoreXTextChange)
				IgnoreYTextChange = true;

			YScaleSlider.Value = (float)CurrentSize.X / OriginalSize.X;
		}

		if (!IgnoreXTextChange)
			XSize.Text = CurrentSize.X.ToString();
		IgnoreXTextChange = false;
	}

	private void OnYScaleChanged(double newValue)
	{
		if (IgnoreSliderChange)
			return;

		CurrentSize = new Vector2I(CurrentSize.X, (int)(OriginalSize.Y * newValue));
		YScaleLabel.Text = $"{(float)CurrentSize.Y / OriginalSize.Y:0.00}";

		if (!IgnoreYTextChange)
			YSize.Text = CurrentSize.Y.ToString();
		IgnoreYTextChange = false;
	}

	private void Close() => WindowManager.Get("export_scaled").Hide();

	private void Export()
	{
		FileDialogs.Show(FileDialogType.Export);
		Close();
	}
}
