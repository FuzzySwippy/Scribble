using ScribbleLib;

namespace Scribble;

public enum QuickPencilType
{
	Primary,
	Secondary,
	AltPrimary,
	AltSecondary
}

public class Brush
{
	private int size = 1;
	public int Size
	{
		get => size;
		set
		{
			size = value;
			if (size < 1)
				size = 1;
			else if (size > 100)
				size = 100;

			Status.Set("brush_size", size);
		}
	}

	private readonly ScribbleColor[] quickPencilColors = new ScribbleColor[4];

	public Brush()
	{
		for (int i = 0; i < 4; i++)
			quickPencilColors[i] = new(1, 1, 1);

		//Set default colors
		GetQuickPencilColor(QuickPencilType.Secondary).SetRGBA(0, 0, 0, 0);
		GetQuickPencilColor(QuickPencilType.AltPrimary).SetRGB(0, 0, 0);
		GetQuickPencilColor(QuickPencilType.AltSecondary).SetRGB(0, 0, 1);

		Status.Set("brush_size", size);
	}

	public ScribbleColor GetQuickPencilColor(QuickPencilType type) => quickPencilColors[(int)type];

	/*public void Pencil(Vector2I pos, Color color)
	{
		if (Size == 1)
		{
			canvas.SetPixel(pos, color, true);
			return;
		}

		int sizeAdd = Size / 2;
		for (int x = pos.X - sizeAdd; x <= pos.X + sizeAdd; x++)
		{
			for (int y = pos.Y - sizeAdd; y <= pos.Y + sizeAdd; y++)
			{
				if (pos.ToVector2().DistanceTo(new(x, y)) <= (float)Size / 2)
					canvas.SetPixel(new(x, y), color, false);
			}
		}
		canvas.UpdateMesh();
	}

	public void Line(Vector2 pos1, Vector2 pos2, Color color)
	{
		if (Size == 1)
		{
			while (pos1 != pos2)
			{
				canvas.SetPixel(pos1.ToVector2I(), color, false);
				pos1 = pos1.MoveToward(pos2, 1);
			}
			canvas.SetPixel(pos2.ToVector2I(), color, false);

			canvas.UpdateMesh();
			return;
		}

		float sizeAdd = (float)Size / 2;
		Vector2I point1 = new Vector2(pos1.X < pos2.X ? pos1.X : pos2.X, pos1.Y < pos2.Y ? pos1.Y : pos2.Y).ToVector2I() - Size.ToVector2I();
		Vector2I point2 = new Vector2(pos1.X > pos2.X ? pos1.X : pos2.X, pos1.Y > pos2.Y ? pos1.Y : pos2.Y).ToVector2I() + Size.ToVector2I();

		for (int x = point1.X; x <= point2.X; x++)
		{
			for (int y = point1.Y; y <= point2.Y; y++)
			{
				if (new Vector2(x, y).DistanceToLine(pos1, pos2) <= sizeAdd)
					canvas.SetPixel(new(x, y), color, false);
			}
		}
		canvas.UpdateMesh();
	}*/
}
