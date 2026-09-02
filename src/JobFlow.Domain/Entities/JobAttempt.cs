using JobFlow.Domain.Enums;

namespace JobFlow.Domain.Entities;

public class JobAttempt
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid? WorkerId { get; set; }
    public int AttemptNumber { get; set; }
    public JobAttemptStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public string? Error { get; set; }

    public Job Job { get; set; } = null!;
    public Worker? Worker { get; set; }
}
