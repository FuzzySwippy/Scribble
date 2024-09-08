namespace Scribble.ScribbleLib.Extensions;

public static class String
{
	public static string AddSpacesBeforeCapitalLetters(this string text)
	{
		for (int i = 1; i < text.Length; i++)
			if (char.IsUpper(text[i]))
				text = text.Insert(i++, " ");
		return text;
	}
}
