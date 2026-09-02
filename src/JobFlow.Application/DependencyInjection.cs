using FluentValidation;
using JobFlow.Application.Jobs;
using JobFlow.Application.Jobs.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace JobFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IJobService, JobService>();
        services.AddValidatorsFromAssemblyContaining<CreateJobRequestValidator>();

        return services;
    }
}
