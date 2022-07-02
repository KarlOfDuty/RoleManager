using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;

namespace RoleManager
{
	internal static class EventHandler
	{
		internal static Task OnReady(DiscordClient client, ReadyEventArgs e)
		{
			Logger.Log(LogID.DISCORD, "Client is ready to process events.");

			// Checking activity type
			if (!Enum.TryParse(Config.presenceType, true, out ActivityType activityType))
			{
				Logger.Log(LogID.CONFIG, "Presence type '" + Config.presenceType + "' invalid, using 'Playing' instead.");
				activityType = ActivityType.Playing;
			}

			client.UpdateStatusAsync(new DiscordActivity(Config.presenceText, activityType), UserStatus.Online);
			return Task.CompletedTask;
		}

		internal static Task OnGuildAvailable(DiscordClient _, GuildCreateEventArgs e)
		{
			Logger.Log(LogID.DISCORD, "Guild available: " + e.Guild.Name);

			IReadOnlyDictionary<ulong, DiscordRole> roles = e.Guild.Roles;

			foreach ((ulong roleID, DiscordRole role) in roles)
			{
				Logger.Log(LogID.DISCORD, role.Name.PadRight(40, '.') + roleID);
			}
			return Task.CompletedTask;
		}

		internal static Task OnClientError(DiscordClient _, ClientErrorEventArgs e)
		{
			Logger.Error(LogID.DISCORD, "Exception occured:\n" + e.Exception);
			return Task.CompletedTask;
		}

		internal static Task OnCommandError(SlashCommandsExtension commandSystem, SlashCommandErrorEventArgs e)
		{
			switch (e.Exception)
			{
				case SlashExecutionChecksFailedException checksFailedException:
					{
						foreach (SlashCheckBaseAttribute attr in checksFailedException.FailedChecks)
						{
							DiscordEmbed error = new DiscordEmbedBuilder
							{
								Color = DiscordColor.Red,
								Description = ParseFailedCheck(attr)
							};
							e.Context?.Channel?.SendMessageAsync(error);
						}
						return Task.CompletedTask;
					}

				default:
					{
						Logger.Error(LogID.COMMAND, "Exception occured: " + e.Exception.GetType() + ": " + e.Exception);
						DiscordEmbed error = new DiscordEmbedBuilder
						{
							Color = DiscordColor.Red,
							Description = "Internal error occured, please report this to the developer."
						};
						e.Context?.Channel?.SendMessageAsync(error);
						return Task.CompletedTask;
					}
			}
		}

		private static string ParseFailedCheck(SlashCheckBaseAttribute attr)
		{
			switch (attr)
			{
				case SlashRequireDirectMessageAttribute _:
					return "This command can only be used in direct messages!";
				case SlashRequireOwnerAttribute _:
					return "Only the server owner can use that command!";
				case SlashRequirePermissionsAttribute _:
					return "You don't have permission to do that!";
				case SlashRequireBotPermissionsAttribute _:
					return "The bot doesn't have the required permissions to do that!";
				case SlashRequireUserPermissionsAttribute _:
					return "You don't have permission to do that!";
				case SlashRequireGuildAttribute _:
					return "This command has to be used in a Discord server!";
				default:
					return "Unknown Discord API error occured, please try again later.";
			}
		}
	}
}
