using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class JoinCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[Config.ConfigPermissionCheckAttribute("join")]
	[SlashRequireBotPermissions(Permissions.ManageRoles)]
	[SlashCommand("join", "Joins a Discord role")]
	public async Task OnExecute(InteractionContext command, [Option("Role", "The role you want to join.")] DiscordRole role)
	{
		if (Roles.savedRoles.All(savedRole => savedRole != role.Id))
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Description = "It is not possible to request that role."
			});
			return;
		}

		try
		{
			await command.Member.GrantRoleAsync(role);
		}
		catch (UnauthorizedException)
		{
			
		}
		
		
		await command.CreateResponseAsync(new DiscordEmbedBuilder
		{
			Color = DiscordColor.Green,
			Description = "Role granted."
		});
	}
}

