using Godot;
using ScribbleLib;
using ScribbleLib.Input;

namespace Scribble;

public partial class PaletteEditor : Node
{
	ItemList paletteList;
	ColorInput colorInput;

	//New palette
	LineEdit newPaletteNameInput;
	Button addPaletteButton;


	//Current palette
	LineEdit currentPaletteNameInput;
	Button deletePaletteButton;
	Control noPaletteSelectedControl;

	public override void _Ready()
	{
        GetControls();
    }

	void GetControls()
	{
        paletteList = this.GetGrandChild<ItemList>(6);
        colorInput = GetChild(0).GetChild(1).GetChild<ColorInput>(0);

		//New palette
		Node parent = this.GetGrandChild(2).GetChild(1).GetGrandChild(2);
        newPaletteNameInput = parent.GetChild<LineEdit>(0);
		addPaletteButton = parent.GetChild<Button>(1);

		//Current palette
        parent = GetChild(0).GetGrandChild(2, 1).GetChild(0);
		noPaletteSelectedControl = parent.GetChild<Control>(1);

		parent = parent.GetGrandChild(2);
        currentPaletteNameInput = parent.GetChild<LineEdit>(0);
		deletePaletteButton = parent.GetChild(2).GetChild<Button>(0);
	}
}
