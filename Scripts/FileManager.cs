using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;

using Environment = System.Environment;

namespace Scribble;

public static class FileManager
{
    static string StorageDirectory { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Scribble";
    static string DefaultSaveDirectory { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}";
    static string TempDirectory { get; } = $"{StorageDirectory}/Temp";
    static string SettingsPath { get; } = $"{StorageDirectory}/Settings.json";
    static string PalettesPath { get; } = $"{StorageDirectory}/Palettes.json";

    static FileManager() => ValidateDirectories();

    static bool ValidateDirectories()
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
    public static void SaveSettings(Settings settings)
    {
        if (!ValidateDirectories())
            return;

        try
        {
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to save settings: {ex.Message}");
        }
    }

    public static Settings LoadSettings()
    {
        if (!ValidateDirectories())
            return new();

        if (!File.Exists(SettingsPath))
            return new();

        try
        {
            string json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<Settings>(json);
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
            string json = JsonSerializer.Serialize(palettes);
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
            return JsonSerializer.Deserialize<List<Palette>>(json);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to load palettes: {ex.Message}");
            return new();
        }
    }
    #endregion
}