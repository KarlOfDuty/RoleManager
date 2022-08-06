using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class PingCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[SlashCommand("ping", "Mentions a Discord role registered with the bot.")]
	public async Task OnExecute(InteractionContext command, [Option("Role", "The role you want to mention.")] DiscordRole role)
	{
		if (Roles.savedRoles.All(savedRole => savedRole != role.Id))
		{
			await command.CreateResponseAsync(new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Description = "This role cannot be pinged."
			}, true);
			return;
		}
		
		await command.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(role.Mention).AddMentions(Mentions.All));
	}
}