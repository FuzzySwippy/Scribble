using Godot;
using System;
using System.Runtime.CompilerServices;

namespace ScribbleLib;

public static class ColorTools
{
    public static Color Grayscale(float value, float alpha = 1) => new(value, value, value, alpha);
}
