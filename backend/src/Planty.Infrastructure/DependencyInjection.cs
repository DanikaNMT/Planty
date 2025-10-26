namespace Planty.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddDbContext<PlantDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPlantRepository, PlantRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        services.AddScoped<IWateringRepository, WateringRepository>();
        services.AddScoped<IFertilizationRepository, FertilizationRepository>();
        services.AddScoped<IPlantPictureRepository, PlantPictureRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordService, PasswordService>();

        // Register MediatR handlers from this assembly (for WaterPlantCommandHandler)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}