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
			SetLabelValue();
		}
	}

	public string DescriptionText { get; }
	public bool DescriptionAtEnd { get; } = false;
	public string Units { get; set; }

	public string Value { get; private set; }
	public bool ValueChanged { get; private set; }

	public InfoLabel(string descriptionText, bool descriptionAtEnd = false)
	{
		DescriptionText = descriptionText;
		DescriptionAtEnd = descriptionAtEnd;
	}

	public InfoLabel(string descriptionText, string units)
	{
		DescriptionText = descriptionText;
		Units = units;
	}

	public void Set(object value)
	{
		Value = value.ToString();
		ValueChanged = true;
	}

	public void SetLabelValue()
	{
		Label.Visible = !string.IsNullOrWhiteSpace(Value);
		Label.Text = DescriptionAtEnd ? $"{Value} {DescriptionText}" : $"{DescriptionText}: {Value}{Units}";
		ValueChanged = false;
	}
}