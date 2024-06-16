using System;
using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class FileDialogs : Node
{
	private FileDialog OpenFileDialog { get; set; }
	private FileDialog SaveFileDialog { get; set; }
	private FileDialog ExportFileDialog { get; set; }

	public event Action<FileDialogType, string> FileSelectedEvent;
	public event Action<FileDialogType> DialogCanceledEvent;

	private string SaveAndExportFileName { get; set; }

	public override void _Ready()
	{
		Global.FileDialogs = this;

		OpenFileDialog = GetChild<FileDialog>(0);
		SaveFileDialog = GetChild<FileDialog>(1);
		ExportFileDialog = GetChild<FileDialog>(2);

		OpenFileDialog.Filters = new[]
			{ "*.scrbl, *.png, *.jpg, *.jpeg, *.webp ; Images",
			"*.scrbl ; Scribble Image",
			"*.png ; PNG", "*.jpg ; JPG", "*.jpeg ; JPEG", "*.webp ; WEBP" };
		SaveFileDialog.Filters = new[] { "*.scrbl ; Scribble Image" };
		ExportFileDialog.Filters = new[]
			{ "*.png ; PNG", "*.jpg ; JPG", "*.jpeg ; JPEG", "*.webp ; WEBP" };

		OpenFileDialog.Canceled += () => DialogCanceled(FileDialogType.Open);
		SaveFileDialog.Canceled += () => DialogCanceled(FileDialogType.Save);
		ExportFileDialog.Canceled += () => DialogCanceled(FileDialogType.Export);

		OpenFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Open, file);
		SaveFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Save, file);
		ExportFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Export, file);
	}

	public static void Show(FileDialogType type) => Global.FileDialogs.ShowInternal(type);

	private void ShowInternal(FileDialogType type)
	{
		Global.InteractionBlocker.Show();
		switch (type)
		{
			case FileDialogType.Open:
				OpenFileDialog.PopupCentered();
				break;
			case FileDialogType.Save:
				SaveFileDialog.PopupCentered();
				break;
			case FileDialogType.Export:
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
			FileSelectedEvent?.Invoke(type, file);
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
