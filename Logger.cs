using Microsoft.Extensions.Logging;

namespace RoleManager
{
	public enum LogID
	{
		GENERAL,
		CONFIG,
		COMMAND,
		DISCORD,
	};

	public static class Logger
	{
		private static Dictionary<LogID, EventId> eventIDs = new Dictionary<LogID, EventId>
		{
			{ LogID.GENERAL,  new EventId(500, "General")  },
			{ LogID.CONFIG,   new EventId(501, "Config")   },
			{ LogID.COMMAND,  new EventId(502, "Command")  },
			{ LogID.DISCORD,  new EventId(503, "Discord")  },
		};

		public static void Debug(LogID logID, string Message)
		{
			try
			{
				RoleManager.discordClient.Logger.Log(LogLevel.Debug, eventIDs[logID], Message);
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("[DEBUG] " + Message);
			}
		}

		public static void Log(LogID logID, string Message)
		{
			try
			{
				RoleManager.discordClient.Logger.Log(LogLevel.Information, eventIDs[logID], Message);
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("[INFO] " + Message);
			}
		}

		public static void Warn(LogID logID, string Message)
		{
			try
			{
				RoleManager.discordClient.Logger.Log(LogLevel.Warning, eventIDs[logID], Message);
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("[WARNING] " + Message);
			}
		}

		public static void Error(LogID logID, string Message)
		{
			try
			{
				RoleManager.discordClient.Logger.Log(LogLevel.Error, eventIDs[logID], Message);
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("[ERROR] " + Message);
			}
		}

		public static void Fatal(LogID logID, string Message)
		{
			try
			{
				RoleManager.discordClient.Logger.Log(LogLevel.Critical, eventIDs[logID], Message);
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("[CRITICAL] " + Message);
			}
		}
	}
}