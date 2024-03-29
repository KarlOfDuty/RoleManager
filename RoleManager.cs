using System.Reflection;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using RoleManager.Commands;

namespace RoleManager;

public class RoleManager
{
	// Sets up a dummy client to use for logging
	public static DiscordClient discordClient = new DiscordClient(new DiscordConfiguration { Token = "DUMMY_TOKEN", TokenType = TokenType.Bot, MinimumLogLevel = LogLevel.Debug });
	private static SlashCommandsExtension commands = null;

	static void Main()
	{
		Console.CancelKeyPress += delegate
		{
			discordClient.DisconnectAsync();
		};
		
		MainAsync().GetAwaiter().GetResult();
	}

	private static async Task MainAsync()
	{
		Logger.Log(LogID.GENERAL,"Starting " + Assembly.GetEntryAssembly().GetName().Name + " version " + GetVersion() + "...");
		try
		{
			Reload();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}
		catch (Exception e)
		{
			Logger.Fatal(LogID.GENERAL,"Fatal error:\n" + e);
			Console.ReadLine();
		}
	}

	public static string GetVersion()
	{
		Version version = Assembly.GetEntryAssembly()?.GetName().Version;
		return version?.Major + "." + version?.Minor + "." + version?.Build + (version?.Revision == 0 ? "" : "-" + (char)(64 + version?.Revision ?? 0));
	}
	
	public static async void Reload()
	{
		if (discordClient != null)
		{
			await discordClient.DisconnectAsync();
			discordClient.Dispose();
			Logger.Log(LogID.GENERAL, "Discord client disconnected.");
		}
		
		Logger.Log(LogID.CONFIG, "Loading config \"" + Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "config.yml\"");
		Config.LoadConfig();
		Roles.LoadRoles();
		
		// Check if token is unset
		if (Config.token == "<add-token-here>" || Config.token == "")
		{
			Logger.Fatal(LogID.CONFIG, "You need to set your bot token in the config and start the bot again.");
			throw new ArgumentException("Invalid Discord bot token");
		}

		Logger.Log(LogID.GENERAL, "Setting up Discord client...");
		
		// Checking log level
		if (!Enum.TryParse(Config.logLevel, true, out LogLevel logLevel))
		{
			Logger.Warn(LogID.CONFIG, "Log level '" + Config.logLevel + "' invalid, using 'Information' instead.");
			logLevel = LogLevel.Information;
		}
		
		// Setting up client configuration
		DiscordConfiguration cfg = new DiscordConfiguration
		{
			Token = Config.token,
			TokenType = TokenType.Bot,
			MinimumLogLevel = logLevel,
			AutoReconnect = true,
			Intents = DiscordIntents.All
		};
		
		discordClient = new DiscordClient(cfg);
		
		Logger.Log(LogID.GENERAL, "Hooking events...");
		discordClient.Ready += EventHandler.OnReady;
		discordClient.GuildAvailable += EventHandler.OnGuildAvailable;
		discordClient.ClientErrored += EventHandler.OnClientError;
		discordClient.ComponentInteractionCreated += EventHandler.OnComponentInteractionCreated;
		
		Logger.Log(LogID.GENERAL, "Registering commands...");
		commands = discordClient.UseSlashCommands();

		commands.RegisterCommands<AddRoleCommand>();
		commands.RegisterCommands<RemoveRoleCommand>();
		commands.RegisterCommands<PingCommand>();
		commands.RegisterCommands<CreateRoleSelectorCommand>();

		Logger.Log(LogID.GENERAL, "Hooking command events...");
		commands.SlashCommandErrored += EventHandler.OnCommandError;

		Logger.Log(LogID.GENERAL, "Connecting to Discord...");
		await discordClient.ConnectAsync();
	}
}