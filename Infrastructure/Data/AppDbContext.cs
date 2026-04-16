using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;

namespace TaskFlow.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Domain.Models.Task> Tasks { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(e =>
        {
            e.Property(p => p.Id)
                .HasDefaultValueSql("NEWID()");

            e.HasMany(p => p.Members)
                .WithOne(m => m.Project)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Member>(e =>
        {
            e.Property(m => m.Id)
                .HasDefaultValueSql("NEWID()");

            e.HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(m => new { m.UserId, m.ProjectId })
                .IsUnique();
        });

        builder.Entity<Domain.Models.Task>(e =>
        {
            e.Property(t => t.Id)
                .HasDefaultValueSql("NEWID()");

            e.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.AssigneeMember)
                .WithMany(am => am.AssignedTasks)
                .HasForeignKey(t => t.AssigneeMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.CreatorMember)
                .WithMany(cm => cm.CreatedTasks)
                .HasForeignKey(t => t.CreatorMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Comment>(e =>
        {
            e.Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            e.HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.Member)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Attachment>(e =>
        {
            e.Property(a => a.Id)
                .HasDefaultValueSql("NEWID()");

            e.HasOne(a => a.Task)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(a => a.Member)
                .WithMany(m => m.Attachments)
                .HasForeignKey(a => a.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}