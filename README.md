RoleManager
======
A Discord bot which allows users to add or remove roles using commands.

## Commands

`+join [role]` or `+add [role]` - Grants a member that role.

`+leave [role]` or `+remove [role]` - Removes a role from that member.

## Config settings:

Rename default_config.json to config.json before you start.

```json
"token": "add-your-token-here"
```

You need to enter your bot token for the bot to function. You can find it on https://discordapp.com/developers/applications/me

```json
"prefix": "+"
```

The command prefix for this bot, you can change it to for instance ! for !join or - for -join depending on other bots clashing with the current one.

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

This is a list of all roles you want your members to be able to access with these commands. The first value in each pair is the keyword members use in their commands, such as `+join scp`. The second value is the name of the Discord role they should be given. Members can also use `+join all` or `+leave all`.
