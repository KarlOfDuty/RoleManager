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

    //Read command
    if (command === 'join') {
        //Cancel if there are no arguments
        if (!args.length) {
            return message.channel.send('You didn\'t provide any arguments, ' + message.author + '!');
        }

        const roleArg = args.shift().toLowerCase();
        //Adds all roles from the config to the user
        if (roleArg === 'all') {
            for (var i = 0; i < roles.length; i++) {
                //Set role for user
                const key = Object.keys(roles[i])[0];
                console.log(key);
                const role = message.guild.roles.find('name', roles[i][key]);
                message.member.addRole(role).catch(err => {
                    console.error(err);
                    message.channel.send('Internal error occured, does the bot have permission to do that?');
                });
            }
        }
        else {
            //Finds the role the user requested from the config
            for (var i = 0; i < roles.length; i++) {
                const key = Object.keys(roles[i])[0];
                console.log(key);

                if (key === roleArg) {
                    console.log(roles[i][key]);

                    //Set role for user
                    const role = message.guild.roles.find('name', roles[i][key]);
                    message.member.addRole(role).catch(err => {
                        console.error(err);
                        message.channel.send('Internal error occured, does the bot have permission to do that?');
                    });
                    message.channel.send('Role added, ' + message.author + '!');
                    return;
                }
            }
            message.channel.send('Invalid role provided, ' + message.author + '!');
        }
    }
});

client.login(token);