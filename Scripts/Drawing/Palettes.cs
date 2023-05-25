using System.Collections.Generic;
using System.Linq;
using ScribbleLib;

namespace Scribble;

public class Palettes
{
	List<Palette> PaletteList { get; }

	public int Count => PaletteList.Count;

	public bool MarkedForSave { get; private set; }


	public Palette this[int index] => PaletteList.InRange(index) ? PaletteList[index] : null;
	public bool InRange(int index) => PaletteList.InRange(index);


	public Palettes() => PaletteList = FileManager.LoadPalettes();

	public void Save()
	{
		FileManager.SavePalettes(PaletteList);
		MarkedForSave = false;
	}

	public void MarkForSave() => MarkedForSave = true;

	public void Add(Palette palette)
	{
		PaletteList.Insert(0, palette);
		Save();
	}

	public void RemoveAt(int index)
	{
		PaletteList.RemoveAt(index);
		Save();
	}

	public bool Contains(string paletteName) => PaletteList.Any(p => p.Name == paletteName);

	public int IndexOf(Palette palette) => PaletteList.IndexOf(palette);
}
