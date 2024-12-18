using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PropertyRentalManagement.Models;

public partial class PropertyRentalManagementDbContext : DbContext
{
    public PropertyRentalManagementDbContext()
    {
    }

    public PropertyRentalManagementDbContext(DbContextOptions<PropertyRentalManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Apartment> Apartments { get; set; }

    public virtual DbSet<ApartmentType> ApartmentTypes { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.Property(e => e.ApartmentId).HasColumnName("ApartmentID");
            entity.Property(e => e.ApartmentCode).HasMaxLength(10);
            entity.Property(e => e.ApartmentTypeId).HasColumnName("ApartmentTypeID");
            entity.Property(e => e.BuildingCode).HasMaxLength(7);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Rent).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StatusId)
                .HasComment("Available, Rented, Unavailable")
                .HasColumnName("StatusID");

            entity.HasOne(d => d.ApartmentType).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.ApartmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartments_ApartmentTypes");

            entity.HasOne(d => d.Building).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.BuildingCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartments_Buildings");

            entity.HasOne(d => d.Status).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartments_Status");
        });

        modelBuilder.Entity<ApartmentType>(entity =>
        {
            entity.Property(e => e.ApartmentTypeId).HasColumnName("ApartmentTypeID");
            entity.Property(e => e.ApartmentTypeDescription).HasMaxLength(50);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.ApartmentId).HasColumnName("ApartmentID");
            entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.StatusId)
                .HasComment("Pending, Confirmed, Canceled")
                .HasColumnName("StatusID");
            entity.Property(e => e.TenantId).HasColumnName("TenantID");

            entity.HasOne(d => d.Apartment).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ApartmentId)
                .HasConstraintName("FK_Appointments_Apartments");

            entity.HasOne(d => d.Manager).WithMany(p => p.AppointmentManagers)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Status");

            entity.HasOne(d => d.Tenant).WithMany(p => p.AppointmentTenants)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.BuildingCode).HasName("PK_Properties");

            entity.Property(e => e.BuildingCode).HasMaxLength(7);
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Manager).WithMany(p => p.BuildingManagers).HasForeignKey(d => d.ManagerId);

            entity.HasOne(d => d.Owner).WithMany(p => p.BuildingOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.ApartmentId).HasColumnName("ApartmentID");
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.StatusId)
                .HasComment("New, Pending, Solved")
                .HasColumnName("StatusID");

            entity.HasOne(d => d.Apartment).WithMany(p => p.Events)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Apartments");

            entity.HasOne(d => d.Manager).WithMany(p => p.Events)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Status).WithMany(p => p.Events)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Status");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.MessageDateTime).HasColumnType("datetime");
            entity.Property(e => e.ReceiverUserId).HasColumnName("ReceiverUserID");
            entity.Property(e => e.SenderUserId).HasColumnName("SenderUserID");
            entity.Property(e => e.StatusId)
                .HasComment("Read, Not Read")
                .HasColumnName("StatusID");
            entity.Property(e => e.Subject).HasMaxLength(100);

            entity.HasOne(d => d.ReceiverUser).WithMany(p => p.MessageReceiverUsers)
                .HasForeignKey(d => d.ReceiverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SenderUser).WithMany(p => p.MessageSenderUsers)
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Status).WithMany(p => p.Messages)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Status");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Category)
                .HasMaxLength(20)
                .HasComment("Apartments, Appointments, Messages, Events");
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username, "UQ_Username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.Property(e => e.RoleId)
                .HasComment("Tenant, Owner, Manager")
                .HasColumnName("RoleID");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_UserRoles");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable(tb => tb.HasComment("Owner, Manager, Tenant"));

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasComment("Owner, Manager, Tenant");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
