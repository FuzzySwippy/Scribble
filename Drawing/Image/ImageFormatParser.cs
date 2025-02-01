namespace Scribble.Drawing;

public static class ImageFormatParser
{
	public static ImageFormat FileExtensionToImageFormat(string extension) => extension switch
	{
		".scrbl" => ImageFormat.SCRIBBLE,
		".png" => ImageFormat.PNG,
		".jpg" => ImageFormat.JPEG,
		".jpeg" => ImageFormat.JPEG,
		".webp" => ImageFormat.WEBP,
		".bmp" => ImageFormat.BMP,
		".gif" => ImageFormat.GIF,
		_ => ImageFormat.Invalid
	};
}
