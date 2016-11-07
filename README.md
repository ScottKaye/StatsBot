# StatsBot
Keep track of various things happening on your Discord server.

Currently keeps track of:
- All messages

Providing:
- How many messsages a user has sent
- How many times a term has been said in all tracked messages.

# Setup
- Ensure you have the latest version of [.NET Core SDK](https://www.microsoft.com/net/core#windows)
- Make sure you update the project build settings with a new path for your dev database
- Run `$env:DBPATH="R:\stats.db"; Update-Database`, replacing the path with your own
- Add two feeds to your NuGet Package Sources:
```
Discord.NET Dev: https://www.myget.org/F/discord-net/api/v3/index.json
.NET Core Dev: https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
```
- Create a Discord bot application on the [Discord Developer Console](https://discordapp.com/developers/applications/me)
- Create the bot user and copy the Token into `appsettings.json`

# Contributing
Make sure to not commit your `appsettings.json` changes by telling Git to assume no changes are ever made:

```
git update-index --assume-unchanged src/StatsBot/appsettings.json
```