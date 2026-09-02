namespace JobFlow.Application.Workers;

public interface IJobProcessor
{
    Task ProcessAsync(
        Guid jobId,
        Guid workerId,
        CancellationToken cancellationToken);
}
