using System;
using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class FileDialogs : Node
{
	private FileDialog SaveFileDialog { get; set; }
	private FileDialog OpenFileDialog { get; set; }

	public event Action<FileDialogType, string> FileSelected;

	public override void _Ready()
	{
		Global.FileDialogs = this;

		SaveFileDialog = GetChild<FileDialog>(0);
		OpenFileDialog = GetChild<FileDialog>(1);

		SaveFileDialog.Filters = new[] { "*.scrbl ; Scribble Images" };
		OpenFileDialog.Filters = new[] { "*.scrbl ; Scribble Images" };

		SaveFileDialog.Canceled += DialogCanceled;
		OpenFileDialog.Canceled += DialogCanceled;

		SaveFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Save, file);
		OpenFileDialog.FileSelected += file => FileSelectedInternal(FileDialogType.Open, file);
	}

	public static void Show(FileDialogType type) => Global.FileDialogs.ShowInternal(type);

	private void ShowInternal(FileDialogType type)
	{
		Global.InteractionBlocker.Show();
		switch (type)
		{
			case FileDialogType.Save:
				SaveFileDialog.PopupCentered();
				break;
			case FileDialogType.Open:
				OpenFileDialog.PopupCentered();
				break;
		}
	}

	private void DialogCanceled() => Global.InteractionBlocker.Hide();

	private void FileSelectedInternal(FileDialogType type, string file)
	{
		try
		{
			FileSelected?.Invoke(type, file);
		}
		catch (Exception ex)
		{
			WindowManager.ShowErrorModal(
				$"An error occurred while {(type == FileDialogType.Save ? "saving" : "opening")} file",
				ex);
		}
		Global.InteractionBlocker.Hide();
	}
}
