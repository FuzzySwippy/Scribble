using System;
using Scribble.Application;

namespace Scribble.ScribbleLib;

public static class Try
{
	#region ReturnsAValue
	public static T Catch<T>(Func<T> func, Action<Exception> onException)
	{
		try
		{
			return func();
		}
		catch (Exception ex)
		{
			onException?.Invoke(ex);
			return default;
		}
	}

	/// <summary>
	/// Catches an exception and reports it returning a default value
	/// </summary>
	public static T Catch<T>(Func<T> func)
	{
		try
		{
			return func();
		}
		catch (Exception ex)
		{
			Main.ReportError(ex);
			return default;
		}
	}
	#endregion

	#region ReturnsVoid

	public static void Catch(Action func, Action<Exception> onException)
	{
		try
		{
			func();
		}
		catch (Exception ex)
		{
			onException?.Invoke(ex);
		}
	}

	/// <summary>
	/// Catches an exception and reports it
	/// </summary>
	public static void Catch(Action func)
	{
		try
		{
			func();
		}
		catch (Exception ex)
		{
			Main.ReportError(ex);
		}
	}
	#endregion
}