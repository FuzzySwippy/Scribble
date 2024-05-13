using Godot;

namespace ScribbleLib;

public static class NodeExtensions
{
	public static T GetGrandChild<T>(this Node node, int generation, int repeatingIndex = 0) where T : Node
	{
		if (generation < 0)
			throw new System.ArgumentOutOfRangeException(nameof(generation), "Generation must be greater than or equal to 0");

		if (repeatingIndex < 0)
			throw new System.ArgumentOutOfRangeException(nameof(repeatingIndex), "Repeating index must be greater than or equal to 0");

		Node child = node;
		for (int i = 0; i < generation - 1; i++)
			child = child.GetChild(repeatingIndex);
		return child.GetChild<T>(repeatingIndex);
	}

	public static Node GetGrandChild(this Node node, int generation, int repeatingIndex = 0) => node.GetGrandChild<Node>(generation, repeatingIndex);
}