namespace Planty.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Planty.Application.Interfaces;
using Planty.Application.Services;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;
using Planty.Infrastructure.Repositories;
using Planty.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Determine which database provider to use based on connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (connectionString != null && connectionString.Contains("Host="))
        {
            // PostgreSQL connection string detected
            services.AddDbContext<PlantDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
        else
        {
            // Default to SQLite for backward compatibility
            services.AddDbContext<PlantDbContext>(options =>
                options.UseSqlite(connectionString ?? "Data Source=plants.db"));
        }

        services.AddScoped<IPlantRepository, PlantRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        services.AddScoped<IWateringRepository, WateringRepository>();
        services.AddScoped<IFertilizationRepository, FertilizationRepository>();
        services.AddScoped<IPlantPictureRepository, PlantPictureRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShareRepository, ShareRepository>();

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IPermissionService, PermissionService>();

        // Register MediatR handlers from this assembly (for WaterPlantCommandHandler)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}