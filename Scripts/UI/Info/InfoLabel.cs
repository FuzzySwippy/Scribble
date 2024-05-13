using Godot;

namespace Scribble;

public class InfoLabel
{
	public Label Label { get; set; }
	public string DescriptionText { get; }
	public bool DescriptionAtEnd { get; }

	string valueText;
	public string Value
	{
		get => valueText;
		set
		{
			valueText = value;
			Label.Text = DescriptionAtEnd ? $"{valueText} {DescriptionText}" : $"{DescriptionText}: {valueText}";
		}
	}

	public InfoLabel(string descriptionText, bool descriptionAtEnd = false)
	{
		DescriptionText = descriptionText;
		DescriptionAtEnd = descriptionAtEnd;
	}

	public void Set(object value) => Value = value.ToString();
}