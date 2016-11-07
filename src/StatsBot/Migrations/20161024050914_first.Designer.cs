using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using StatsBot.Models;

namespace StatsBot.Migrations
{
    [DbContext(typeof(StatsContext))]
    [Migration("20161024050914_first")]
    partial class first
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("StatsBot.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("Author_Id");

                    b.Property<ulong>("Channel_Id");

                    b.Property<string>("Content");

                    b.Property<DateTime>("Date");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });
        }
    }
}
