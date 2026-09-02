using JobFlow.Domain.Entities;
using JobFlow.Infrastructure.Persistence;
using JobFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JobFlow.IntegrationTests;

public class JobRepositoryTests
{
    [Fact]
    public async Task AddAsync_then_GetByIdAsync_returns_the_job()
    {
        var options = new DbContextOptionsBuilder<JobFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new JobFlowDbContext(options);
        var repository = new JobRepository(context);
        var job = Job.Create("EmailNotification", "{\"subject\":\"Welcome\"}");

        await repository.AddAsync(job, CancellationToken.None);
        var found = await repository.GetByIdAsync(job.Id, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(job.Id, found.Id);
        Assert.Equal("EmailNotification", found.Type);
        Assert.Equal("{\"subject\":\"Welcome\"}", found.Payload);
    }
}
