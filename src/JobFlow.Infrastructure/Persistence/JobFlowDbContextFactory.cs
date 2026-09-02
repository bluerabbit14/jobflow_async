using JobFlow.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobFlow.Infrastructure.Persistence;

public class JobFlowDbContextFactory : IDesignTimeDbContextFactory<JobFlowDbContext>
{
    public JobFlowDbContext CreateDbContext(string[] args)
    {
        DotEnv.Load();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is not set. Add it to the .env file.");

        var options = new DbContextOptionsBuilder<JobFlowDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new JobFlowDbContext(options);
    }
}
