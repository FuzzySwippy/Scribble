using System;
using System.Collections.Generic;
using System.Linq;
using Scribble.Application;
using Scribble.UI;

namespace Scribble;

public class Settings
{
	private static Dictionary<string, object> DefaultSettingsDict { get; } = new()
	{
		{ "AutosaveEnabled", true },
		{ "AutosaveIntervalMinutes", 5 },
		{ "HistorySize", 250 },
		{ "ContentScale", 1f },
		{ "PencilPreview", true },

		{ "GridEnabled", false },
		{ "GridColor", "000000ff" },
		{ "GridInterval", 1 }
	};


	private Dictionary<string, object> SettingsDict { get; set; } = new();

	private bool IsClone { get; } = false;

	#region Autosave
	public bool AutosaveEnabled
	{
		get => (bool)SettingsDict["AutosaveEnabled"];
		set => SettingsDict["AutosaveEnabled"] = value;
	}

	public int AutosaveIntervalMinutes
	{
		get => Convert.ToInt32(SettingsDict["AutosaveIntervalMinutes"]);
		set => SettingsDict["AutosaveIntervalMinutes"] = value;
	}
	public static int MinAutosaveIntervalMinutes { get; } = 1;
	public static int MaxAutosaveIntervalMinutes { get; } = 60;
	public static int AutosaveIntervalStep { get; } = 1;
	#endregion

	#region History
	public int HistorySize
	{
		get => Convert.ToInt32(SettingsDict["HistorySize"]);
		set => SettingsDict["HistorySize"] = value;
	}

	public static int MaxHistorySize { get; } = 1000;
	public static int MinHistorySize { get; } = 25;
	public static int HistorySizeStep { get; } = 25;
	#endregion

	#region UI
	public float ContentScale
	{
		get => Convert.ToSingle(SettingsDict["ContentScale"]);
		set => SettingsDict["ContentScale"] = value;
	}
	public static float MaxContentScale { get; } = 2;
	public static float MinContentScale { get; } = 0.5f;
	public static float ContentScaleStep { get; } = 0.25f;
	#endregion

	#region Tools
	public bool PencilPreview
	{
		get => (bool)SettingsDict["PencilPreview"];
		set => SettingsDict["PencilPreview"] = value;
	}
	#endregion

	#region Grid
	public bool GridEnabled
	{
		get => (bool)SettingsDict["GridEnabled"];
		set => SettingsDict["GridEnabled"] = value;
	}

	public Godot.Color GridColor
	{
		get => new(SettingsDict["GridColor"].ToString());
		set => SettingsDict["GridColor"] = value.ToHtml();
	}

	public int GridInterval
	{
		get => Convert.ToInt32(SettingsDict["GridInterval"]);
		set => SettingsDict["GridInterval"] = value;
	}
	#endregion

	public Settings()
	{
		Load();
		Main.Ready += () => Apply(this);
	}

	public Settings(Dictionary<string, object> settings)
	{
		SettingsDict = settings;
		IsClone = true;
	}

	public Settings GetSettings() =>
		new(SettingsDict.ToDictionary(entry => entry.Key, entry => entry.Value));
	public Settings GetDefaultSettings() =>
		new(DefaultSettingsDict.ToDictionary(entry => entry.Key, entry => entry.Value));

	public void Apply(Settings settings)
	{
		if (IsClone)
			throw new InvalidOperationException("Cannot apply settings to a clone.");

		SettingsDict = settings.SettingsDict;
		Save();

		//Content Scale
		UserInterface.ContentScale = ContentScale;

		//Grid
		Global.CanvasChunkMaterial.SetShaderParameter("show_grid", GridEnabled);
		Global.CanvasChunkMaterial.SetShaderParameter("line_color", GridColor);
		Global.CanvasChunkMaterial.SetShaderParameter("interval", GridInterval);
	}

	public void Load()
	{
		if (IsClone)
			throw new InvalidOperationException("Cannot load settings into a clone.");

		SettingsDict = FileManager.LoadSettings();
		if (SettingsDict == null || SettingsDict.Count == 0)
		{
			//Clone the default settings
			SettingsDict =
				DefaultSettingsDict.ToDictionary(entry => entry.Key, entry => entry.Value);
		}

		//Add any missing settings from the default settings
		foreach (string key in DefaultSettingsDict.Keys)
		{
			if (!SettingsDict.ContainsKey(key))
				SettingsDict[key] = DefaultSettingsDict[key];
		}
	}

	public void Save()
	{
		if (IsClone)
			throw new InvalidOperationException("Cannot save settings from a clone.");

		FileManager.SaveSettings(SettingsDict);
	}
}
