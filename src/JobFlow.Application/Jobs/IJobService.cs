using JobFlow.Application.Jobs.DTOs;

namespace JobFlow.Application.Jobs;

public interface IJobService
{
    Task<JobResponse> CreateAsync(
        CreateJobRequest request,
        CancellationToken cancellationToken);

    Task<JobResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}
