using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PetShop.Models;

namespace PetShop.Data;

public partial class PetShopContext : DbContext
{
    public PetShopContext()
    {
    }

    public PetShopContext(DbContextOptions<PetShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Listing> Listings { get; set; }

    public virtual DbSet<ListingType> ListingTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderListing> OrderListings { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseNpgsql("Host=localhost;Database=PetShop;Username=postgres;Password=postgres");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Configuration is done in Program.cs — leave this empty to avoid conflicts.
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Currency_pkey");

            entity.ToTable("Currency");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Sign).HasMaxLength(5);
        });

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Listings_pkey");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.Currency).WithMany(p => p.Listings)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("Listings_CurrencyId_fkey");

            entity.HasOne(d => d.Owner).WithMany(p => p.Listings)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("Listings_OwnerId_fkey");

            entity.HasOne(d => d.Photo).WithMany(p => p.Listings)
                .HasForeignKey(d => d.PhotoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_listings_photo");

            entity.HasOne(d => d.Type).WithMany(p => p.Listings)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("Listings_TypeId_fkey");
        });

        modelBuilder.Entity<ListingType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Types_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Types_Id_seq\"'::regclass)");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Orders_pkey");

            entity.HasIndex(e => e.Number, "UX_Orders_Number").IsUnique();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsPaid).HasDefaultValue(false);

            entity.HasOne(d => d.Owner).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("Orders_OwnerId_fkey");
        });

        modelBuilder.Entity<OrderListing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OrderListings_pkey");

            entity.HasOne(d => d.Listing).WithMany(p => p.OrderListings)
                .HasForeignKey(d => d.ListingId)
                .HasConstraintName("fk_listing");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderListings)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order");

            entity.HasOne(d => d.Owner).WithMany(p => p.OrderListings)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_orderlistings_owner");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PasswordResetTokens_pkey");

            entity.HasIndex(e => e.Token, "UX_PasswordResetTokens_Token").IsUnique();

            entity.Property(e => e.Token).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Photos_pkey");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Url).HasMaxLength(250);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Roles_pkey");

            entity.Property(e => e.Name).HasMaxLength(25);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.HasIndex(e => e.Email, "Users_Email_key").IsUnique();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Users_RoleId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
