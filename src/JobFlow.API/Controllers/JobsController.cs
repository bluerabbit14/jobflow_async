using JobFlow.Application.Jobs;
using JobFlow.Application.Jobs.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JobFlow.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost]
    public async Task<ActionResult<JobResponse>> Create(
        CreateJobRequest request,
        CancellationToken cancellationToken)
    {
        var job = await _jobService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var job = await _jobService.GetByIdAsync(id, cancellationToken);
        if (job is null)
        {
            return NotFound();
        }

        return Ok(job);
    }
}
