using JobFlow.Domain.Entities;

namespace JobFlow.Application.JobAttempts;

public interface IJobAttemptRepository
{
    Task AddAsync(
        JobAttempt attempt,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        JobAttempt attempt,
        CancellationToken cancellationToken);
}
