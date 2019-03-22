const Discord = require("discord.js");
const roles = require("./roles.json");
var fs = require("fs");
const YAML = require("yaml");
const config = fs.readFileSync("./config.yml", "utf8");
const { token, prefix, commands } = YAML.parse(config);

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
            console.log("Executing \"" + message.content + "\" on " + message.member.user.tag + ".");
            doesNotExist = false;
            const role = message.guild.roles.find("name", roles[i][key]);
            if(role == null)
            {
                message.channel.send("```diff\n- Error occured, does that role exist?```");
                return;
            }

            if(message.member == null)
            {
                message.channel.send("```diff\n- Error occured, sender of that message was null. Discord claims you do not exist. Debug info: " + message.member + " " + message.author + "```");
                return;
            }

            if(addRole)
            {
                message.member.addRole(role).then(() =>
                {
                    message.channel.send("```diff\n+ Role \"" + role.name + "\" granted.```");
                }).catch(() =>
                {
                    message.channel.send("```diff\n- Error occured, does the bot have permission to manage that role?```");
                });
            }
            else
            {
                message.member.removeRole(role).then(() =>
                {
                    message.channel.send("```diff\n+ Role \"" + role.name + "\" revoked.```");
                }).catch(() =>
                {
                    message.channel.send("```diff\n- Error occured, does the bot have permission to manage that role?```");
                });
            }
        }
    }
    if(doesNotExist)
    {
        message.channel.send("```diff\n- Invalid role provided.```");
    }
}

function pingCommand(message, roleTag)
{
    var doesNotExist = true;
    //Finds the role the user requested from the config
    for (var i = 0; i < roles.length; i++)
    {
        const key = Object.keys(roles[i])[0];
        if (key === roleTag)
        {
            doesNotExist = false;
            const role = message.guild.roles.find("name", roles[i][key]);
            if (role == null)
            {
                message.channel.send("```diff\n- Error occured, does that role exist?```");
                return;
            }

            if (role.mentionable)
            {
                const embed = {
                    "description": `Ping brought to you by <@${message.member.id}>`,
                    "color": 3768539
                };
                message.channel.send("<@&" + role.id + ">", { embed }).then(message.delete().catch(console.error)).catch((err) =>
                {
                    console.error("ERROR: Could not send response message. " + err);
                });
            }
            else
            {
                role.setMentionable(true).then(() =>
                {
                    const embed = {
                        "description": `Ping brought to you by <@${message.member.id}>`,
                        "color": 3768539
                    };
                    message.channel.send("<@&" + role.id + ">", { embed }).then(() =>
                    {
                        role.setMentionable(false).catch(() =>
                        {
                            message.channel.send("```diff\n- Error occured, does the bot have permission to manage that role?```");
                        });
                    }).catch((err) =>
                    {
                        console.error("Could not send response message. " + err);
                        role.setMentionable(false).catch(() =>
                        {
                            message.channel.send("```diff\n- Error occured, does the bot have permission to manage that role?```");
                        });
                    });
                    message.delete().catch(console.error);
                }).catch(() =>
                {
                    message.channel.send("```diff\n- Error occured, does the bot have permission to manage that role?```");
                });
            }
        }
    }
    if (doesNotExist)
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
            fs.writeFile("roles.json", JSON.stringify(roles, null, 4), (err) =>
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
    helpMessage += ("\n- < " + prefix + "join all > gives all of the above roles.");
    helpMessage += ("\n");
    helpMessage += ("\n- < " + prefix + "leave (tag) > removes any of the above roles.");
    if (hasPermission(message, "addrole") || hasPermission(message, "removerole") || hasPermission(message, "ping"))
    {
        helpMessage += ("\n");
        helpMessage += ("\n# Admin commands");
        helpMessage += ("\n- < " + prefix + "addrole (tag) (role) > adds a new role to rolemanager.");
        helpMessage += ("\n- < " + prefix + "removerole (tag) > removes a role from rolemanager.");
        helpMessage += ("\n- < " + prefix + "ping (tag) > pings a normally unpingable role.");
    }
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
    fs.writeFile("roles.json", JSON.stringify(roles, null, 4), (err) =>
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

function hasPermission(message, command)
{
    if (!commands[command])
    {
        return false;
    }

    if (!commands[command].channels.includes(message.channel.id) && !commands[command].channels.includes("*"))
    {
        return false;
    }

    if (commands[command].permissions.roles.includes("*") || commands[command].permissions.users.includes("*"))
    {
        return true;
    }

    if (commands[command].permissions.users.includes(message.member.id))
    {
        return true;
    }

    var memberRoles = message.member.roles;
    for (var [roleID, role] of memberRoles)
    {
        if (commands[command].permissions.roles.includes(roleID))
        {
            return true;
        }
    }
    return false;
}

discordClient.on("ready", () =>
{
    console.log("Discord connection established!");
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
            // Abort if no permission
            if (!hasPermission(message, command))
            {
                message.channel.send("```diff\n- You are not allowed to do that.```");
                return;
            }
            if (!args.length)
            {
                return message.channel.send("```diff\n- You didn't specify a role.```");
            }
            manageUser(message, args.shift().toLowerCase(), true);
            break;
        case "leave":
            // Abort if no permission
            if (!hasPermission(message, command))
            {
                message.channel.send("```diff\n- You are not allowed to do that.```");
                return;
            }
            if (!args.length)
            {
                return message.channel.send("```diff\n- You didn't specify a role.```");
            }
            manageUser(message, args.shift().toLowerCase(), false);
            break;
        case "ping":
            // Abort if no permission
            if (!hasPermission(message, command))
            {
                message.channel.send("```diff\n- You are not allowed to do that.```");
                return;
            }
            if (!args.length)
            {
                return message.channel.send("```diff\n- You didn't specify a role.```");
            }
            pingCommand(message, args.shift().toLowerCase());
            break;
        case "addrole":
            // Abort if no permission
            if (!hasPermission(message, command))
            {
                message.channel.send("```diff\n- You are not allowed to do that.```");
                return;
            }
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
            // Abort if no permission
            if (!hasPermission(message, command))
            {
                message.channel.send("```diff\n- You are not allowed to do that.```");
                return;
            }
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
            // Abort if no permission
            if (!hasPermission(message, command))
            {
                message.channel.send("```diff\n- You are not allowed to do that.```");
                return;
            }
            helpCommand(message);
            break;
    }
});

discordClient.on("error", (e) => console.error(e));
discordClient.on("warn", (e) => console.warn(e));
//discordClient.on("debug", (e) => console.info(e));

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
