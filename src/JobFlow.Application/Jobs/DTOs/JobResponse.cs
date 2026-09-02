using JobFlow.Domain.Enums;

namespace JobFlow.Application.Jobs.DTOs;

public class JobResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public int Attempts { get; set; }
    public Guid? WorkerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
