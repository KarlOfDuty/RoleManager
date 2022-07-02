using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;

namespace RoleManager.Commands;

public class JoinCommand : ApplicationCommandModule
{
	[SlashCommand("join", "Joins a Discord role")]
	public async Task OnExecute(InteractionContext command, [ChoiceProvider(typeof(Roles.RoleChoiceProvider))][Option("Role", "The alias of the role you want to join.")] string roleAlias)
	{
		if (!await RoleManager.VerifyPermission(command, "join")) return;

		// TODO: Implement
	}
}

