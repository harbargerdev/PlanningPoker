﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlanningPoker.Website.Data;

namespace PlanningPoker.Website.Migrations
{
    [DbContext(typeof(GameContext))]
    [Migration("20200810152518_VotingHistory")]
    partial class VotingHistory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PlanningPoker.Core.Entities.Card", b =>
                {
                    b.Property<byte[]>("CardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varbinary(16)");

                    b.Property<string>("CardNumber")
                        .HasColumnType("text");

                    b.Property<string>("CardSource")
                        .HasColumnType("text");

                    b.Property<int>("DeveloperSize")
                        .HasColumnType("int");

                    b.Property<int>("DeveloperVotes")
                        .HasColumnType("int");

                    b.Property<byte[]>("GameId")
                        .HasColumnType("varbinary(16)");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("bit");

                    b.Property<int>("StorySize")
                        .HasColumnType("int");

                    b.Property<int>("TestingSize")
                        .HasColumnType("int");

                    b.Property<int>("TestingVotes")
                        .HasColumnType("int");

                    b.HasKey("CardId");

                    b.HasIndex("GameId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Game", b =>
                {
                    b.Property<byte[]>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varbinary(16)");

                    b.Property<byte[]>("ActiveCardCardId")
                        .HasColumnType("varbinary(16)");

                    b.Property<byte[]>("GameMasterPlayerId")
                        .HasColumnType("varbinary(16)");

                    b.Property<string>("GameName")
                        .HasColumnType("text");

                    b.Property<DateTime>("GameTime")
                        .HasColumnType("datetime");

                    b.HasKey("GameId");

                    b.HasIndex("ActiveCardCardId");

                    b.HasIndex("GameMasterPlayerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Player", b =>
                {
                    b.Property<byte[]>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varbinary(16)");

                    b.Property<byte[]>("GameId")
                        .HasColumnType("varbinary(16)");

                    b.Property<string>("PlayerName")
                        .HasColumnType("text");

                    b.Property<int>("PlayerType")
                        .HasColumnType("int");

                    b.HasKey("PlayerId");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Vote", b =>
                {
                    b.Property<byte[]>("VoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varbinary(16)");

                    b.Property<byte[]>("CardId")
                        .HasColumnType("varbinary(16)");

                    b.Property<byte[]>("PlayerId")
                        .HasColumnType("varbinary(16)");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.HasKey("VoteId");

                    b.HasIndex("CardId");

                    b.HasIndex("PlayerId");

                    b.ToTable("Vote");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Card", b =>
                {
                    b.HasOne("PlanningPoker.Core.Entities.Game", null)
                        .WithMany("Cards")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Game", b =>
                {
                    b.HasOne("PlanningPoker.Core.Entities.Card", "ActiveCard")
                        .WithMany()
                        .HasForeignKey("ActiveCardCardId");

                    b.HasOne("PlanningPoker.Core.Entities.Player", "GameMaster")
                        .WithMany()
                        .HasForeignKey("GameMasterPlayerId");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Player", b =>
                {
                    b.HasOne("PlanningPoker.Core.Entities.Game", null)
                        .WithMany("Players")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Vote", b =>
                {
                    b.HasOne("PlanningPoker.Core.Entities.Card", "Card")
                        .WithMany("Votes")
                        .HasForeignKey("CardId");

                    b.HasOne("PlanningPoker.Core.Entities.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });
#pragma warning restore 612, 618
        }
    }
}
