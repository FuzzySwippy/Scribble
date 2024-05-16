using Godot.Collections;
using Godot;

namespace Scribble.Drawing;

public partial class CanvasChunk : MeshInstance2D
{
	private readonly ArrayMesh mesh;
	private Array meshValues;
	private StandardMaterial3D material;
	private Vector2[] vertices;
	private int[] indexes;
	private Color[] colors;

	public Vector2I PixelPosition { get; private set; }
	public Vector2I SizeInPixels { get; private set; }

	public bool MarkedForUpdate { get; set; }

	public CanvasChunk()
	{
		mesh = new();
		Mesh = mesh;
	}

	public void Init(Vector2I position, Vector2I size)
	{
		Position = position;
		PixelPosition = position;
		SizeInPixels = size;

		Generate();
	}

	private void Generate()
	{
		if (meshValues == null)
		{
			meshValues = new();
			meshValues.Resize((int)Mesh.ArrayType.Max);
		}

		vertices = new Vector2[SizeInPixels.X * SizeInPixels.Y * 4];
		indexes = new int[SizeInPixels.X * SizeInPixels.Y * 6];
		colors = new Color[SizeInPixels.X * SizeInPixels.Y * 4];

		int arrayIndex;
		for (int x = 0; x < SizeInPixels.X; x++)
		{
			for (int y = 0; y < SizeInPixels.Y; y++)
			{
				arrayIndex = (y * SizeInPixels.X) + x;
				AddPixel(arrayIndex * 4, arrayIndex * 6, x, y);
			}
		}

		material ??= new()
		{
			VertexColorUseAsAlbedo = true,
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
		};

		UpdateMesh();
	}

	private void AddPixel(int vertexID, int indexID, int x, int y)
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


		colors[vertexID] = new(0, 0, 0, 0);
		colors[vertexID + 1] = new(0, 0, 0, 0);
		colors[vertexID + 2] = new(0, 0, 0, 0);
		colors[vertexID + 3] = new(0, 0, 0, 0);
	}

	public void UpdateMesh()
	{
		mesh.ClearSurfaces();

		meshValues[(int)Mesh.ArrayType.Vertex] = vertices;
		meshValues[(int)Mesh.ArrayType.Index] = indexes;
		meshValues[(int)Mesh.ArrayType.Color] = colors;

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshValues);
		mesh.SurfaceSetMaterial(0, material);

		MarkedForUpdate = false;
	}

	public void SetColors(Color[,] colors)
	{
		for (int x = 0; x < SizeInPixels.X; x++)
		{
			for (int y = 0; y < SizeInPixels.Y; y++)
			{
				int arrayIndex = ((y * SizeInPixels.X) + x) * 4;
				Vector2I colorPosition = PixelPosition + new Vector2I(x, y);

				this.colors[arrayIndex] = colors[colorPosition.X, colorPosition.Y];
				this.colors[arrayIndex + 1] = colors[colorPosition.X, colorPosition.Y];
				this.colors[arrayIndex + 2] = colors[colorPosition.X, colorPosition.Y];
				this.colors[arrayIndex + 3] = colors[colorPosition.X, colorPosition.Y];
			}
		}
	}

	public void Clear()
	{
		MarkedForUpdate = false;

		vertices = null;
		indexes = null;
		colors = null;
	}
}
