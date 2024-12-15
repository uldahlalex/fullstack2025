using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace infrastructure;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Winnersequence> Winnersequences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(entity =>
        {
            entity.ToTable("board", "jerneif");

            entity.HasIndex(e => e.Gameid, "IX_board_gameid");

            entity.HasIndex(e => new { e.Userid, e.Gameid }, "IX_board_userid_gameid");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Afviklet).HasColumnName("afviklet");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Gameid).HasColumnName("gameid");
            entity.Property(e => e.Sortednumbers).HasColumnName("sortednumbers");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Won).HasColumnName("won");

            entity.HasOne(d => d.Game).WithMany(p => p.Boards).HasForeignKey(d => d.Gameid);

            entity.HasOne(d => d.User).WithMany(p => p.Boards).HasForeignKey(d => d.Userid);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("game", "jerneif");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Weeknumber).HasColumnName("weeknumber");
            entity.Property(e => e.Yearnumber).HasColumnName("yearnumber");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("player", "jerneif");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Activated).HasColumnName("activated");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<Winnersequence>(entity =>
        {
            entity.ToTable("winnersequence", "jerneif");

            entity.HasIndex(e => e.Gameid, "winnersequence_gameid_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Gameid).HasColumnName("gameid");
            entity.Property(e => e.Sequence).HasColumnName("sequence");

            entity.HasOne(d => d.Game).WithOne(p => p.Winnersequence).HasForeignKey<Winnersequence>(d => d.Gameid);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
