using Microsoft.EntityFrameworkCore;
using UserAccount.Api.Features.Accounts.CreateAccount;

namespace UserAccount.Api.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserAccountEntity> UserAccounts => Set<UserAccountEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
