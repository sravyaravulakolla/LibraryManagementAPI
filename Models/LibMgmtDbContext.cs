using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Models;

public partial class LibMgmtDbContext: DbContext
{
    public LibMgmtDbContext()
    {
    }

    public LibMgmtDbContext(DbContextOptions<LibMgmtDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Library> Libraries { get; set; }

    public virtual DbSet<LibraryBook> LibraryBooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=WIN11;Database=LibMgmtDb;Integrated Security=true;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C207F912F932");

            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.AvailableCopies).HasDefaultValue(1);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Books_Categories");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B7704149F");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.HasKey(e => e.LibraryId).HasName("PK__Librarie__A136475F7D96DC22");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<LibraryBook>(entity =>
        {
            entity.HasKey(e => e.LibraryBookId).HasName("PK__LibraryB__86922193BDEDE531");

            entity.HasIndex(e => new { e.LibraryId, e.BookId }, "UQ_LibraryBooks").IsUnique();

            entity.Property(e => e.AvailableCopies).HasDefaultValue(1);

            entity.HasOne(d => d.Book).WithMany(p => p.LibraryBooks)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LibraryBooks_Books");

            entity.HasOne(d => d.Library).WithMany(p => p.LibraryBooks)
                .HasForeignKey(d => d.LibraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LibraryBooks_Libraries");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}