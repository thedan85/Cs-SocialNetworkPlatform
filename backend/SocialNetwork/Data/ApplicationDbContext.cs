using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SocialNetwork.Model;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace SocialNetwork.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Hashtag> Hashtags { get; set; }
    public DbSet<PostHashtag> PostHashtags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<UserFriendship> UserFriendships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(500);
            
            entity.Property(e => e.Bio)
                .HasMaxLength(1000);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });

        // Configure Post entity
        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");

            entity.HasKey(e => e.PostId);

            entity.Property(e => e.PostId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.Content)
                .HasColumnType("TEXT")
                .IsRequired();

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            entity.Property(e => e.LikeCount)
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Hashtag entity
        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.ToTable("Hashtag");

            entity.HasKey(e => e.HashtagId);

            entity.Property(e => e.HashtagId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.Tag)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(e => e.Tag)
                .IsUnique();

            entity.Property(e => e.UsageCount)
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure PostHashtag entity
        modelBuilder.Entity<PostHashtag>(entity =>
        {
            entity.ToTable("PostHashtag");

            // Composite primary key
            entity.HasKey(e => new { e.PostId, e.HashtagId });

            entity.Property(e => e.PostId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.HashtagId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationships
            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Hashtag)
                .WithMany()
                .HasForeignKey(e => e.HashtagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Comment entity
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.HasKey(e => e.CommentId);

            entity.Property(e => e.CommentId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.PostId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.Content)
                .HasColumnType("TEXT")
                .IsRequired();

            entity.Property(e => e.LikeCount)
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            // Foreign key relationships
            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Story entity
        modelBuilder.Entity<Story>(entity =>
        {
            entity.ToTable("Story");

            entity.HasKey(e => e.StoryId);

            entity.Property(e => e.StoryId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.Content)
                .IsRequired();

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Like entity
        modelBuilder.Entity<Like>(entity =>
        {
            entity.ToTable("Like");

            entity.HasKey(e => e.LikeId);

            entity.Property(e => e.LikeId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.PostId)
                .HasMaxLength(36);

            entity.Property(e => e.CommentId)
                .HasMaxLength(36);

            entity.Property(e => e.StoryId)
                .HasMaxLength(36);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationships
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Comment)
                .WithMany()
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Story)
                .WithMany()
                .HasForeignKey(e => e.StoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }); 

        // Configure Friendship entity
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("Friendship");

            entity.HasKey(e => e.FriendshipId);

            entity.Property(e => e.FriendshipId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.UserId1)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.UserId2)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            // Foreign key relationships
            entity.HasOne(e => e.User1)
                .WithMany()
                .HasForeignKey(e => e.UserId1)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User2)
                .WithMany()
                .HasForeignKey(e => e.UserId2)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserFriendship entity
        modelBuilder.Entity<UserFriendship>(entity =>
        {
            entity.ToTable("UserFriendship");

            // Composite primary key
            entity.HasKey(e => new { e.FriendshipId, e.UserId });

            entity.Property(e => e.FriendshipId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.AcceptedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationships
            entity.HasOne(e => e.Friendship)
                .WithMany()
                .HasForeignKey(e => e.FriendshipId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
