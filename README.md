RoleManager [![Release](https://img.shields.io/github/release/KarlofDuty/RoleManager.svg)](https://github.com/KarlOfDuty/RoleManager/releases) [![Downloads](https://img.shields.io/github/downloads/KarlOfDuty/RoleManager/total.svg)](https://github.com/KarlOfDuty/RoleManager/releases) [![Discord Server](https://img.shields.io/discord/430468637183442945.svg?label=discord)](https://discord.gg/C5qMvkj)
======
A Discord bot which allows users to add or remove roles using commands.

## Commands

`/addrole <role>` - Adds a role to the bot so users can join it.

`/removerole <role>` - Removes a role from the bot so users can no longer join it.

`/createroleselector` - Creates a role selector message like in the image below:

![Image showing a Discord selection box](readmeImage.png)

`/ping <role>` - Pings a role which would normally be un-pingable. Role must be added as joinable in the bot.

## Installation

1. Install .NET 6
2. [Create a new bot application](https://discordpy.readthedocs.io/en/latest/discord.html). You don't have to select and permission for the bot in the setup but you do need to add the application intents on the bot page and `applications.commands` in the OAuth page when you create the invite link.
3. Download the bot for your operating system, either a [release version](https://github.com/KarlOfDuty/RoleManager/releases) or a [dev build](https://jenkins.karlofduty.com/blue/organizations/jenkins/RoleManager/activity)
4. Run `./RoleManager` on Linux or `./RoleManager.exe` on Windows.
5. Set up the config (`config.yml`) to your specifications, there are instructions inside and also further down on this page. If you need more help either contact me in Discord or through an issue here.

## Config settings

This is the default included config:
```yaml
bot:
    # Bot token.
    token: "<add-token-here>"
    # ID of the Discord server
    server-id: 000000000000000000
    # Decides what messages are shown in console
    # Possible values are: Critical, Error, Warning, Information, Debug.
    console-log-level: "Information"
    # Sets the type of activity for the bot to display in its presence status
    # Possible values are: Playing, Streaming, ListeningTo, Watching, Competing
    presence-type: "ListeningTo"
    # Sets the activity text shown in the bot's status
    presence-text: "role requests"
```
