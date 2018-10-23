const Discord = require("discord.js");
const { token, prefix, avatarURL, roles } = require("./config.json");
var fs = require("fs");

const discordClient = new Discord.Client();

function manageUser(message, roleTag, addRole)
{
    var doesNotExist = true;
    //Finds the role the user requested from the config
    for (var i = 0; i < roles.length; i++)
    {
        const key = Object.keys(roles[i])[0];
        if (key === roleTag || roleTag === "all")
        {
            doesNotExist = false;
            const role = message.guild.roles.find("name", roles[i][key]);
            if(addRole)
            {
                message.member.addRole(role).catch((err) =>
                {
                    console.error(err);
                    message.channel.send("```diff\n- Error occured, does the bot have permission to do that?```");
                });
                message.channel.send("```diff\n+ Role \"" + role.name + "\" granted.```");
            }
            else
            {
                message.member.removeRole(role).catch((err) =>
                {
                    console.error(err);
                    message.channel.send("```diff\n- Error occured, does the bot have permission to do that?```");
                });
                message.channel.send("```diff\n+ Role \"" + role.name + "\" revoked.```");
            }
            console.log("Executed \"" + message.content + "\" successfully on " + message.member.user.tag + ".");
        }
    }
    if(doesNotExist)
    {
        message.channel.send("```diff\n- Invalid role provided.```");
    }
}

function removeRoleCommand(message, command)
{
    for(var i = 0; i < roles.length; i++)
    {
        const key = Object.keys(roles[i])[0];
        if(key === command)
        {
            roles.splice(i, 1);
            fs.writeFile("config.json", JSON.stringify({ token, prefix, avatarURL, roles }, null, 4), (err) =>
            {
                if (err)
                {
                    message.channel.send("```diff\n- Internal error occured, could not write to config file.```");
                    console.log(err);
                }
                else
                {
                    message.channel.send("```diff\n+ Command removed.```");
                }
            });
            return;
        }
    }
    message.channel.send("```diff\n- Invalid command \"" + command + "\" provided.```");
}

function helpCommand(message)
{
    var helpMessage = "```md";
    helpMessage += ("\n# Member commands");
    for(var i = 0; i < roles.length; i++)
    {
        const key = Object.keys(roles[i])[0];
        helpMessage += ("\n- < " + prefix + "join " + key + " > gives the role < " + roles[i][key] + " >");
    }
    helpMessage += ("\n- < " + prefix + "join all > gives all of the above roles");
    helpMessage += ("\n");
    for(var i = 0; i < roles.length; i++)
    {
        const key = Object.keys(roles[i])[0];
        helpMessage += ("\n- < " + prefix + "leave " + key + " > removes the role < " + roles[i][key] + " >");
    }
    helpMessage += ("\n- < " + prefix + "join all > removes all of the above roles");
    helpMessage += ("\n");
    helpMessage += ("\n# Admin commands");
    helpMessage += ("\n- < " + prefix + "addrole (command) (role name) > adds a new command to rolemanager.");
    helpMessage += ("\n- < " + prefix + "removerole (command) > removes a command from rolemanager");
    helpMessage += "\n```";
    message.channel.send(helpMessage);
}

function addRoleCommand(message, command, role)
{
    for(var i = 0; i < roles.length; i++)
    {
        if(roles[i].key === command)
        {
            message.channel.send("```diff\n- That command already exists.```");
            return;
        }
    }
    roles.push(JSON.parse("{\"" + command + "\":\"" + role +"\"}"));
    fs.writeFile("config.json", JSON.stringify({ token, prefix, avatarURL, roles }, null, 4), (err) =>
    {
        if (err)
        {
            message.channel.send("```diff\n- Internal error occured, could not write to config file.```");
            console.log(err);
        }
        else
        {
            message.channel.send("```diff\n+ Command added.```");
        }
    });
}
discordClient.on("ready", () =>
{
    console.log("Discord connection established!");
    discordClient.user.setAvatar(avatarURL);
    discordClient.user.setActivity(prefix + "help",
    {
        type: "LISTENING"
    });
});

discordClient.on("message", (message) =>
{
    //Abort if message does not start with the prefix
    if (!message.content.startsWith(prefix) || message.author.bot)
    {
        return;
    }

    //Cut message into base command and arguments
    const args = message.content.slice(prefix.length).split(/ +/);
    const command = args.shift().toLowerCase();
    switch(command)
    {
        case "join":
            if (!args.length)
            {
                return message.channel.send("```diff\n- You didn't specify a role.```");
            }
            manageUser(message, args.shift().toLowerCase(), true);
            break;
        case "leave":
            if (!args.length)
            {
                return message.channel.send("```diff\n- You didn't specify a role.```");
            }
            manageUser(message, args.shift().toLowerCase(), false);
            break;
        case "addrole":
            if (args.length < 2)
            {
                return message.channel.send("```diff\n- Missing arguments.```");
            }
            if (!message.member.hasPermission("ADMINISTRATOR"))
            {
                return message.channel.send("```diff\n- You don't have permission to do that.```");
            }
            addRoleCommand(message, args.shift().toLowerCase(), args.join(" "));
            break;
        case "removerole":
            if (!args.length)
            {
                return message.channel.send("```diff\n- You didn't specify a role command to remove.```");
            }
            if (!message.member.hasPermission("ADMINISTRATOR"))
            {
                return message.channel.send("```diff\n- You don't have permission to do that.```");
            }
            removeRoleCommand(message, args.shift().toLowerCase());
            break;
        case "help":
            helpCommand(message);
            break;
    }
});

discordClient.on("error", (e) => console.error(e));
discordClient.on("warn", (e) => console.warn(e));
discordClient.on("debug", (e) => console.info(e));

discordClient.login(token);

// Runs when the server shuts down
function shutdown()
{
    console.log("Signing out of Discord...");
    discordClient.destroy();
}
process.on("exit", () => shutdown());
process.on("SIGINT", () => shutdown());
process.on("SIGUSR1", () => shutdown());
process.on("SIGUSR2", () => shutdown());
process.on("SIGHUP", () => shutdown());
