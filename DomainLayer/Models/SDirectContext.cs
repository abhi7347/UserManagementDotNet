using System;
using System.Collections.Generic;
using System.ComponentModel;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace USERMANAGEMENT.Models;

public partial class SDirectContext : DbContext
{
    public SDirectContext()
    {
    }

    public SDirectContext(DbContextOptions<SDirectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AbhiUser> AbhiUsers { get; set; }

    public virtual DbSet<AddresssAbhi> AddresssAbhis { get; set; }

    public virtual DbSet<AddresssMasterAbhi> AddresssMasterAbhis { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
                => optionsBuilder.UseSqlServer("Server=172.24.0.101;Database=sDirect;User Id=sDirect;Password=sDirect;TrustServerCertificate=True;");

    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
      //      => optionsBuilder.UseSqlServer("Server=TIWARIJI\\SQLEXPRESS;Database=AbhiDB;Integrated Security=True;TrustServerCertificate=True;");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<AbhiUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__AbhiUser__1788CC4CD7B129E1");

            entity.ToTable("AbhiUser");

            entity.HasIndex(e => e.Email, "UQ__AbhiUser__A9D1053433C6B673").IsUnique();

            entity.Property(e => e.AlternatePhone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ImagePath)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);

            // Configuring the one-to-many relationship
            entity.HasMany(u => u.AddresssAbhis)
                .WithOne(d => d.AbhiUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AddresssAbhi_AbhiUser");
        });

        modelBuilder.Entity<AddresssAbhi>(entity =>
        {
            // Assuming AddresssAbhi should have a primary key
            entity.HasKey(e => new { e.AId, e.UserId }); // Composite key

            entity.ToTable("AddresssAbhi");

            entity.Property(e => e.AId).HasColumnName("AId");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ZipCode).HasColumnName("ZipCode");

            // Configuring the many-to-one relationship to AbhiUser
            entity.HasOne(d => d.AbhiUser)
                .WithMany(u => u.AddresssAbhis)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AddresssAbhi_AbhiUser");

            // Configuring the many-to-one relationship to AddresssMasterAbhi
            entity.HasOne(d => d.AddresssMasterAbhi)
                .WithMany(m => m.AddresssAbhis)
                .HasForeignKey(d => d.AId)
                .HasConstraintName("FK_AddresssAbhi_AddresssMasterAbhi");
        });

        modelBuilder.Entity<AddresssMasterAbhi>(entity =>
        {
            entity.HasKey(e => e.AId); // Primary key

            entity.ToTable("AddresssMasterAbhi");

            entity.Property(e => e.AId).HasColumnName("AId");
            entity.Property(e => e.AType)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Configuring the one-to-many relationship
            entity.HasMany(m => m.AddresssAbhis)
                .WithOne(d => d.AddresssMasterAbhi)
                .HasForeignKey(d => d.AId)
                .HasConstraintName("FK_AddresssAbhi_AddresssMasterAbhi");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
