using Godot.Collections;
using Godot;

namespace Scribble.Drawing;

public partial class CanvasChunk : MeshInstance2D
{
	private readonly ArrayMesh mesh;
	private Array meshValues;
	private Vector2[] vertices;
	private Vector2[] uvs;
	private int[] indexes;
	private Color[] colors;

	private MeshDataTool MeshDataTool { get; set; }

	public Vector2I PixelPosition { get; private set; }
	public Vector2I SizeInPixels { get; private set; }

	public bool MarkedForUpdate { get; set; }

	public CanvasChunk()
	{
		mesh = new();
		Mesh = mesh;
		MeshDataTool = new();
	}

	public override void _Ready() =>
		RenderingServer.CanvasItemSetCustomRect(GetCanvasItem(), true,
			new Rect2(Vector2.Zero, Vector2.One * Canvas.BaseScale));

	public void Init(Vector2I position, Vector2I size)
	{
		Position = position;
		PixelPosition = position;
		SizeInPixels = size;

		Generate();
		Show();
	}

	private void Generate()
	{
		vertices = new Vector2[SizeInPixels.X * SizeInPixels.Y * 4];
		indexes = new int[SizeInPixels.X * SizeInPixels.Y * 6];
		colors = new Color[SizeInPixels.X * SizeInPixels.Y * 4];
		uvs = new Vector2[SizeInPixels.X * SizeInPixels.Y * 4];

		int arrayIndex;
		for (int x = 0; x < SizeInPixels.X; x++)
		{
			for (int y = 0; y < SizeInPixels.Y; y++)
			{
				arrayIndex = (y * SizeInPixels.X) + x;
				AddPixel(arrayIndex * 4, arrayIndex * 6, x, y);
			}
		}

		InitializeMesh();
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

		uvs[vertexID] = new(x, y);
		uvs[vertexID + 1] = new(x, y + 1);
		uvs[vertexID + 2] = new(x + 1, y + 1);
		uvs[vertexID + 3] = new(x + 1, y);

		colors[vertexID] = new(0, 0, 0, 0);
		colors[vertexID + 1] = new(0, 0, 0, 0);
		colors[vertexID + 2] = new(0, 0, 0, 0);
		colors[vertexID + 3] = new(0, 0, 0, 0);
	}

	private void InitializeMesh()
	{
		if (meshValues == null)
		{
			meshValues = new();
			meshValues.Resize((int)Mesh.ArrayType.Max);
		}

		mesh.ClearSurfaces();

		meshValues[(int)Mesh.ArrayType.Vertex] = vertices;
		meshValues[(int)Mesh.ArrayType.Index] = indexes;
		meshValues[(int)Mesh.ArrayType.Color] = colors;
		meshValues[(int)Mesh.ArrayType.TexUV] = uvs;

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshValues);
		mesh.SurfaceSetMaterial(0, Material);

		MeshDataTool.CreateFromSurface(mesh, 0);
		meshValues.Dispose();
		meshValues = null;

		MarkedForUpdate = false;
	}

	public void UpdateMesh()
	{
		mesh.ClearSurfaces();
		MeshDataTool.CommitToSurface(mesh);

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

				MeshDataTool.SetVertexColor(arrayIndex, colors[colorPosition.X, colorPosition.Y]);
				MeshDataTool.SetVertexColor(arrayIndex + 1, colors[colorPosition.X, colorPosition.Y]);
				MeshDataTool.SetVertexColor(arrayIndex + 2, colors[colorPosition.X, colorPosition.Y]);
				MeshDataTool.SetVertexColor(arrayIndex + 3, colors[colorPosition.X, colorPosition.Y]);
			}
		}
	}

	public void Clear()
	{
		MarkedForUpdate = false;

		vertices = null;
		indexes = null;
		colors = null;
		Hide();
	}
}
