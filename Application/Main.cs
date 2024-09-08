using System;
using Godot;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;
using Scribble.UI;
using System.Diagnostics;

namespace Scribble.Application;

public partial class Main : Node2D
{
	public static Vector2I BaseWindowSize { get; } = new(1920, 1080);

	public static Godot.Window Window { get; private set; }

	public static Rect2 ViewportRect { get; private set; }

	public static Artist Artist { get; private set; }

	public static new event Action Ready;
	public static event Action WindowSizeChanged;
	public static event Action WindowFocusEntered;

	//Checking for unsaved changes
	private Action PendingSaveAction { get; set; }

	//Frame time
	private Stopwatch FrameTimeStopwatch { get; } = new();
	public static double FrameTimeMs => Global.Main.FrameTimeStopwatch.Elapsed.TotalMilliseconds;
	public const double TargetFrameTimeMs = 1000.0 / 60.0;

	public override void _Ready()
	{
		Global.Main = this;
		GetTree().AutoAcceptQuit = false; //Dont quit immediately when closing window

		Window = GetWindow();
		Window.MinSize = new Vector2I(800, 500);

		Artist = new();

		Ready?.Invoke();
		Window.SizeChanged += WindowSizeChangeHandler;
		Window.FocusEntered += () => WindowFocusEntered?.Invoke();

		WindowSizeChangeHandler();

		Global.Canvas.Init(Canvas.DefaultResolution.ToVector2I(), Artist);
		Global.FileDialogs.DialogCanceledEvent += FileDialogCanceled;
		Global.FileDialogs.FileSelectedEvent += FileDialogFileSelected;

		FrameTimeStopwatch.Start();

		GD.Print("Main Ready");
	}

	public override void _Process(double delta) => FrameTimeStopwatch.Restart();

	public override void _Notification(int what)
	{
		//Handles window close request
		if (what == NotificationWMCloseRequest)
			CheckUnsavedChanges(Quit);
	}

	private void WindowSizeChangeHandler()
	{
		ViewportRect = GetViewportRect();
		WindowSizeChanged?.Invoke();
	}

	public static Modal ReportError(string message, Exception exception = null)
	{
		string error = "";
		if (string.IsNullOrWhiteSpace(message))
			error = exception.Message;
		else if (exception == null)
			error = message;
		else
			error = $"{message}:{System.Environment.NewLine}{exception.Message}";

		GD.PrintErr(error);
		Global.QuickInfo.Set(error);
		return WindowManager.ShowErrorModal(error);
	}

	public static void ReportError(Exception exception) => ReportError(null, exception);

	public static void CheckUnsavedChanges(Action action)
	{
		if (Global.Canvas.HasUnsavedChanges)
			WindowManager.ShowUnsavedChangeModal(() =>
			{
				if (!Canvas.SaveToPreviousPath())
				{
					Global.Main.PendingSaveAction = action;
					return;
				}

				action?.Invoke();
			}, action, null);
		else
			action?.Invoke();
	}

	private void FileDialogCanceled(FileDialogType type)
	{
		if (type == FileDialogType.Open)
			return;

		PendingSaveAction = null;
	}

	private void FileDialogFileSelected(FileDialogType type, string _, object[] __)
	{
		if (type == FileDialogType.Open)
			return;

		PendingSaveAction?.Invoke();
		PendingSaveAction = null;
	}

	public static void Quit() => Global.Main.QuitInternal();
	private void QuitInternal() => GetTree().Quit();
}
