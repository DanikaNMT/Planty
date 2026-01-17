using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planty.Infrastructure;
using Planty.Infrastructure.Data;

Console.WriteLine("Planty Database Migration Tool");
Console.WriteLine("==============================");

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Production.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Setup services with warning suppression for pending model changes
// This is necessary because migrations were originally created with SQLite
// but the database has been migrated to PostgreSQL with the correct schema
var services = new ServiceCollection();
services.AddInfrastructure(configuration);

// Override the DbContext configuration to suppress the pending model changes warning
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<PlantDbContext>((serviceProvider, options) =>
{
    if (connectionString != null && connectionString.Contains("Host="))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString ?? "Data Source=plants.db");
    }
    
    // Suppress the pending model changes warning during migration
    options.ConfigureWarnings(warnings => 
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
}, ServiceLifetime.Scoped);

// Run migrations
using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<PlantDbContext>();

Console.WriteLine("Applying database migrations...");
try
{    
    context.Database.Migrate();
    Console.WriteLine("✓ Database migrations applied successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Migration failed: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Environment.Exit(1);
}
