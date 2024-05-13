namespace ScribbleLib;

public delegate void LoopAction2(int x, int y);
public delegate void LoopAction3(int x, int y, int z);
public delegate void LoopAction4(int x, int y, int z, int w);

public static class For
{
	public static void Loop2(int lengthX, int lengthY, LoopAction2 action)
	{
		for (int x = 0; x < lengthX; x++)
			for (int y = 0; y < lengthY; y++)
				action(x, y);
	}

	public static void Loop3(int lengthX, int lengthY, int lengthZ, LoopAction3 action)
	{
		for (int x = 0; x < lengthX; x++)
			for (int y = 0; y < lengthY; y++)
				for (int z = 0; z < lengthZ; z++)
					action(x, y, z);
	}

	public static void Loop4(int lengthX, int lengthY, int lengthZ, int lengthW, LoopAction4 action)
	{
		for (int x = 0; x < lengthX; x++)
			for (int y = 0; y < lengthY; y++)
				for (int z = 0; z < lengthZ; z++)
					for (int w = 0; w < lengthW; w++)
						action(x, y, z, w);
	}
}