using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Scribble.Drawing;
using ScribbleLib.Extensions;
using ScribbleLib.Input;

namespace Scribble;
public partial class Canvas : MeshInstance2D
{
    public Vector2I Size { get; private set; }
    CanvasMesh mesh;

    //Pixel
    Vector2I oldMousePixelPos = Vector2I.One * -1;
    Vector2I frameMousePixelPos;
    public Vector2I MousePixelPos { get => frameMousePixelPos; }

    //Layers
    List<Layer> Layers { get; } = new();
    int currentLayerIndex = 0;
    Layer CurrentLayer { get => Layers[currentLayerIndex]; }

    //Events and overrides
    public override void _Ready()
    {
        CreateNew(new(256, 128+64));
        Mouse.ButtonDown += MouseDown;
    }

    public override void _Process(double delta)
    {
        frameMousePixelPos = (GetGlobalMousePosition() / mesh.PixelSize).ToVector2I();
        if (oldMousePixelPos != MousePixelPos)
        {
            if (Mouse.IsPressed(MouseButton.Left))
                SetLine(MousePixelPos, oldMousePixelPos, new(1, 1, 1, 1));
            else if (Mouse.IsPressed(MouseButton.Right))
                SetLine(MousePixelPos, oldMousePixelPos, new(0, 0, 0, 1));
            oldMousePixelPos = MousePixelPos;
        }

        GetNode<DebugInfo>("/root/main/UI/Debug_Canvas/DebugInfo").Labels["pixel_pos"].Text = $"Pixel under cursor: {MousePixelPos}";
    }

    void MouseDown(MouseButton button, Vector2 position)
    {
        if (button == MouseButton.Left)
            SetPixel(MousePixelPos, new(1, 1, 1, 1));
        else if (button == MouseButton.Right)
            SetPixel(MousePixelPos, new(0, 0, 0, 1));
    }

    //Drawing
    Color[,] FlattenLayers()
    {
        Color[,] colors = new Color[Size.X, Size.Y];
        for (int x = 0; x < Size.X; x++)
        {
            for (int y = 0; y < Size.Y; y++)
            {
                colors[x, y] = new(1, 1, 1, 1);
                for (int l = 0; l < Layers.Count; l++)
                    colors[x, y] *= Layers[l].GetPixel(x, y);
            }
        }

        return colors;
    }

    void UpdateMesh()
    {
        mesh.SetColors(FlattenLayers());
        mesh.Update();
    }

    public void SetPixel(Vector2I position, Color color, bool update = true)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
            return;
        CurrentLayer.SetPixel(position, color);

        if (update)
            UpdateMesh();
    }

    public Color GetPixel(Vector2I position)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
            return new();
        return CurrentLayer.GetPixel(position);
    }

    public void SetLine(Vector2 position1, Vector2 position2, Color color)
    {
        //Draw line pixels
        while (position1 != position2)
        {
            SetPixel(position1.ToVector2I(), color, false);
            position1 = position1.MoveToward(position2, 1);
        }
        SetPixel(position2.ToVector2I(), color, false);

        UpdateMesh();
    }

    //New
    public void CreateNew(Vector2I size)
    {
        Size = size;

        Layers.Clear();
        NewLayer();

        mesh = new(Size, this);

        //Position the camera's starting position in the middle of the canvas
        GetNode<Camera2D>("../Camera").Position = mesh.SizeInWorld / 2;
    }


    public Layer NewLayer()
    {
        Layer layer = new(this);
        Layers.Add(layer);

        //Select the new layer
        //...

        return layer;
    }
}
