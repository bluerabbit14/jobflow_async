using JobFlow.Application.JobAttempts;
using JobFlow.Domain.Entities;

namespace JobFlow.Infrastructure.Persistence.Repositories;

public class JobAttemptRepository : IJobAttemptRepository
{
    private readonly JobFlowDbContext _context;

    public JobAttemptRepository(JobFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(JobAttempt attempt, CancellationToken cancellationToken)
    {
        _context.JobAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(JobAttempt attempt, CancellationToken cancellationToken)
    {
        _context.JobAttempts.Update(attempt);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
