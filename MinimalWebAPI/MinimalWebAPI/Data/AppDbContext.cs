using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MinimalWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Note> Notes => Set<Note>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Roles.Ad
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NoteTypeConfiguration());
        }
    }

    public class UserTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Password).IsRequired();
            builder.Property(u => u.Name).IsRequired();
            builder.Property(u => u.CreatedTime).HasDefaultValueSql("getdate()");
            builder.Property(u => u.ChangedTime).HasDefaultValueSql("getdate()");
            builder.HasOne(u => u.Role).WithMany(u => u.Users).HasForeignKey(u => u.RoleId);
            builder.HasMany(u => u.Notes).WithOne(u => u.User).HasForeignKey(u => u.UserId);
        }
    }

    public class RoleTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.RoleName).IsRequired();
        }
    }
    public class NoteTypeConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Title).IsRequired();
            builder.Property(u => u.CreatedTime).HasDefaultValueSql("getdate()");
            builder.Property(u => u.ChangedTime).HasDefaultValueSql("getdate()");
        }
    }

}


