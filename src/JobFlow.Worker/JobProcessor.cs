using JobFlow.Application.JobAttempts;
using JobFlow.Application.Jobs;
using JobFlow.Application.Workers;
using JobFlow.Domain.Entities;
using JobFlow.Domain.Enums;

namespace JobFlow.Worker;

public class JobProcessor : IJobProcessor
{
    private readonly IJobRepository _jobs;
    private readonly IJobAttemptRepository _attempts;
    private readonly ILogger<JobProcessor> _logger;

    public JobProcessor(
        IJobRepository jobs,
        IJobAttemptRepository attempts,
        ILogger<JobProcessor> logger)
    {
        _jobs = jobs;
        _attempts = attempts;
        _logger = logger;
    }

    public async Task ProcessAsync(
        Guid jobId,
        Guid workerId,
        CancellationToken cancellationToken)
    {
        var job = await _jobs.GetByIdAsync(jobId, cancellationToken)
            ?? throw new InvalidOperationException($"Job '{jobId}' was not found.");

        var startedAt = DateTime.UtcNow;

        job.Status = JobStatus.Processing;
        job.WorkerId = workerId;
        job.StartedAt ??= startedAt;
        job.Attempts++;
        job.UpdatedAt = startedAt;
        await _jobs.UpdateAsync(job, cancellationToken);

        var attempt = new JobAttempt
        {
            Id = Guid.NewGuid(),
            JobId = job.Id,
            WorkerId = workerId,
            AttemptNumber = job.Attempts,
            Status = JobAttemptStatus.Processing,
            StartedAt = startedAt
        };
        await _attempts.AddAsync(attempt, cancellationToken);

        _logger.LogInformation(
            "Worker {WorkerId} is processing job {JobId} (attempt {AttemptNumber})",
            workerId,
            job.Id,
            attempt.AttemptNumber);

        await Task.Delay(1000, cancellationToken);

        var completedAt = DateTime.UtcNow;

        attempt.Status = JobAttemptStatus.Completed;
        attempt.CompletedAt = completedAt;
        attempt.DurationMs = (long)(completedAt - attempt.StartedAt).TotalMilliseconds;
        await _attempts.UpdateAsync(attempt, cancellationToken);

        job.Status = JobStatus.Completed;
        job.CompletedAt = completedAt;
        job.UpdatedAt = completedAt;
        await _jobs.UpdateAsync(job, cancellationToken);

        _logger.LogInformation("Completed job {JobId}", job.Id);
    }
}
