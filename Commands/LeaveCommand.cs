using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class LeaveCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[Config.ConfigPermissionCheckAttribute("leave")]
	[SlashRequireBotPermissions(Permissions.ManageRoles)]
	[SlashCommand("leave", "Joins a Discord role")]
	public async Task OnExecute(InteractionContext command, [Option("Role", "The role you want to leave.")] DiscordRole role)
	{
		if (Roles.savedRoles.All(savedRole => savedRole != role.Id))
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Description = "It is not possible to leave that role using the bot."
			});
			return;
		}

		await command.Member.RevokeRoleAsync(role);
		
		await command.CreateResponseAsync(new DiscordEmbedBuilder
		{
			Color = DiscordColor.Green,
			Description = "Role revoked."
		});
	}
}