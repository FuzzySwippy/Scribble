using Godot;

namespace Scribble.UI;

public class InfoLabel
{
	private Label label;
	public Label Label
	{
		get => label;
		set
		{
			label = value;
			Value = valueText;
		}
	}

	public string DescriptionText { get; }
	public bool DescriptionAtEnd { get; }

	private string valueText;
	public string Value
	{
		get => valueText;
		set
		{
			valueText = value;
			Label.Visible = !string.IsNullOrWhiteSpace(valueText);
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