using JobFlow.Domain.Entities;

namespace JobFlow.Application.Workers;

public interface IWorkerRepository
{
    Task AddAsync(
        Worker worker,
        CancellationToken cancellationToken);

    Task<Worker?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Worker worker,
        CancellationToken cancellationToken);
}
