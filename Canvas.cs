using Godot;
using Godot.Collections;

public partial class Canvas : MeshInstance2D
{
    CanvasMesh m;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //GenerateMesh();
        m = new(new(20,10), this);
        //m.GenerateMesh();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
