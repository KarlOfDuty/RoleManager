using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
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
						e.Context.CreateResponseAsync(error);
					}
					return Task.CompletedTask;
				}
				default:
				{
					Logger.Error(LogID.COMMAND, "Exception occured: " + e.Exception.GetType() + ": " + e.Exception);
					if (e.Exception is UnauthorizedException ex)
					{
						Logger.Error(LogID.DISCORD, ex.WebResponse.Response);
					}
					
					DiscordEmbed error = new DiscordEmbedBuilder
					{
						Color = DiscordColor.Red,
						Description = "Internal error occured, please report this to the developer."
					};
					e.Context.CreateResponseAsync(error);
					return Task.CompletedTask;
				}
			}
		}

		internal static async Task OnComponentInteractionCreated(DiscordClient client, ComponentInteractionCreateEventArgs e)
		{
			try
			{
				switch (e.Interaction.Data.ComponentType)
				{
					case ComponentType.Select:
						if (!e.Interaction.Data.CustomId.StartsWith("rolemanager_togglerole"))
						{
							return;
						}

						if (e.Interaction.Data.Values.Length == 0)
						{
							await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(e.Message.Content).AddComponents(e.Message.Components));
						}
						
						foreach (string stringID in e.Interaction.Data.Values)
						{
							if (!ulong.TryParse(stringID, out ulong roleID) || roleID == 0) continue;

							DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
							if (!e.Guild.Roles.ContainsKey(roleID) || member == null) continue;

							if (member.Roles.Any(role => role.Id == roleID))
							{
								await member.RevokeRoleAsync(e.Guild.Roles[roleID]);
								await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder
								{
									Color = DiscordColor.Green,
									Description = "Revoked role " + e.Guild.Roles[roleID].Mention + "!"
								}).AsEphemeral());
							}
							else
							{
								await member.GrantRoleAsync(e.Guild.Roles[roleID]);
								await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder
								{
									Color = DiscordColor.Green,
									Description = "Granted role " + e.Guild.Roles[roleID].Mention + "!"
								}).AsEphemeral());
							}
						}
						break;

					case ComponentType.ActionRow:
					case ComponentType.Button:
					case ComponentType.FormInput:
						return;
				}
			}
			catch (UnauthorizedException ex)
			{
				await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder
				{
					Color = DiscordColor.Red,
					Description = "The bot doesn't have the required permissions to do that!"
				}).AsEphemeral());
			}
			catch (Exception ex)
			{
				Logger.Error(LogID.COMMAND, "Exception occured: " + ex.GetType() + ": " + ex);
				await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed( new DiscordEmbedBuilder
                {
                	Color = DiscordColor.Red,
                	Description = "Internal interaction error occured, please report this to the developer."
                }).AsEphemeral());
			}
		}
		
		private static string ParseFailedCheck(SlashCheckBaseAttribute attr)
		{
			return attr switch
			{
				SlashRequireDirectMessageAttribute _ => "This command can only be used in direct messages!",
				SlashRequireOwnerAttribute _ => "Only the server owner can use that command!",
				SlashRequirePermissionsAttribute _ => "You don't have permission to do that!",
				SlashRequireBotPermissionsAttribute _ => "The bot doesn't have the required permissions to do that!",
				SlashRequireUserPermissionsAttribute _ => "You don't have permission to do that!",
				SlashRequireGuildAttribute _ => "This command has to be used in a Discord server!",
				_ => "Unknown Discord API error occured, please try again later."
			};
		}
	}
}
