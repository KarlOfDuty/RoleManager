using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class ListRolesCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[Config.ConfigPermissionCheckAttribute("listroles")]
	[SlashCommand("listroles", "Lists joinable roles")]
	public async Task OnExecute(InteractionContext command)
	{
		if (Roles.savedRoles.Count == 0)
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Description = "There are no registered roles."
			});
			return;
		}
		
		DiscordGuild guild = await RoleManager.discordClient.GetGuildAsync(Config.serverID);

		List<string> listRows = new List<string>();
		foreach (ulong savedRole in Roles.savedRoles)
		{
			if (guild.Roles.ContainsKey(savedRole))
			{
				listRows.Add(guild.Roles[savedRole].Mention + "\n");
			}
		}

		LinkedList<string> messageStrings = ParseListIntoMessages(listRows);

		foreach (string messageString in messageStrings)
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Title = "Joinable roles: ",
				Color = DiscordColor.Green,
				Description = messageString
			});
		}
	}
	
	public static LinkedList<string> ParseListIntoMessages(List<string> listItems)
	{ 
		LinkedList<string> messages = new LinkedList<string>();

		foreach (string listItem in listItems)
		{
			if (messages.Last?.Value?.Length + listItem?.Length < 2048)
			{
				messages.Last.Value += listItem;
			}
			else
			{
				messages.AddLast(listItem);
			}
		}

		return messages;
	}
}