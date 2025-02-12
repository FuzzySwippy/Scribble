using System;
using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class FileDialogs : Node
{
	private FileDialog OpenFileDialog { get; set; }
	private FileDialog SaveFileDialog { get; set; }
	private FileDialog ExportFileDialog { get; set; }

	public event Action<FileDialogType, string, object[]> FileSelectedEvent;
	public event Action<FileDialogType> DialogCanceledEvent;

	private string SaveAndExportFileName { get; set; }
	private object[] AdditionalData { get; set; }

	public override void _Ready()
	{
		Global.FileDialogs = this;

		OpenFileDialog = GetChild<FileDialog>(0);
		SaveFileDialog = GetChild<FileDialog>(1);
		ExportFileDialog = GetChild<FileDialog>(2);

		OpenFileDialog.Filters = [ "*.scrbl, *.png, *.jpg, *.jpeg, *.webp, *.bmp, *.gif, *.apng ; Images",
			"*.scrbl ; Scribble Image",
			"*.png ; PNG", "*.jpg ; JPG", "*.jpeg ; JPEG", "*.webp ; WEBP", "*.bmp ; BMP", "*.gif ; GIF", "*.apng ; APNG" ];
		SaveFileDialog.Filters = ["*.scrbl ; Scribble Image"];
		ExportFileDialog.Filters = ["*.png ; PNG", "*.jpg ; JPG", "*.jpeg ; JPEG", "*.webp ; WEBP", "*.bmp ; BMP", "*.gif ; GIF", "*.apng ; APNG"];

		OpenFileDialog.Canceled += () => DialogCanceled(FileDialogType.Open);
		SaveFileDialog.Canceled += () => DialogCanceled(FileDialogType.Save);
		ExportFileDialog.Canceled += () => DialogCanceled(FileDialogType.Export);

		OpenFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Open, file);
		SaveFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Save, file);
		ExportFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Export, file);
	}

	public static void Show(FileDialogType type, params object[] additionalData) =>
		Global.FileDialogs.ShowInternal(type, additionalData);

	private void ShowInternal(FileDialogType type, params object[] additionalData)
	{
		Global.InteractionBlocker.Show();
		AdditionalData = additionalData;
		switch (type)
		{
			case FileDialogType.Open:
				OpenFileDialog.CurrentPath = Global.Canvas.SaveDirectoryPath;
				OpenFileDialog.PopupCentered();
				break;
			case FileDialogType.Save:
				SaveFileDialog.CurrentPath = Global.Canvas.FilePath;
				SaveFileDialog.PopupCentered();
				break;
			case FileDialogType.Export:
				ExportFileDialog.CurrentPath = Global.Canvas.FilePath;
				ExportFileDialog.PopupCentered();
				break;
		}
	}

	private void DialogCanceled(FileDialogType type)
	{
		Global.InteractionBlocker.Hide();
		DialogCanceledEvent?.Invoke(type);

		SaveFileDialog.CurrentFile = SaveAndExportFileName;
		ExportFileDialog.CurrentFile = SaveAndExportFileName;
	}

	private void FileSelectedInternal(FileDialogType type, string file)
	{
		try
		{
			FileSelectedEvent?.Invoke(type, file, AdditionalData);
		}
		catch (Exception ex)
		{
			Main.ReportError(
				$"An error occurred while {(type == FileDialogType.Open ? "opening" : "saving")} file", ex);
		}
		Global.InteractionBlocker.Hide();
	}

	public void SetSaveAndExportFileName(string name)
	{
		SaveAndExportFileName = name;

		SaveFileDialog.CurrentFile = SaveAndExportFileName;
		ExportFileDialog.CurrentFile = SaveAndExportFileName;
	}
}
