using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class GridWindow : Control
{
	#region Mappings
	private Dictionary<int, Color> ColorMap { get; } = new()
	{
		{ 0, new Color(0, 0, 0, 1) },
		{ 1, new Color(0.5f, 0.5f, 0.5f, 1) },
		{ 2, new Color(1, 1, 1, 1) },
	};

	private Dictionary<int, int> IntervalMap { get; } = new()
	{
		{ 0, 1 },
		{ 1, 2 },
		{ 2, 4 },
		{ 3, 8 },
		{ 4, 16 },
		{ 5, 32 },
		{ 6, 64 }
	};

	private int ColorToGridColorIndex(Color color) =>
		ColorMap.FirstOrDefault(pair => pair.Value.ToHtml() == color.ToHtml()).Key;

	private Color GridColorIndexToColor(int index) =>
		ColorMap[index];

	private int IntervalToGridIntervalIndex(int interval) =>
		IntervalMap.FirstOrDefault(pair => pair.Value == interval).Key;

	private int GridIntervalIndexToInterval(int index) =>
		IntervalMap[index];
	#endregion


	[ExportGroup("Settings")]
	[Export] private CheckButton gridEnabled;
	[Export] private OptionButton gridColor;
	[Export] private OptionButton gridInterval;

	[ExportGroup("Buttons")]
	[Export] private Button okButton;
	[Export] private Button applyButton;
	[Export] private Button cancelButton;

	private Settings Settings { get; set; }

	public override void _Ready()
	{
		SetupButtons();
		SetupControls();
		Main.Ready += () => WindowManager.Get("grid").WindowShow += SetupWindow;
	}

	private void SetupButtons()
	{
		okButton.Pressed += () => { Global.Settings.Apply(Settings); Close(); };
		applyButton.Pressed += () => Global.Settings.Apply(Settings);
		cancelButton.Pressed += Close;
	}

	private void Close() => WindowManager.Get("grid").Hide();

	private void SetupControls()
	{
		gridEnabled.Toggled += OnGridEnabledToggled;
		gridColor.ItemSelected += OnGridColorItemSelected;
		gridInterval.ItemSelected += OnGridIntervalItemSelected;
	}

	private void SetupWindow()
	{
		Settings = Global.Settings.GetSettings();

		gridEnabled.ButtonPressed = Settings.GridEnabled;
		gridColor.Selected = ColorToGridColorIndex(Settings.GridColor);
		gridInterval.Selected = IntervalToGridIntervalIndex(Settings.GridInterval);
	}

	private void OnGridEnabledToggled(bool enabled)
	{
		if (Settings != null)
			Settings.GridEnabled = enabled;
	}

	private void OnGridColorItemSelected(long index)
	{
		if (Settings != null)
			Settings.GridColor = GridColorIndexToColor((int)index);
	}

	private void OnGridIntervalItemSelected(long index)
	{
		if (Settings != null)
			Settings.GridInterval = GridIntervalIndexToInterval((int)index);
	}
}
