using JobFlow.Application.Jobs.DTOs;
using JobFlow.Domain.Entities;
using JobFlow.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JobFlow.Application.Jobs;

public class JobService : IJobService
{
    private readonly IJobRepository _jobs;
    private readonly IJobPublisher _publisher;
    private readonly ILogger<JobService> _logger;

    public JobService(
        IJobRepository jobs,
        IJobPublisher publisher,
        ILogger<JobService> logger)
    {
        _jobs = jobs;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<JobResponse> CreateAsync(
        CreateJobRequest request,
        CancellationToken cancellationToken)
    {
        var job = Job.Create(request.Type, request.Payload);

        await _jobs.AddAsync(job, cancellationToken);

        job.Status = JobStatus.Queued;
        job.UpdatedAt = DateTime.UtcNow;
        await _jobs.UpdateAsync(job, cancellationToken);

        await _publisher.PublishAsync(job, cancellationToken);

        _logger.LogInformation("Created and queued job {JobId} of type {JobType}", job.Id, job.Type);

        return ToResponse(job);
    }

    public async Task<JobResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var job = await _jobs.GetByIdAsync(id, cancellationToken);
        return job is null ? null : ToResponse(job);
    }

    private static JobResponse ToResponse(Job job) => new()
    {
        Id = job.Id,
        Type = job.Type,
        Status = job.Status,
        Attempts = job.Attempts,
        WorkerId = job.WorkerId,
        CreatedAt = job.CreatedAt,
        StartedAt = job.StartedAt,
        CompletedAt = job.CompletedAt
    };
}
