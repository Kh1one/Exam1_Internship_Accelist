using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace exam1.Entities;

public partial class AccelokaContext : DbContext
{
    public AccelokaContext()
    {
    }

    public AccelokaContext(DbContextOptions<AccelokaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AvailableTicket> AvailableTickets { get; set; }

    public virtual DbSet<BookedTicket> BookedTickets { get; set; }

    public virtual DbSet<BookedTicketDetail> BookedTicketDetails { get; set; }

    public virtual DbSet<CategoryTicket> CategoryTickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured == false)
        {
            optionsBuilder.UseSqlServer("data source=.\\SQLEXPRESS;initial catalog=acceloka;trusted_connection=true;TrustServerCertificate=True");
        }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AvailableTicket>(entity =>
        {
            entity.HasKey(e => e.TicketCode).HasName("PK__Availabl__FCC3B00094108995");

            entity.ToTable("AvailableTicket");

            entity.Property(e => e.TicketCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("ticketCode");
            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.EventDate)
                .HasColumnType("datetime")
                .HasColumnName("eventDate");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quota).HasColumnName("quota");
            entity.Property(e => e.TicketName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ticketName");

            entity.HasOne(d => d.Category).WithMany(p => p.AvailableTickets)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Available__categ__398D8EEE");
        });

        modelBuilder.Entity<BookedTicket>(entity =>
        {
            entity.HasKey(e => e.BookedTicketId).HasName("PK__BookedTi__4EE9C54E7113F6D6");

            entity.ToTable("BookedTicket");

            entity.Property(e => e.BookedTicketId).HasColumnName("bookedTicketID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createdAt");
        });

        modelBuilder.Entity<BookedTicketDetail>(entity =>
        {
            entity
                .ToTable("BookedTicketDetail");

            entity.Property(e => e.BookedTicketDetailId).HasColumnName("bookedTicketDetailId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("ticketCode");

            entity.HasKey(e => new { e.BookedTicketDetailId, e.TicketCode });

            entity.HasOne(d => d.BookedTicketDetailNavigation).WithMany()
                .HasForeignKey(d => d.BookedTicketDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookedTic__booke__4222D4EF");

            entity.HasOne(d => d.TicketCodeNavigation).WithMany()
                .HasForeignKey(d => d.TicketCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookedTic__ticke__412EB0B6");
        });

        modelBuilder.Entity<CategoryTicket>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__23CAF1F845687953");

            entity.ToTable("CategoryTicket");

            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("categoryName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
