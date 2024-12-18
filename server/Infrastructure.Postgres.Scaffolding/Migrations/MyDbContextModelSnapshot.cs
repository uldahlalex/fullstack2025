﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Postgres.Scaffolding.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Application.Models.Entities.Board", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("Afviklet")
                        .HasColumnType("boolean")
                        .HasColumnName("afviklet");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("Gameid")
                        .HasColumnType("uuid")
                        .HasColumnName("gameid");

                    b.Property<List<int>>("Sortednumbers")
                        .IsRequired()
                        .HasColumnType("integer[]")
                        .HasColumnName("sortednumbers");

                    b.Property<Guid>("Userid")
                        .HasColumnType("uuid")
                        .HasColumnName("userid");

                    b.Property<bool>("Won")
                        .HasColumnType("boolean")
                        .HasColumnName("won");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Gameid" }, "IX_board_gameid");

                    b.HasIndex(new[] { "Userid", "Gameid" }, "IX_board_userid_gameid");

                    b.ToTable("board", "jerneif");
                });

            modelBuilder.Entity("Application.Models.Entities.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("Weeknumber")
                        .HasColumnType("integer")
                        .HasColumnName("weeknumber");

                    b.Property<int>("Yearnumber")
                        .HasColumnType("integer")
                        .HasColumnName("yearnumber");

                    b.HasKey("Id");

                    b.ToTable("game", "jerneif");
                });

            modelBuilder.Entity("Application.Models.Entities.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("Activated")
                        .HasColumnType("boolean")
                        .HasColumnName("activated");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("player", "jerneif");
                });

            modelBuilder.Entity("Application.Models.Entities.Winnersequence", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("Gameid")
                        .HasColumnType("uuid")
                        .HasColumnName("gameid");

                    b.Property<List<int>>("Sequence")
                        .IsRequired()
                        .HasColumnType("integer[]")
                        .HasColumnName("sequence");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Gameid" }, "winnersequence_gameid_key")
                        .IsUnique();

                    b.ToTable("winnersequence", "jerneif");
                });

            modelBuilder.Entity("Application.Models.Entities.Board", b =>
                {
                    b.HasOne("Application.Models.Entities.Game", "Game")
                        .WithMany("Boards")
                        .HasForeignKey("Gameid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Application.Models.Entities.Player", "User")
                        .WithMany("Boards")
                        .HasForeignKey("Userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Application.Models.Entities.Winnersequence", b =>
                {
                    b.HasOne("Application.Models.Entities.Game", "Game")
                        .WithOne("Winnersequence")
                        .HasForeignKey("Application.Models.Entities.Winnersequence", "Gameid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Application.Models.Entities.Game", b =>
                {
                    b.Navigation("Boards");

                    b.Navigation("Winnersequence");
                });

            modelBuilder.Entity("Application.Models.Entities.Player", b =>
                {
                    b.Navigation("Boards");
                });
#pragma warning restore 612, 618
        }
    }
}
