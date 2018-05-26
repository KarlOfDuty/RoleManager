const Discord = require('discord.js');
const { prefix, token, roles } = require('./config.json');

const client = new Discord.Client();

client.on('ready', () => {
    console.log('Discord connection established!');
});

client.on('message', message => {
    //Abort if message does not start with the prefix
    if (!message.content.startsWith(prefix) || message.author.bot) return;

    //Cut message into base command and arguments
    const args = message.content.slice(prefix.length).split(/ +/);
    const command = args.shift().toLowerCase();

    //Join command
    if (command === 'join') {
        //Cancel if there are no arguments
        if (!args.length) {
            return message.channel.send('You didn\'t specify a role, ' + message.author + '!');
        }

        const roleArg = args.shift().toLowerCase();
        //Adds all roles from the config to the user
        if (roleArg === 'all') {
            for (var i = 0; i < roles.length; i++) {
                //Set role for user
                const key = Object.keys(roles[i])[0];
                const role = message.guild.roles.find('name', roles[i][key]);
                message.member.addRole(role).catch(err => {
                    console.error(err);
                    message.channel.send('Internal error occured, does the bot have permission to do that?');
                });
            }
            message.channel.send('Roles added, ' + message.author + '!');
            console.log('Executed "' + message.content + '" successfully on ' + message.member.user.tag + '.');
        }
        else {
            //Finds the role the user requested from the config
            for (var i = 0; i < roles.length; i++) {
                const key = Object.keys(roles[i])[0];
                if (key === roleArg) {
                    //Set role for user
                    const role = message.guild.roles.find('name', roles[i][key]);
                    message.member.addRole(role).catch(err => {
                        console.error(err);
                        message.channel.send('Internal error occured, does the bot have permission to do that?');
                    });
                    message.channel.send('Role added, ' + message.author + '!');
                    console.log('Executed "' + message.content + '" successfully on ' + message.member.user.tag + '.');
                }
            }
            message.channel.send('Invalid role provided, ' + message.author + '!');
        }
    }
    //Leave command
    if (command === 'leave') {
        //Cancel if there are no arguments
        if (!args.length) {
            return message.channel.send('You didn\'t specify a role, ' + message.author + '!');
        }

        const roleArg = args.shift().toLowerCase();
        //Remove all roles from the config to the user
        if (roleArg === 'all') {
            for (var i = 0; i < roles.length; i++) {
                //Set role for user
                const key = Object.keys(roles[i])[0];
                const role = message.guild.roles.find('name', roles[i][key]);
                message.member.removeRole(role).catch(err => {
                    console.error(err);
                    message.channel.send('Internal error occured, does the bot have permission to do that?');
                });
            }
            message.channel.send('Roles removed, ' + message.author + '!');
            console.log('Executed "' + message.content + '" successfully on ' + message.member.user.tag + '.');
        }
        else {
            //Finds the role the user requested from the config
            for (var i = 0; i < roles.length; i++) {
                const key = Object.keys(roles[i])[0];
                if (key === roleArg) {
                    //Remove role for user
                    const role = message.guild.roles.find('name', roles[i][key]);
                    message.member.removeRole(role).catch(err => {
                        console.error(err);
                        message.channel.send('Internal error occured, does the bot have permission to do that?');
                    });
                    message.channel.send('Role removed, ' + message.author + '!');
                    console.log('Executed "' + message.content + '" successfully on ' + message.member.user.tag + '.');
                    return;
                }
            }
            message.channel.send('Invalid role provided, ' + message.author + '!');
        }
    }
});

client.login(token);