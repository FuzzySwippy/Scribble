using System;
using System.Collections.Generic;
using Godot;
using Scribble.Drawing;
using ScribbleLib;
using ScribbleLib.Input;

using static Godot.CanvasItem;

namespace Scribble;
public class Canvas
{
    static float BaseScale { get; } = 2048;

    //Nodes
    public static MeshInstance2D MeshInstance { get; private set; }
    static Panel backgroundPanel;

    //Values
    public static Vector2 SizeInWorld { get; private set; }
    public Vector2I Size { get; private set; }
    public Vector2 TargetScale { get; private set; }
    public Vector2 PixelSize { get => TargetScale; }
    readonly Artist artist;
    Vector2 oldWindowSize;
    CanvasMesh mesh;

    readonly Dictionary<MouseCombination, PencilColorType> mouseColorMap = new()
    {
        { new (MouseButton.Left), PencilColorType.Primary },
        { new (MouseButton.Right), PencilColorType.Secondary },
        { new (MouseButton.Left, KeyModifierMask.MaskCtrl), PencilColorType.AltPrimary },
        { new (MouseButton.Right, KeyModifierMask.MaskCtrl), PencilColorType.AltSecondary },
    };

    //Pixel
    Vector2I oldMousePixelPos = Vector2I.One * -1;
    Vector2I frameMousePixelPos;
    public Vector2I MousePixelPos { get => frameMousePixelPos; }

    //Layers
    List<Layer> Layers { get; } = new();
    int currentLayerIndex = 0;
    Layer CurrentLayer { get => Layers[currentLayerIndex]; }

    //Dynamic properties
    Vector2 ScreenScaleMultiplier
    {
        get
        {
            Vector2 multiplier = Main.Window.Size.ToVector2() / Main.BaseWindowSize;
            if (multiplier.X > multiplier.Y)
                return new(multiplier.Y, multiplier.Y);
            return new(multiplier.X, multiplier.X);
        }
    }

    Brush Brush { get => artist.Brush; }

    public Canvas(Vector2I size, Artist artist)
    {
        MeshInstance ??= Global.CanvasNode.GetChild<MeshInstance2D>(1);
        backgroundPanel ??= Global.CanvasNode.GetChild<Panel>(0);
        this.artist = artist;

        CreateNew(size);
        Mouse.ButtonDown += MouseDown;
        Main.Window.SizeChanged += UpdateScale;

        Status.Set("canvas_size", Size);
    }

    ~Canvas()
    {
        Mouse.ButtonDown -= MouseDown;
        Main.Window.SizeChanged -= UpdateScale;
    }

    public void Update()
    {
        frameMousePixelPos = (Mouse.GlobalPosition / PixelSize).ToVector2I();

        if (oldMousePixelPos != MousePixelPos)
        {
            if (Spacer.MouseInBounds)
            {
                foreach (MouseCombination combination in mouseColorMap.Keys)
                    if (Mouse.IsPressed(combination))
                        Brush.Line(MousePixelPos, oldMousePixelPos, Brush.GetPencilColor(mouseColorMap[combination]));
            }
            oldMousePixelPos = MousePixelPos;
        }

        Status.Set("pixel_pos", MousePixelPos);
    }

    void MouseDown(MouseCombination combination, Vector2 position)
    {
        if (!Spacer.MouseInBounds)
            return;

        if (mouseColorMap.ContainsKey(combination))
            Brush.Pencil(MousePixelPos, Brush.GetPencilColor(mouseColorMap[combination]));
    }

    void UpdateScale()
    {
        TargetScale = Vector2.One * (BaseScale / (Size.X > Size.Y ? Size.X : Size.Y)) * ScreenScaleMultiplier;
        SizeInWorld = PixelSize * Size;

        //Update camera position based on the window size change
        Global.Camera.Position *= Main.Window.Size / oldWindowSize;
        oldWindowSize = Main.Window.Size;

        //Update node scales
        MeshInstance.Scale = TargetScale;
        backgroundPanel.Scale = TargetScale;
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
                {
                    colors[x, y] *= Layers[l].GetPixel(x, y);
                }
            }
        }

        return colors;
    }

    public void UpdateMesh()
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

    //New
    void CreateNew(Vector2I size)
    {
        Size = size;
        UpdateScale();

        Layers.Clear();
        NewLayer();

        mesh = new(this);
        SetBackgroundTexture();

        //Position the camera's starting position in the middle of the canvas
        Global.Camera.Position = SizeInWorld / 2;
    }

    void SetBackgroundTexture()
    {
        Global.BackgroundStyle.Texture = TextureGenerator.NewBackgroundTexture(Size * Settings.Canvas.BG_ResolutionMult);

        //Disable texture filtering and set background node size
        backgroundPanel.TextureFilter = TextureFilterEnum.Nearest;
        backgroundPanel.Size = Size;
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