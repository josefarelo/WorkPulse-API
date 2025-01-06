using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Commit> Commits { get; set; }
    public DbSet<PullRequest> PullRequests { get; set; }
    public DbSet<Issue> Issues { get; set; }
}
