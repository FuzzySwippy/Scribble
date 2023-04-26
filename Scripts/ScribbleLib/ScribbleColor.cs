using System;
using Godot;

namespace ScribbleLib;

public class ScribbleColor : IEquatable<ScribbleColor>
{
    float h, s, v;
    public float H
    {
        get => h;
        set
        {
            h = value;
            UpdateRGB();
        }
    }

    public float S
    {
        get => s;
        set
        {
            s = value;
            UpdateRGB();
        }
    }

    public float V
    {
        get => v;
        set
        {
            v = value;
            UpdateRGB();
        }
    }


    float r, g, b;
    public float R
    {
        get => r;
        set
        {
            r = value;
            UpdateHSV();
        }
    }

    public float G
    {
        get => g;
        set
        {
            g = value;
            UpdateHSV();
        }
    }

    public float B
    {
        get => b;
        set
        {
            b = value;
            UpdateHSV();
        }
    }


    float a;
    public float A
    {
        get => a;
        set => a = value;
    }


    public Color GodotColor => new(r, g, b, a);


    public ScribbleColor() { }

    public ScribbleColor(uint rgba)
    {
        byte[] data = BitConverter.GetBytes(rgba);
        r = ((float)data[0]) / byte.MaxValue;
        g = ((float)data[1]) / byte.MaxValue;
        b = ((float)data[2]) / byte.MaxValue;
        a = ((float)data[3]) / byte.MaxValue;
        UpdateHSV();
    }

    public ScribbleColor(float r, float g, float b, float a = 1) => SetRGBA(r, g, b, a);

    public void SetRGB(float r, float g, float b) => SetRGBA(r, g, b, a);

    public void SetHSV(float h, float s, float v) => SetHSVA(h, s, v, a);

    public void SetRGBA(float r, float g, float b, float a = 1)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
        UpdateHSV();
    }

    public void SetHSVA(float h, float s, float v, float a = 1)
    {
        this.h = h;
        this.s = s;
        this.v = v;
        this.a = a;
        UpdateRGB();
    }

    /// <summary>
    /// Sets color values from Godot HSV data
    /// </summary>
    /// <param name="color">Godot color</param>
    public void SetFromHSVA(Color color)
    {
        color.ToHsv(out float hue, out float saturation, out float value);

        if (saturation > 0 && saturation < 1 && value > 0 && value < 1)
            h = hue;

        if (value > 0 && value < 1)
            s = saturation;

        v = value;
        a = color.A;
        UpdateRGB();
    }

    public void SetFromRGBA(Color color)
    {
        r = color.R;
        g = color.G;
        b = color.B;
        a = color.A;
        UpdateHSV();
    }

    public void CloneFrom(ScribbleColor color)
    {
        r = color.R;
        g = color.G;
        b = color.B;

        h = color.H;
        s = color.S;
        v = color.V;

        a = color.A;
    }

    void UpdateRGB()
    {
        Color color = Color.FromHsv(h, s, v);
        r = color.R;
        g = color.G;
        b = color.B;
    }

    void UpdateHSV()
    {
        GodotColor.ToHsv(out float hue, out float saturation, out float value);

        if ((r + g + b).InRangeEx(0, 3))
        {
            h = hue;
            s = saturation;
        }
        v = value;
    }

    public uint ToRGBA32() => new Color(r, g, b, a).ToRgba32();


    //Overrides
    public bool Equals(ScribbleColor other) => r == other.r && g == other.g && b == other.b && a == other.a;

    public override bool Equals(object obj) => obj is ScribbleColor color && Equals(color);

    public static bool operator ==(ScribbleColor left, ScribbleColor right) => left.Equals(right);

    public static bool operator !=(ScribbleColor left, ScribbleColor right) => !(left == right);

    public override int GetHashCode() => (int)ToRGBA32();

    public override string ToString() => $"(RGB: {r}, {g}, {b} | HSV: {h}, {s}, {v} | A: {a})";
}
