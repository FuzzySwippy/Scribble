using Godot;
using Scribble.Drawing.Tools.Properties;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Pencil;

public partial class PencilProperties : ToolProperties
{
	private OptionButton TypeOptionButton { get; set; }

	public override void _Ready()
	{
		GetControls();
		SetupControls();
	}

	private void GetControls()
	{
		TypeOptionButton = this.GetGrandChild(2).GetChild<OptionButton>(1);
	}

	private void SetupControls()
	{
		TypeOptionButton.ItemSelected += OnTypeSelected;
	}

	private void OnTypeSelected(long index) => ((PencilTool)Tool).Type = (Type)index;

	public override void UpdateProperties()
	{
		OnTypeSelected(TypeOptionButton.Selected);
	}
}
