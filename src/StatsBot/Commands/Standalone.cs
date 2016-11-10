using Discord.Commands;
using Discord.WebSocket;
using StatsBot.Models;
using System;
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

		[Command("halp"), Alias("help"), Summary("Get a list of commands.")]
		public async Task HalpMe()
		{
			await ReplyAsync($":weary: :weary: **HALP ME** :weary: :weary:");
			await ReplyAsync($"**wc** [words...] :arrow_forward: Count occurences of words separated by spaces. Maximum of 10 words.");
			await ReplyAsync($"**mc** [mentions...] :arrow_forward: Count messages sent by specified users. Maximum of 10 users.");
			await ReplyAsync($"**ar** [mentions...] :arrow_forward: Check if users specified are cucks. Maximum of 5 users.");
		}

		[Command("arrating"), Alias("ar"), Summary("Find how far into the Trump zone someone is.")]
		public async Task AltRightRating(params string[] mentions)
		{
			var userIds = Context.Message.MentionedUserIds.Where(id => id != Context.Client.CurrentUser.Id).ToList();

			if (userIds.Count > 5)
			{
				await ReplyAsync("I can only do a maximum of 5 users at a time, cuck.");
				return;
			}

			foreach (var userId in userIds)
			{
				var cuckSightings = from s in _stats.Messages
									where s.Author_Id == userId
									where s.Content.Contains("cuck") || s.Content.Contains("trump")
									select s;

				var count = cuckSightings.Count();
				var user = await Context.Channel.GetUserAsync(userId);

				// Max 100 because I'm lazy
				if (count > 100) count = 100;

				// Interval is 10, break into ranges
				int range = (count - 1) / 10;

				switch (range)
				{
					case 0:
						// 1-10 occurences
						await ReplyAsync($":bangbang:**{user.Username}** is a normal human bean. :innocent: ");
						break;
					case 1:
						await ReplyAsync($":bangbang:**{user.Username}** is a **cuck.** :jeb:");
						break;
					case 2:
						await ReplyAsync($":bangbang:**{user.Username}** is currently boarding the **TRUMP :tangerine: TRAIN** :train2:");
						break;
					case 3:
						await ReplyAsync($":bangbang:**{user.Username}** just sat down in their reserved Trump :tangerine: Train :train2: seat and ordered a beer :beer:. Cheers. :beers:");
						break;
					case 4:
						await ReplyAsync($":bangbang:**{user.Username}** is playing Beer :beer: Pong :ping_pong: with Ted Cruz in the Trump :tangerine: Train :train2:");
						break;
					case 5:
						await ReplyAsync($":bangbang:**{user.Username}** is going FULL SPEED AHEAD. THERE'S NO TURNING BACK NOW!!!! :train2: :train2: :train2:");
						break;
					case 6:
						await ReplyAsync($":bangbang:**{user.Username}** just bought the entire crew a round of **AMERICAN :flag_us: WHISKEY :tumbler_glass:**");
						break;
					case 7:
						await ReplyAsync($":bangbang:**{user.Username}** is searching :mag: through Wikileaks :sweat_drops: emails for the **SMOKING :smoking: GUN :gun:**");
						break;
					case 8:
						await ReplyAsync($":bangbang:**{user.Username}** is making fresh :leaves: **PEPE THE FROG** :frog: memes to SPREAD :peanuts: THE WORD :loudspeaker:");
						break;
					case 9:
						await ReplyAsync($":bangbang:**{user.Username}** got an American :flag_us: citizenship :classical_building: just to vote for Trump :tangerine:. Bravo. :clap::skin-tone-1:");
						break;
				}
			}
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

				var count = sightings.Count();

				if (wordRaw.Equals("trump", StringComparison.CurrentCultureIgnoreCase))
					count = 270;

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
						await ReplyAsync($":one: I've seen **{user.Username}** only send 1 message.");
						break;
					default:
						await ReplyAsync($":white_check_mark: I've seen **{user.Username}** send **{count}** messages.");
						break;
				}
			}
		}
	}
}