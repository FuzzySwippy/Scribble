using Godot;
using System;

public partial class Label_FPS : Label
{
    float frames;
    int lastSecond;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        lastSecond = DateTime.Now.Second;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        frames++;
        if (DateTime.Now.Second != lastSecond)
        {
            if (DateTime.Now.Second == lastSecond + 1)
                Text = $"{frames} FPS";
            else if (DateTime.Now.Second > lastSecond + 1)
                Text = $"{1 / (DateTime.Now.Second - lastSecond)} FPS";

            frames = 0;
            lastSecond = DateTime.Now.Second;
        }
    }
}
