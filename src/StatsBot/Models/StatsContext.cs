using System;
using Microsoft.EntityFrameworkCore;

namespace StatsBot.Models
{
	public class StatsContext : DbContext
	{
		public DbSet<Message> Messages { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// If you're arrived here after trying to run Update-Database
			// Powershell: $env:DBPATH="R:\stats.db"; Update-Database
			// The file will be created if it does not already exist

			var env = Environment.GetEnvironmentVariable("DBPATH");
			if (string.IsNullOrEmpty(env)) throw new ArgumentException("DBPATH environment variable was not set.");

			optionsBuilder.UseSqlite($"filename={env}");
		}
	}

	public class Message
	{
		public int Id { get; set; }
		public ulong Channel_Id { get; set; }
		public ulong Author_Id { get; set; }
		public DateTime Date { get; set; }
		public string Content { get; set; }
	}
}