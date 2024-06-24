using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Godot;

using Environment = System.Environment;
using Scribble.Drawing;

namespace Scribble.Application;

public static class FileManager
{
	private static string StorageDirectory { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Scribble";

	//static string DefaultSaveDirectory { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}";
	private static string TempDirectory { get; } = $"{StorageDirectory}/Temp";
	private static string SettingsPath { get; } = $"{StorageDirectory}/Settings.json";
	private static string PalettesPath { get; } = $"{StorageDirectory}/Palettes.json";

	static FileManager() => ValidateDirectories();

	private static bool ValidateDirectories()
	{
		try
		{
			if (!Directory.Exists(StorageDirectory))
			{
				Directory.CreateDirectory(StorageDirectory);
				GD.Print($"Created missing storage directory at: {StorageDirectory}");
			}

			if (!Directory.Exists(TempDirectory))
			{
				Directory.CreateDirectory(TempDirectory);
				GD.Print($"Created missing temp directory at: {TempDirectory}");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to create directories: {ex.Message}");
			return false;
		}
		return true;
	}

	#region Settings
	public static void SaveSettings(Dictionary<string, object> settings)
	{
		if (!ValidateDirectories())
			return;

		try
		{
			string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
			File.WriteAllText(SettingsPath, json);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to save settings: {ex.Message}");
		}
	}

	public static Dictionary<string, object> LoadSettings()
	{
		if (!ValidateDirectories())
			return new();

		if (!File.Exists(SettingsPath))
			return new();

		try
		{
			string json = File.ReadAllText(SettingsPath);
			return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to load settings: {ex.Message}");
			return new();
		}
	}
	#endregion

	#region Palettes
	public static void SavePalettes(List<Palette> palettes)
	{
		if (!ValidateDirectories())
			return;

		try
		{
			string json = JsonConvert.SerializeObject(palettes, Formatting.Indented);
			File.WriteAllText(PalettesPath, json);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to save palettes: {ex.Message}");
		}
	}

	public static List<Palette> LoadPalettes()
	{
		if (!ValidateDirectories())
			return new();

		if (!File.Exists(PalettesPath))
			return new();

		try
		{
			string json = File.ReadAllText(PalettesPath);
			return JsonConvert.DeserializeObject<List<Palette>>(json);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to load palettes: {ex.Message}");
			return new();
		}
	}
	#endregion
}