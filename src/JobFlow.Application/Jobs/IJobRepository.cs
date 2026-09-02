using JobFlow.Domain.Entities;

namespace JobFlow.Application.Jobs;

public interface IJobRepository
{
    Task AddAsync(
        Job job,
        CancellationToken cancellationToken);

    Task<Job?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Job job,
        CancellationToken cancellationToken);
}
