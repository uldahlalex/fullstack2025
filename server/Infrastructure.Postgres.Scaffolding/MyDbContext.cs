﻿using System;
using System.Collections.Generic;
using Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding;

public partial class MyDbContext : DbContext, IMyDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Devicelog> Devicelogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("device_pkey");

            entity.ToTable("device", "surveillance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Devicelog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("devicelog_pkey");

            entity.ToTable("devicelog", "surveillance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Deviceid).HasColumnName("deviceid");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Unit).HasColumnName("unit");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Device).WithMany(p => p.Devicelogs)
                .HasForeignKey(d => d.Deviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("devicelog_deviceid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user", "surveillance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Hash).HasColumnName("hash");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Salt).HasColumnName("salt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
