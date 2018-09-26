RoleManager
======
A Discord bot which allows users to add or remove roles using commands.

## Commands

`join [role]` - Grants a member that role.

`leave [role]` - Removes a role from that member.

`help` - Shows a help window with command information.

`addrole <command> <role name>` - Adds a new role command to the bot.

`removerole <command>` - Removes a role command from the bot.

## Installation

1. Install Node either by direct download: https://nodejs.org/en/download/ or via package manager in linux distros: https://nodejs.org/en/download/package-manager/
2. Set up your config according to the guide below.
3. Run either via start.bat/start.sh or by entering `node <path to root dir>` in the terminal.

If it complains that it does not reqognize some discord things you may need to run `npm install discord.js` in the bot folder.

## Config settings

Rename default_config.json to config.json before you start.

```json
"token": "add-your-token-here"
```

You need to enter your bot token for the bot to function. You can find it on https://discordapp.com/developers/applications/me

```json
"prefix": "+"
```

The command prefix for this bot, you can change it to for instance ! for !join or - for -join depending on other bots clashing with the current prefix.

```json
"avatarURL": "https://karlofduty.com/img/tardisIcon.jpg"
```

This is either the path or url designating the avatar you want the bot to have. Default is my github avatar.

```json
"roles": [
  { "scp": "SCP:SL Player" },
  { "dystopia": "Dystopia Player" },
  { "ttt": "TTT Player" }
]
```

This is a list of all roles you want your members to be able to access with these commands. The first value in each pair is the keyword members use in their commands, such as `+join scp`. The second value is the name of the Discord role they should be given. Members can also use `+join all` or `+leave all`. You can also add to these using the `addrole` command.
