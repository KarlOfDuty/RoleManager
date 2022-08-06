using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
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
		}
	}
}
