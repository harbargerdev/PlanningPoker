﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlanningPoker.Website.Data;

#nullable disable

namespace PlanningPoker.Website.Migrations
{
    [DbContext(typeof(GameContext))]
    partial class GameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PlanningPoker.Core.Entities.Card", b =>
                {
                    b.Property<Guid>("CardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CardNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("CardSource")
                        .HasColumnType("longtext");

                    b.Property<int>("DeveloperSize")
                        .HasColumnType("int");

                    b.Property<int>("DeveloperVotes")
                        .HasColumnType("int");

                    b.Property<Guid?>("GameId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("tinyint(1)");

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
                    b.Property<Guid>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ActiveCardCardId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("GameMasterPlayerId")
                        .HasColumnType("char(36)");

                    b.Property<string>("GameName")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("GameTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("GameId");

                    b.HasIndex("ActiveCardCardId");

                    b.HasIndex("GameMasterPlayerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Player", b =>
                {
                    b.Property<Guid>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("GameId")
                        .HasColumnType("char(36)");

                    b.Property<string>("PlayerName")
                        .HasColumnType("longtext");

                    b.Property<int>("PlayerType")
                        .HasColumnType("int");

                    b.HasKey("PlayerId");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Vote", b =>
                {
                    b.Property<Guid>("VoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CardId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("PlayerId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.HasKey("VoteId");

                    b.HasIndex("CardId");

                    b.HasIndex("PlayerId");

                    b.ToTable("Votes");
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

                    b.Navigation("ActiveCard");

                    b.Navigation("GameMaster");
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

                    b.Navigation("Card");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Card", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("PlanningPoker.Core.Entities.Game", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
