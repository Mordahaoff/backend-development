using Microsoft.EntityFrameworkCore;

namespace ClientApi.Models;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Login)
                .HasColumnName("login")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.HashPassword)
                .HasColumnName("hash_password")
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(u => u.Login)
                .IsUnique();

            entity.ToTable(t => t.HasCheckConstraint("CK_User_Login_Length", "LENGTH(LOGIN) BETWEEN 3 AND 50"));
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.FullName)
                .HasColumnName("full_name")
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Phone)
                .HasColumnName("phone")
                .HasMaxLength(10);

            entity.Property(c => c.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Discount)
                .HasColumnName("discount")
                .HasDefaultValue(0)
                .HasPrecision(5, 2);

            entity.Property(c => c.Verified)
                .HasColumnName("verified")
                .HasDefaultValue(false);

            entity.HasIndex(c => c.Email)
                .IsUnique();

            entity.ToTable(t => t.HasCheckConstraint("CK_Client_Discount_Range", "discount >= 0 AND discount <= 100"));
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.HasIndex(rt => rt.Token)
                .IsUnique();

            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(rt => rt.UserId)
                .HasColumnName("user_id");

            entity.Property(rt => rt.Token)
                .HasColumnName("token")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(rt => rt.ExpiresAt)
                .HasColumnName("expires_at");

            entity.Property(rt => rt.IsRevoked)
                .HasColumnName("is_revoked")
                .HasDefaultValue(false);

            entity.Property(rt => rt.ReplacedByToken)
                .HasColumnName("replaced_by_token");

            entity.Property(rt => rt.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(rt => new { rt.UserId, rt.ExpiresAt, rt.IsRevoked });
        });
    }
}

// dotnet ef migrations add InitialCreate
// dotnet ef database update