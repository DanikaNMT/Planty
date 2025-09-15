using Microsoft.EntityFrameworkCore;
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

// Setup services
var services = new ServiceCollection();
services.AddInfrastructure(configuration);

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
