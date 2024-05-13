using Godot.Collections;
using Godot;

namespace Scribble.Drawing.Visualization;

public class CanvasMesh
{
	readonly ArrayMesh mesh;
	Array meshValues;
	StandardMaterial3D material;

	Vector2[] vertices;
	int[] indexes;
	Color[] colors;

	readonly Canvas canvas;
	public Vector2I Size => canvas.Size;

	public CanvasMesh(Canvas canvas)
	{
		this.canvas = canvas;
		mesh = new();
		canvas.MeshInstance.Mesh = mesh;

		Generate();
	}

	void Generate()
	{
		meshValues = new();
		meshValues.Resize((int)Mesh.ArrayType.Max);

		vertices = new Vector2[Size.X * Size.Y * 4];
		indexes = new int[Size.X * Size.Y * 6];
		colors = new Color[Size.X * Size.Y * 4];

		int arrayIndex;
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				arrayIndex = (y * Size.X) + x;
				AddPixel(arrayIndex * 4, arrayIndex * 6, x, y);
			}
		}

		material = new()
		{
			VertexColorUseAsAlbedo = true,
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
		};

		Update();
	}

	void AddPixel(int vertexID, int indexID, int x, int y)
	{
		vertices[vertexID] = new(x, y);
		vertices[vertexID + 1] = new(x, y + 1);
		vertices[vertexID + 2] = new(x + 1, y + 1);
		vertices[vertexID + 3] = new(x + 1, y);


		indexes[indexID] = vertexID;
		indexes[indexID + 1] = vertexID + 1;
		indexes[indexID + 2] = vertexID + 2;

		indexes[indexID + 3] = vertexID;
		indexes[indexID + 4] = vertexID + 2;
		indexes[indexID + 5] = vertexID + 3;


		colors[vertexID] = new(0, 0, 0, 1);
		colors[vertexID + 1] = new(0, 0, 0, 0);
		colors[vertexID + 2] = new(0, 0, 0, 0);
		colors[vertexID + 3] = new(0, 0, 0, 0);
	}

	public void Update()
	{
		mesh.ClearSurfaces();

		meshValues[(int)Mesh.ArrayType.Vertex] = vertices;
		meshValues[(int)Mesh.ArrayType.Index] = indexes;
		meshValues[(int)Mesh.ArrayType.Color] = colors;

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshValues);
		mesh.SurfaceSetMaterial(0, material);
	}

	public void SetColors(Color[,] colors)
	{
		int arrayIndex;
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				arrayIndex = ((y * Size.X) + x) * 4;

				this.colors[arrayIndex] = colors[x, y];
				this.colors[arrayIndex + 1] = colors[x, y];
				this.colors[arrayIndex + 2] = colors[x, y];
				this.colors[arrayIndex + 3] = colors[x, y];
			}
		}
	}

	/*public void GenerateMesh()
	{
		Array meshArray = new();
		StandardMaterial3D material = new();

		material.VertexColorUseAsAlbedo = true;
		material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;

		meshArray.Resize((int)Mesh.ArrayType.Max);

		Vector2[] vertices = new Vector2[]
		{
			new(0,0),
			new(0,1),
			new(1,1),
			new(1,0),
		};

		int[] triangles = new int[]
		{
			0,1,2,
			0,2,3
		};

		Color[] colors = new Color[]
		{
			new(1,0,0,0),
			new(0,0,0),
			new(0,0,0),
			new(0,0,0)
		};

		meshArray[(int)Mesh.ArrayType.Vertex] = vertices;
		meshArray[(int)Mesh.ArrayType.Index] = triangles;
		meshArray[(int)Mesh.ArrayType.Color] = colors;

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArray);

		mesh.SurfaceSetMaterial(0, material);

		GD.Print($"{mesh.GetSurfaceCount()}");
		mesh.ClearSurfaces();
		GD.Print($"{mesh.GetSurfaceCount()}");

		colors[1] = new(1, 0, 1);
		meshArray[(int)Mesh.ArrayType.Color] = colors;
		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArray);

		GD.Print($"{mesh.GetSurfaceCount()}");
	}*/
}
