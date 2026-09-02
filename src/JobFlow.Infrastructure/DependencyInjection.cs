using JobFlow.Application.JobAttempts;
using JobFlow.Application.Jobs;
using JobFlow.Application.Workers;
using JobFlow.Infrastructure.Messaging;
using JobFlow.Infrastructure.Persistence;
using JobFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<JobFlowDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IJobAttemptRepository, JobAttemptRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddSingleton<IJobPublisher, RabbitMqJobPublisher>();

        return services;
    }
}
