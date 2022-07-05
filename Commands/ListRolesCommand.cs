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
			}, true);
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

		DiscordMessageBuilder builder = new DiscordMessageBuilder().WithContent("Use this to join or leave public roles:");

		foreach (DiscordSelectComponent component in await GetSelectComponents())
		{
			builder.AddComponents(component);
		}
		
		await command.Channel.SendMessageAsync(builder);
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

	// TODO: Refactor this mess of a function
	public static async Task<List<DiscordSelectComponent>> GetSelectComponents()
	{
		DiscordGuild guild = await RoleManager.discordClient.GetGuildAsync(Config.serverID);
		
		List<DiscordRole> savedRoles = guild.Roles.Where(rolePair => Roles.savedRoles.Contains(rolePair.Key))
			.Select(rolePair => rolePair.Value).ToList();
		
		List<DiscordSelectComponent> selectionComponents = new List<DiscordSelectComponent>();
		int selectionOptions = 0;
		for (int selectionBoxes = 0; selectionBoxes < 5 && selectionOptions < savedRoles.Count; selectionBoxes++)
		{
			List<DiscordSelectComponentOption> roleOptions = new List<DiscordSelectComponentOption>();
			
			for (; selectionOptions < 25 * (selectionBoxes + 1) && selectionOptions < savedRoles.Count; selectionOptions++)
            {
				roleOptions.Add(new DiscordSelectComponentOption(savedRoles[selectionOptions].Name, savedRoles[selectionOptions].Id.ToString()));
            }
			selectionComponents.Add(new DiscordSelectComponent("rolemanager_togglerole" + selectionBoxes, "Join/Leave role", roleOptions, false, 1, 1));
		}

		return selectionComponents;
	}
}