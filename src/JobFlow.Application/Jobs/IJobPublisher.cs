using JobFlow.Domain.Entities;

namespace JobFlow.Application.Jobs;

public interface IJobPublisher
{
    Task PublishAsync(
        Job job,
        CancellationToken cancellationToken);
}
