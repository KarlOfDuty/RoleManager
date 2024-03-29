using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class CreateRoleSelectorCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[SlashCommand("createroleselector", "Creates a selection box which users can use to get new roles.")]
	public async Task OnExecute(InteractionContext command)
	{
		DiscordMessageBuilder builder = new DiscordMessageBuilder().WithContent("Use this to join or leave public roles:");

		foreach (DiscordSelectComponent component in await GetSelectComponents(command))
		{
			builder.AddComponents(component);
		}
		
		await command.Channel.SendMessageAsync(builder);
		await command.CreateResponseAsync(new DiscordEmbedBuilder
		{
			Color = DiscordColor.Green,
			Description = "Successfully created message, make sure to run this command again if you add new roles to the bot."
		}, true);
	}
	
	public static async Task<List<DiscordSelectComponent>> GetSelectComponents(InteractionContext command)
	{
		List<DiscordRole> savedRoles = command.Guild.Roles.Where(rolePair => Roles.savedRoles.Contains(rolePair.Key))
			.Select(rolePair => rolePair.Value).ToList();
		
		savedRoles = savedRoles.OrderBy(x => x.Name).ToList();
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