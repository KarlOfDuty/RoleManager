using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class AddRoleCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[SlashCommand("addrole", "Adds a Discord role to the bot")]
	public async Task OnExecute(InteractionContext command, [Option("Role", "The role you want to add.")] DiscordRole role)
	{
		if (Roles.savedRoles.Any(savedRole => savedRole == role.Id))
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Description = "That role is already enabled."
			}, true);
			return;
		}
		
		if (!command.Guild.Roles.ContainsKey(role.Id))
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Description = "That role doesn't exist."
			}, true);
			return;
		}

		Roles.AddRole(role.Id);
		
		await command.CreateResponseAsync(new DiscordEmbedBuilder
		{
			Color = DiscordColor.Green,
			Description = "Role added."
		}, true);
	}
}