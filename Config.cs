using System.Text;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using RoleManager.Properties;
using YamlDotNet.Serialization;

namespace RoleManager
{
	internal static class Config
	{
		internal static string token = "";
		internal static string logLevel = "Information";
		internal static string presenceType = "Playing";
		internal static string presenceText = "";
		internal static ulong serverID = 0;

		private static Dictionary<string, ulong[]> permissions = new Dictionary<string, ulong[]>
		{
			{ "join",		new ulong[]{ } },
			{ "leave",		new ulong[]{ } },
            { "help",		new ulong[]{ } },
			{ "addrole",    new ulong[]{ } },
			{ "removerole",	new ulong[]{ } },
			{ "ping",		new ulong[]{ } }
		};
		
		public static void LoadConfig()
		{
			// Writes default config to file if it does not already exist
			if (!File.Exists("./config.yml"))
			{
				File.WriteAllText("./config.yml", Encoding.UTF8.GetString(Resources.default_config));
			}

			// Reads config contents into FileStream
			FileStream stream = File.OpenRead("./config.yml");

			// Converts the FileStream into a YAML object
			IDeserializer deserializer = new DeserializerBuilder().Build();
			object yamlObject = deserializer.Deserialize(new StreamReader(stream));

			// Converts the YAML object into a JSON object as the YAML ones do not support traversal or selection of nodes by name 
			ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();
			JObject json = JObject.Parse(serializer.Serialize(yamlObject));

			// Sets up the bot
			token = json.SelectToken("bot.token").Value<string>() ?? "";
			logLevel = json.SelectToken("bot.console-log-level").Value<string>() ?? "";
			presenceType = json.SelectToken("bot.presence-type")?.Value<string>() ?? "Playing";
			presenceText = json.SelectToken("bot.presence-text")?.Value<string>() ?? "";
			serverID = json.SelectToken("bot.server-id")?.Value<ulong>() ?? 0;

			foreach (KeyValuePair<string, ulong[]> node in permissions.ToList())
			{
				try
				{
					permissions[node.Key] = json.SelectToken("permissions." + node.Key).Value<JArray>().Values<ulong>().ToArray();
				}
				catch (ArgumentNullException)
				{
					Logger.Warn(LogID.CONFIG, "Permission node '" + node.Key + "' was not found in the config, using default value: []");
				}
			}
		}

		/// <summary>
		/// Checks whether a user has a specific permission.
		/// </summary>
		/// <param name="member">The Discord user to check.</param>
		/// <param name="permission">The permission name to check.</param>
		/// <returns></returns>
		public static bool HasPermission(DiscordMember member, string permission)
		{
			return member.Roles.Any(role => permissions[permission].Contains(role.Id)) || permissions[permission].Contains(member.Guild.Id);
		}
	}
}
