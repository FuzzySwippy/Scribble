using System.Net.Sockets;
using Godot;
using System;

public partial class CameraController : Camera2D
{
    float speed = 500;
    bool isDragging = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Up"))
            Position = new Vector2(Position.X, (float)(Position.Y - speed * delta));

        if (Input.IsActionPressed("Down"))
            Position = new Vector2(Position.X, (float)(Position.Y + speed * delta));

        if (Input.IsActionPressed("Right"))
            Position = new Vector2((float)(Position.X + speed * delta), Position.Y);

        if (Input.IsActionPressed("Left"))
            Position = new Vector2((float)(Position.X - speed * delta), Position.Y);

		//if (Mouse)
    }

    
}
