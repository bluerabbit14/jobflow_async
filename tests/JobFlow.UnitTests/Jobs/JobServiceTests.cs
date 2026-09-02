using JobFlow.Application.Jobs;
using JobFlow.Application.Jobs.DTOs;
using JobFlow.Domain.Entities;
using JobFlow.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;

namespace JobFlow.UnitTests.Jobs;

public class JobServiceTests
{
    [Fact]
    public async Task CreateAsync_saves_job_as_queued_and_publishes_it()
    {
        var jobs = new InMemoryJobRepository();
        var publisher = new FakeJobPublisher();
        var service = new JobService(jobs, publisher, NullLogger<JobService>.Instance);
        var request = new CreateJobRequest
        {
            Type = "EmailNotification",
            Payload = "{\"subject\":\"Welcome\"}"
        };

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("EmailNotification", result.Type);
        Assert.Equal(JobStatus.Queued, result.Status);
        Assert.Equal(0, result.Attempts);
        Assert.Single(publisher.Published);
        Assert.Equal(result.Id, publisher.Published[0].Id);

        var stored = await jobs.GetByIdAsync(result.Id, CancellationToken.None);
        Assert.NotNull(stored);
        Assert.Equal(JobStatus.Queued, stored.Status);
    }

    [Fact]
    public async Task GetByIdAsync_returns_job_when_it_exists()
    {
        var jobs = new InMemoryJobRepository();
        var job = Job.Create("GenerateReport", "{}");
        job.Status = JobStatus.Queued;
        await jobs.AddAsync(job, CancellationToken.None);

        var service = new JobService(jobs, new FakeJobPublisher(), NullLogger<JobService>.Instance);

        var result = await service.GetByIdAsync(job.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(job.Id, result.Id);
        Assert.Equal("GenerateReport", result.Type);
        Assert.Equal(JobStatus.Queued, result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_job_does_not_exist()
    {
        var service = new JobService(
            new InMemoryJobRepository(),
            new FakeJobPublisher(),
            NullLogger<JobService>.Instance);

        var result = await service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    private sealed class InMemoryJobRepository : IJobRepository
    {
        private readonly Dictionary<Guid, Job> _jobs = new();

        public Task AddAsync(Job job, CancellationToken cancellationToken)
        {
            _jobs[job.Id] = job;
            return Task.CompletedTask;
        }

        public Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _jobs.TryGetValue(id, out var job);
            return Task.FromResult(job);
        }

        public Task UpdateAsync(Job job, CancellationToken cancellationToken)
        {
            _jobs[job.Id] = job;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeJobPublisher : IJobPublisher
    {
        public List<Job> Published { get; } = [];

        public Task PublishAsync(Job job, CancellationToken cancellationToken)
        {
            Published.Add(job);
            return Task.CompletedTask;
        }
    }
}
