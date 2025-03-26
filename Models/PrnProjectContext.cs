﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ProjectPRN_SE1886.Models;

public partial class PrnProjectContext : DbContext
{
    public PrnProjectContext()
    {
    }

    public PrnProjectContext(DbContextOptions<PrnProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Household> Households { get; set; }

    public virtual DbSet<HouseholdMember> HouseholdMembers { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

    }
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server= dunghoa\\SQLEXPRESS;uid=sa;password=123;database=PRN_Project;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Household>(entity =>
        {
            entity.HasKey(e => e.HouseholdId).HasName("PK__Househol__1453D6EC040DE786");

            entity.HasIndex(e => e.HouseholdNumber, "UQ_HouseholdNumber").IsUnique();

            entity.Property(e => e.HouseholdId).HasColumnName("HouseholdID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.HeadOfHouseholdId).HasColumnName("HeadOfHouseholdID");
            entity.Property(e => e.HouseholdNumber)
                .HasMaxLength(20)
                .HasDefaultValue("");

            entity.HasOne(d => d.HeadOfHousehold).WithMany(p => p.Households)
                .HasForeignKey(d => d.HeadOfHouseholdId)
                .HasConstraintName("FK__Household__HeadO__48CFD27E");
        });

        modelBuilder.Entity<HouseholdMember>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Househol__0CF04B3841638C20");

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.HouseholdId).HasColumnName("HouseholdID");
            entity.Property(e => e.Relationship).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Household).WithMany(p => p.HouseholdMembers)
                .HasForeignKey(d => d.HouseholdId)
                .HasConstraintName("FK__Household__House__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.HouseholdMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Household__UserI__47DBAE45");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Logs__5E5499A815B9F375");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Logs__UserID__49C3F6B7");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E324634580A");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__4AB81AF0");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Registra__6EF5883026765377");

            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
            entity.Property(e => e.HouseholdId).HasColumnName("HouseholdID");
            entity.Property(e => e.RegistrationType).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.RegistrationApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__Registrat__Appro__4BAC3F29");

            entity.HasOne(d => d.Household).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.HouseholdId)
                .HasConstraintName("FK__Registrat__House__4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.RegistrationUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Registrat__UserI__4D94879B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC81459932");

            entity.HasIndex(e => e.Cccd, "UQ_CCCD").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053485A2C165").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .HasDefaultValue("")
                .HasColumnName("CCCD");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

