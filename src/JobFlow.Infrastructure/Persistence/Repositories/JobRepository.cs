using JobFlow.Application.Jobs;
using JobFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobFlow.Infrastructure.Persistence.Repositories;

public class JobRepository : IJobRepository
{
    private readonly JobFlowDbContext _context;

    public JobRepository(JobFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Job job, CancellationToken cancellationToken)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Jobs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken)
    {
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
