using AIIncidentAnalysisAuthServiceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AIIncidentAnalysisAuthServiceAPI.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<PoliceOfficer>(options)
{
    public DbSet<Token> Tokens { get; init; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}