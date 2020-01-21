namespace Framework.Utils
{
	public static class LoggingUtilities
	{
		/// <summary>
		///		Logs a message into the 
		///		UnityEngine.Debug.Log(),
		///		System.Diagnostics.Debug.Writeline(), 
		///		System.Console.Writeline()
		/// </summary>
		public static void Log(string message)
		{
#if UNITY_EDITOR
			UnityEngine.Debug.Log(message);
#endif
			System.Diagnostics.Debug.WriteLine(message);
			System.Console.WriteLine(message);
		}

		/// <summary>
		///		Logs a message into the 
		///		UnityEngine.Debug.Log(),
		///		System.Diagnostics.Debug.Writeline(), 
		///		System.Console.Writeline()
		/// </summary>
		public static void Log(object obj)
		{
			Log(obj.ToString());
		}

		/// <summary>
		///		Logs a message into the 
		///		UnityEngine.Debug.Log(),
		///		System.Diagnostics.Debug.Writeline(), 
		///		System.Console.Writeline()
		/// </summary>
		public static void LogFormat(string message, params object[] obj)
		{
			string msg = string.Format(message, obj);
			Log(msg);
		}
	}
}
