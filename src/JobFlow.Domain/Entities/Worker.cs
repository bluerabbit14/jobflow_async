using JobFlow.Domain.Enums;

namespace JobFlow.Domain.Entities;

public class Worker
{
    public Guid Id { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public WorkerStatus Status { get; set; }
    public DateTime LastHeartbeat { get; set; }
    public DateTime StartedAt { get; set; }

    public ICollection<JobAttempt> JobAttempts { get; set; } = new List<JobAttempt>();

    public static Worker Create(string hostname)
    {
        var now = DateTime.UtcNow;

        return new Worker
        {
            Id = Guid.NewGuid(),
            Hostname = hostname,
            Status = WorkerStatus.Online,
            StartedAt = now,
            LastHeartbeat = now
        };
    }
}
