using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InLeague.Features.RaceResults;

[ApiController]
[Route("api/races/{raceId:guid}/results")]
public class RaceResultsController : ControllerBase
{
    private readonly IRaceResultService _service;

    public RaceResultsController(IRaceResultService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByRaceId(Guid raceId)
    {
        var results = await _service.GetByRaceIdAsync(raceId);
        return Ok(results);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid raceId, Guid id)
    {
        var result = await _service.GetByIdAsync(raceId, id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Guid raceId, [FromBody] CreateRaceResultDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(raceId, dto);
            if (created is null)
                return BadRequest(new { error = "Nieprawidłowe dane (wyścig, zawodnik lub gokart nie istnieje)" });
                
            return CreatedAtAction(nameof(GetById), new { raceId, id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid raceId, Guid id, [FromBody] UpdateRaceResultDto dto)
    {
        var updated = await _service.UpdateAsync(raceId, id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid raceId, Guid id)
    {
        var success = await _service.DeleteAsync(raceId, id);
        return success ? NoContent() : NotFound();
    }
}
