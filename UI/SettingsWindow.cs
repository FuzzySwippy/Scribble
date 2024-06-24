using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class SettingsWindow : Control
{
	[ExportGroup("Autosave")]
	[Export] private CheckButton autosaveEnabled;
	[Export] private SpinBox autosaveIntervalMinutes;

	[ExportGroup("History")]
	[Export] private SpinBox historySize;

	[ExportGroup("UI")]
	[Export] private HSlider uiContentScale;
	[Export] private Label uiContentScaleLabel;

	[ExportGroup("Buttons")]
	[Export] private Button okButton;
	[Export] private Button applyButton;
	[Export] private Button defaultButton;
	[Export] private Button cancelButton;

	private Settings Settings { get; set; }

	public override void _Ready()
	{
		SetupButtons();
		SetupControls();
		Main.Ready += () => WindowManager.Get("settings").WindowShow += SetupWindow;
	}

	private void SetupButtons()
	{
		okButton.Pressed += () => { Global.Settings.Apply(Settings); Close(); };
		applyButton.Pressed += () => Global.Settings.Apply(Settings);
		defaultButton.Pressed += () =>
		{
			Global.Settings.Apply(Global.Settings.GetDefaultSettings());
			SetupWindow();
		};
		cancelButton.Pressed += Close;
	}

	private void SetupControls()
	{
		autosaveEnabled.Toggled += OnAutosaveEnabledToggled;
		autosaveIntervalMinutes.ValueChanged += OnAutosaveIntervalMinutesValueChanged;
		historySize.ValueChanged += OnHistorySizeValueChanged;
		uiContentScale.ValueChanged += OnUiContentScaleValueChanged;
	}

	private void Close() => WindowManager.Get("settings").Hide();

	private void SetupWindow()
	{
		//Autosave
		autosaveIntervalMinutes.MaxValue = Settings.MaxAutosaveIntervalMinutes;
		autosaveIntervalMinutes.MinValue = Settings.MinAutosaveIntervalMinutes;
		autosaveIntervalMinutes.Step = Settings.AutosaveIntervalStep;

		//History
		historySize.MaxValue = Settings.MaxHistorySize;
		historySize.MinValue = Settings.MinHistorySize;
		historySize.Step = Settings.HistorySizeStep;

		//UI
		uiContentScale.MaxValue = Settings.MaxContentScale;
		uiContentScale.MinValue = Settings.MinContentScale;
		uiContentScale.Step = Settings.ContentScaleStep;

		//Set values
		Settings = Global.Settings.GetSettings();

		autosaveEnabled.ButtonPressed = Settings.AutosaveEnabled;
		autosaveIntervalMinutes.Value = Settings.AutosaveIntervalMinutes;
		historySize.Value = Settings.HistorySize;
		uiContentScale.Value = Settings.ContentScale;
	}

	private void OnAutosaveEnabledToggled(bool pressed)
	{
		if (Settings != null)
			Settings.AutosaveEnabled = pressed;
	}

	private void OnAutosaveIntervalMinutesValueChanged(double value)
	{
		if (Settings != null)
			Settings.AutosaveIntervalMinutes = (int)value;
	}

	private void OnHistorySizeValueChanged(double value)
	{
		if (Settings != null)
			Settings.HistorySize = (int)value;
	}

	private void OnUiContentScaleValueChanged(double value)
	{
		if (Settings == null)
			return;

		Settings.ContentScale = (float)value;
		uiContentScaleLabel.Text = value.ToString("0.00");
	}
}
