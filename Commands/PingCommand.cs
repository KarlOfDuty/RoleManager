using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RoleManager.Commands;

public class PingCommand : ApplicationCommandModule
{
	[SlashRequireGuild]
	[Config.ConfigPermissionCheckAttribute("ping")]
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
		
		await command.Channel.SendMessageAsync(role.Mention, new DiscordEmbedBuilder
	    {
	        Color = DiscordColor.Green,
	        Description = "Ping brought to you by " + command.Member.Mention + "!"
	    });
		
		await command.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Done! :ok_hand:").AsEphemeral());
	}
}