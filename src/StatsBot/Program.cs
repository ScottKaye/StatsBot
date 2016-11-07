using Discord;
using Discord.Commands;
using Discord.WebSocket;
using StatsBot.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net;
using Discord.Net;

namespace StatsBot
{
	public class Program
	{
		private DiscordSocketClient _client;
		private CommandService _commands;
		private IDependencyMap _map;
		private StatsContext _stats;
		private IConfigurationRoot config;

		public static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile(
					path: "appsettings.json",
					optional: false,
					reloadOnChange: true
				);

			using (var stats = new StatsContext())
			{
				try
				{
					new Program(builder.Build())
						.RunAsync(stats)
						.GetAwaiter()
						.GetResult();
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
				{
					Console.Error.WriteLine("Discord authorization failed.  Your API token may not be correct.");
				}
			}
		}

		public Program(IConfigurationRoot config)
		{
			this.config = config;
		}

		private async Task RunAsync(StatsContext stats)
		{
			_stats = stats;
			_client = new DiscordSocketClient();
			_commands = new CommandService();
			await _commands.AddModules(Assembly.GetEntryAssembly());
			_map = new DependencyMap();
			_map.Add(_client);
			_map.Add(_stats);

			_client.MessageReceived += HandleCommand;
			_client.MessageReceived += HandleMessage;

			Console.WriteLine("Logging in...");
			await _client.LoginAsync(TokenType.Bot, config["Token"]);
			await _client.ConnectAsync();
			Console.WriteLine("Bot is ready.");
			await Task.Delay(-1);
		}

		private async Task HandleCommand(SocketMessage messageRaw)
		{
			if (_client.CurrentUser.Id == messageRaw.Author.Id) return;

			var message = messageRaw as SocketUserMessage;
			if (message == null) return;

			int argPos = 0;
			if (message.HasMentionPrefix(_client.CurrentUser, ref argPos))
			{
				var context = new CommandContext(_client, message);
				var result = await _commands.Execute(context, argPos, _map);

				if (!result.IsSuccess)
					await message.Channel.SendMessageAsync(result.ErrorReason);
			}
		}

		private async Task HandleMessage(SocketMessage message)
		{
			if (_client.CurrentUser.Id == message.Author.Id) return;
			int argPos = 0;
			if (message is SocketUserMessage && (message as SocketUserMessage).HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

			_stats.Messages.Add(new Message
			{
				Author_Id = message.Author.Id,
				Channel_Id = message.Channel.Id,
				Content = message.Content,
				Date = DateTime.Now
			});

			int changes = await _stats.SaveChangesAsync();
		}
	}
}