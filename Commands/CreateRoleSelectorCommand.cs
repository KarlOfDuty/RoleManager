using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class CreateRoleSelectorCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[Config.ConfigPermissionCheckAttribute("createroleselector")]
	[SlashCommand("createroleselector", "Creates a selection box which users can use to get new roles.")]
	public async Task OnExecute(InteractionContext command)
	{
		DiscordMessageBuilder builder = new DiscordMessageBuilder().WithContent("Use this to join or leave public roles:");

		foreach (DiscordSelectComponent component in await GetSelectComponents())
		{
			builder.AddComponents(component);
		}
		
		await command.Channel.SendMessageAsync(builder);
	}
	
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
			selectionComponents.Add(new DiscordSelectComponent("rolemanager_togglerole" + selectionBoxes, "Join/Leave role", roleOptions, false, 0, 1));
		}

		return selectionComponents;
	}
}