using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SocialNetwork.Helpers;
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
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<UserFriendship> UserFriendships { get; set; }
    public DbSet<PostReport> PostReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.UserName);

            entity.HasIndex(e => e.Email);
            
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(500);
            
            entity.Property(e => e.Bio)
                .HasMaxLength(1000);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });

        // Configure Post entity
        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");

            entity.HasKey(e => e.PostId);

            entity.HasIndex(e => e.CreatedAt);

            entity.HasIndex(e => new { e.UserId, e.CreatedAt });

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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Comments)
                .WithOne(e => e.Post)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Likes)
                .WithOne(e => e.Post)
                .HasForeignKey(e => e.PostId)
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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            // Foreign key relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.PostHashtags)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Hashtag)
                .WithMany(h => h.PostHashtags)
                .HasForeignKey(e => e.HashtagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Comment entity
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.HasKey(e => e.CommentId);

            entity.HasIndex(e => new { e.PostId, e.CreatedAt });

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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            // Foreign key relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Story entity
        modelBuilder.Entity<Story>(entity =>
        {
            entity.ToTable("Story");

            entity.HasKey(e => e.StoryId);

            entity.HasIndex(e => new { e.ExpiresAt, e.CreatedAt });

            entity.HasIndex(e => new { e.UserId, e.ExpiresAt, e.CreatedAt });

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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany(u => u.Stories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Like entity
        modelBuilder.Entity<Like>(entity =>
        {
            entity.ToTable("Like", table =>
            {
                table.HasCheckConstraint(
                    "CK_Like_ExactlyOneTarget",
                    "(CASE WHEN PostId IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN CommentId IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN StoryId IS NOT NULL THEN 1 ELSE 0 END) = 1");
            });

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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            // Foreign key relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Story)
                .WithMany(s => s.Likes)
                .HasForeignKey(e => e.StoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }); 

        // Configure Friendship entity
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("Friendship", table =>
            {
                table.HasCheckConstraint(
                    "CK_Friendship_DifferentUsers",
                    "UserId1 <> UserId2");
            });

            entity.HasKey(e => e.FriendshipId);

            entity.HasIndex(e => new { e.Status, e.UserId1, e.UpdatedAt });

            entity.HasIndex(e => new { e.UserId2, e.Status, e.CreatedAt });

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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            // Foreign key relationships
            entity.HasOne(e => e.User1)
                .WithMany(u => u.FriendshipsAsUser1)
                .HasForeignKey(e => e.UserId1)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User2)
                .WithMany(u => u.FriendshipsAsUser2)
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
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            // Foreign key relationships
            entity.HasOne(e => e.Friendship)
                .WithMany(f => f.UserFriendships)
                .HasForeignKey(e => e.FriendshipId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserFriendships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Notification entity
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.HasKey(e=>e.NotificationId);

            entity.HasIndex(e => new { e.RecipientUserId, e.IsRead, e.CreatedAt });

            entity.Property(e => e.NotificationId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.RecipientUserId)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.SenderUserId)
                .HasMaxLength(128)
                .IsRequired();  

            entity.Property(e => e.Type)
                .HasMaxLength(100);

            entity.Property(e => e.Content)
                .HasColumnType("TEXT");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.IsRead)
                .HasDefaultValue(false);

            // Foreign key relationships
            entity.HasOne(e => e.RecipientUser)
                .WithMany(u => u.ReceivedNotifications)
                .HasForeignKey(e => e.RecipientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SenderUser)
                .WithMany(u => u.SentNotifications)
                .HasForeignKey(e => e.SenderUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostReport>(entity =>
        {
            entity.ToTable("PostReport");

            entity.HasKey(e => e.PostReportId);

            entity.HasIndex(e => new { e.Status, e.CreatedAt });

            entity.Property(e => e.PostReportId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.PostId)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.ReporterUserId)
                .HasMaxLength(128)
                .IsRequired();
            
            entity.Property(e => e.Reason)
                .HasMaxLength(500);
            
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            
            // Foreign key relationships
            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.ReporterUser)
                .WithMany(u => u.PostReports)
                .HasForeignKey(e => e.ReporterUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        );
    }
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        LocalEnvironmentLoader.LoadFromDirectory(Directory.GetCurrentDirectory());

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
