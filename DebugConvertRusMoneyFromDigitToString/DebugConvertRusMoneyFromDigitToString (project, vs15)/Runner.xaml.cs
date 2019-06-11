using DebugConvertRusMoneyFromDigitToString.Util;

namespace DebugConvertRusMoneyFromDigitToString
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
