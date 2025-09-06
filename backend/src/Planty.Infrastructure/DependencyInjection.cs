namespace Planty.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;
using Planty.Infrastructure.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<PlantDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPlantRepository, PlantRepository>();

        return services;
    }
}