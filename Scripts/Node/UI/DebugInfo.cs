using System.Collections.Generic;
using Godot;
using System;
using ScribbleLib.Input;
using System.IO;

namespace Scribble;
public partial class DebugInfo : VBoxContainer
{
    LabelSettings labelSettings;
    public Dictionary<string, Label> Labels { get; } = new()
    {
        {"fps_c", null},
        {"fps_d", null},
        {"cam_zoom", null},
        {"cam_pos", null},
        {"is_drawing", null},
        {"scale", null},
    };

    public override void _Ready()
    {
        CreateLabelSettings();
        GenerateLabels();

        Keyboard.KeyDown += KeyDown;
        Visible = false;
    }

    void KeyDown(Key key)
    {
        //Show/Hide debug info
        if (key == Key.F3)
            Visible = !Visible;
    }

    public override void _Process(double delta)
    {
        if (!Visible)
            return;

        CountFPS();
        CalculateFPS(delta);
    }

    void GenerateLabels()
    {
        foreach (string name in Labels.Keys)
        {
            Labels[name] = new()
            {
                LabelSettings = labelSettings
            };
            AddChild(Labels[name]);
        }
    }

    void CreateLabelSettings()
    {
        labelSettings = new()
        {
			LineSpacing = 0
        };
    }

    float frames;
    int lastSecond = DateTime.Now.Second;
    void CountFPS()
    {
        frames++;
        if (DateTime.Now.Second != lastSecond)
        {
            if (DateTime.Now.Second == lastSecond + 1)
                Labels["fps_c"].Text = $"{frames} FPS (counted)";
            else if (DateTime.Now.Second > lastSecond + 1)
                Labels["fps_c"].Text = $"{1 / (DateTime.Now.Second - lastSecond)} FPS (counted)";

            frames = 0;
            lastSecond = DateTime.Now.Second;
        }
	}

    DateTime fpsNextUpdateTime = DateTime.Now;
    void CalculateFPS(double deltaTime)
    {
        if (DateTime.Now > fpsNextUpdateTime)
        {
            Labels["fps_d"].Text = $"{(1 / deltaTime):.000} FPS (from frame time)";
            fpsNextUpdateTime = DateTime.Now + TimeSpan.FromSeconds(0.5);
        }
    }
}