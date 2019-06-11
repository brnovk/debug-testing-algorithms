using DebugConvertRusNamesToDativeCase.Util;

namespace DebugConvertRusNamesToDativeCase
{
	/// <summary>
	/// Application launch class
	/// </summary>
	/// <inheritdoc />
	public partial class Runner
	{
		private Runner()
		{
			// Handling uncaught exceptions
			Dispatcher.UnhandledException += Common.RootExceptionHandler;
		}
	}
}