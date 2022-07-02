using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RoleManager;

public class Roles
{
	public class SavedRole
	{
		public ulong roleID = 0;
		public string alias = "";
	}

	public static List<SavedRole> savedRoles = new List<SavedRole>();

	public class RoleChoiceProvider : IChoiceProvider
	{
		public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
		{
			DiscordGuild guild = await RoleManager.discordClient.GetGuildAsync(Config.serverID);

			List<DiscordApplicationCommandOptionChoice> roleChoices = new List<DiscordApplicationCommandOptionChoice>();
			foreach (SavedRole savedRole in savedRoles)
			{
				if (guild.Roles.ContainsKey(savedRole.roleID))
				{
					roleChoices.Add(new DiscordApplicationCommandOptionChoice(guild.Roles[savedRole.roleID].Name, savedRole.alias));
				}
			}
			return roleChoices;
		}
	}
}