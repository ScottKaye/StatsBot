using Discord.Commands;
using Discord.WebSocket;
using StatsBot.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StatsBot.Commands
{
	public class Standalone : ModuleBase
	{
		private StatsContext _stats;
		private DiscordSocketClient _client;

		public Standalone(StatsContext stats, DiscordSocketClient client)
		{
			_stats = stats;
			_client = client;
		}

		[Command("wordcount"), Alias("wc"), Summary("Find the number of times a word has been said.")]
		public async Task WordCount([Summary("Word to find")] params string[] words)
		{
			if (words.Length > 10)
			{
				await ReplyAsync("I can only do a maximum of 10 words at a time.");
				return;
			}

			foreach (string wordRaw in words)
			{
				var word = wordRaw.ToLowerInvariant();

				var sightings = from s in _stats.Messages
								where s.Content.Contains(word)
								where s.Channel_Id == Context.Channel.Id
								select s;

				int count = sightings.Count();

				switch (count)
				{
					case 0:
						await ReplyAsync($":x: I've never seen **'{word}'** before.");
						break;
					case 1:
						await ReplyAsync($":one: I've seen **'{word}'** just once.");
						break;
					default:
						await ReplyAsync($":white_check_mark: I've seen **'{word}'** **{count}** times.");
						break;
				}
			}
		}

		[Command("messagecount"), Alias("mc"), Summary("Find the number of messages a user has sent.")]
		public async Task MessageCount(params string[] mentions) // Parameters are not used
		{
			var userIds = Context.Message.MentionedUserIds.Where(id => id != Context.Client.CurrentUser.Id).ToList();

			if (userIds.Count > 10)
			{
				await ReplyAsync("I can only do a maximum of 10 users at a time.");
				return;
			}

			foreach (var userId in userIds)
			{
				var sightings = from s in _stats.Messages
								where s.Author_Id == userId
								where s.Channel_Id == Context.Channel.Id
								select s;

				var count = sightings.Count();
				var user = await Context.Channel.GetUserAsync(userId);

				switch (count)
				{
					case 0:
						await ReplyAsync($":x: I've never seen **{user.Username}** send a message before.");
						break;
					case 1:
						await ReplyAsync($":one: I've seen **{user.Username}** only sent 1 message.");
						break;
					default:
						await ReplyAsync($":white_check_mark: I've seen **{user.Username}** send **{count}** messages.");
						break;
				}
			}
		}
	}
}