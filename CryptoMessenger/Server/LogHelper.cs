using System.Runtime.CompilerServices;

namespace Server
{
	/// <summary>
	/// Help methods for logger.
	/// </summary>
	class LogHelper
	{
		/// <summary>
		/// Get instanse of logger for specific file.
		/// </summary>
		/// <param name="name">name of file (automatic parameter).</param>
		/// <returns>logger.</returns>
		public static log4net.ILog GetLogger([CallerFilePath]string name = "")
		{
			return log4net.LogManager.GetLogger(name);
		}
	}
}
