using JobFlow.Domain.Enums;

namespace JobFlow.Domain.Entities;

public class Job
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public int Attempts { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? WorkerId { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Worker? Worker { get; set; }
    public ICollection<JobAttempt> JobAttempts { get; set; } = new List<JobAttempt>();

    public static Job Create(string type, string payload)
    {
        var now = DateTime.UtcNow;

        return new Job
        {
            Id = Guid.NewGuid(),
            Type = type,
            Payload = payload,
            Status = JobStatus.Pending,
            Attempts = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
