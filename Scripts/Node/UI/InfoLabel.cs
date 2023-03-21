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
            if (DescriptionAtEnd)
                Label.Text = $"{valueText} {DescriptionText}";
            else
                Label.Text = $"{DescriptionText}: {valueText}";
        }
    }

    public InfoLabel(string descriptionText, bool descriptionAtEnd = false)
    {
        DescriptionText = descriptionText;
        DescriptionAtEnd = descriptionAtEnd;
    }

    public void Set(object value) => Value = value.ToString();
}