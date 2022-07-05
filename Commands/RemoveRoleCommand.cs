using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class RemoveRoleCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[Config.ConfigPermissionCheckAttribute("removerole")]
	[SlashCommand("removerole", "Removes a Discord role from the bot")]
	public async Task OnExecute(InteractionContext command, [Option("Role", "ID of the role you want to remove.")] DiscordRole role)
	{
		if (Roles.savedRoles.All(savedRole => savedRole != role.Id))
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Description = "That role is already disabled."
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

		Roles.RemoveRole(role.Id);
		
		await command.CreateResponseAsync(new DiscordEmbedBuilder
		{
			Color = DiscordColor.Green,
			Description = "Role removed."
		}, true);
	}
}