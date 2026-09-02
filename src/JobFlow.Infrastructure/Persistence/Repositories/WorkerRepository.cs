using JobFlow.Application.Workers;
using JobFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobFlow.Infrastructure.Persistence.Repositories;

public class WorkerRepository : IWorkerRepository
{
    private readonly JobFlowDbContext _context;

    public WorkerRepository(JobFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Worker worker, CancellationToken cancellationToken)
    {
        _context.Workers.Add(worker);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Worker?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Workers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Worker worker, CancellationToken cancellationToken)
    {
        _context.Workers.Update(worker);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
