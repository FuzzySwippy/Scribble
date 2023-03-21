using Godot;
using System;

namespace Scribble;

public class Artist
{
	public Canvas Canvas { get; }

	public Artist(Vector2I canvasSize)
	{
        Canvas = new Canvas(canvasSize);
    }

    public void Update()
    {
        Canvas.Update();
    }
}
