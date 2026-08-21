using CulturalCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Data
{
    public class CulturalDbContext : DbContext
    {
        public CulturalDbContext(DbContextOptions<CulturalDbContext> options) : base(options) 
        {
        }

        public DbSet<User> Users { get; set; } = null!; 
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<CulturalItem> CulturalItems { get; set; } = null!;
        public DbSet<ItemMetadata> ItemMetadata { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e =>e.Name).HasMaxLength(50);
                  
                entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Firstname).HasMaxLength(50);
                entity.Property(e => e.Lastname).HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Users_RoleId");

                entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();
                entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();

            });

            modelBuilder.Entity<CulturalItem>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.HistoricalPeriod).HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(d => d.Creator).WithMany(p => p.CreatedItems)
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CulturalItems_CreatedById");

                entity.OwnsOne(e => e.Dimensions);
                entity.OwnsOne(e => e.Coordinates);

                entity.HasIndex(e => e.Status, "IX_CulturalItems_Status");
                entity.HasIndex(e => e.ViewCount, "IX_CulturalItems_ViewCount");

                entity.HasQueryFilter(e => !e.IsDeleted);

            });

            modelBuilder.Entity<ItemMetadata>(entity =>
            {
                entity.Property(e => e.Key).HasMaxLength(100);
                entity.Property(e => e.Value).HasMaxLength(500);

                entity.HasOne(d => d.CulturalItem).WithMany(p => p.Metadata)
                    .HasForeignKey(d => d.CulturalItemId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ItemMetadata_CulturalItemId");

                entity.HasIndex(e => new {e.CulturalItemId, e.Key, e.Value},
                        "UQ_ItemMetadata_CulturalItemId_Key_Value")
                        .IsUnique();


                entity.HasIndex(e => new { e.Key, e.Value }, "IX_ItemMetadata_KeyValue");

            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.EntityName).HasMaxLength(100);
                entity.Property(e => e.ChangedColumns).HasMaxLength(500);

                entity.Property(e => e.Action)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                entity.Property(e => e.OldValues).HasColumnType("jsonb");
                entity.Property(e => e.NewValues).HasColumnType("jsonb");

                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AuditLogs_UserId");
            });
        }
    }
}
