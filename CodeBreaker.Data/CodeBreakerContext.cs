namespace CodeBreaker.Data;

using Microsoft.EntityFrameworkCore;

public class CodeBreakerContext : DbContext
{
    public CodeBreakerContext(DbContextOptions<CodeBreakerContext> options)
        : base(options)
    {
    }

    public DbSet<Game> CodeBreakerItems { get; set; }
}

^
