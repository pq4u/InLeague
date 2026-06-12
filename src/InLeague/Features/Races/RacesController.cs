using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InLeague.Features.Races;

[ApiController]
[Route("api/leagues/{leagueId:guid}/races")]
public class RacesController : ControllerBase
{
    private readonly IRaceService _service;

    public RacesController(IRaceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByLeagueId(Guid leagueId)
    {
        var races = await _service.GetByLeagueIdAsync(leagueId);
        return Ok(races);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid leagueId, Guid id)
    {
        var race = await _service.GetByIdAsync(id);
        if (race is null || race.LeagueId != leagueId)
            return NotFound();
            
        return Ok(race);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Guid leagueId, [FromBody] CreateRaceDto dto)
    {
        var created = await _service.CreateAsync(leagueId, dto);
        if (created is null) return NotFound(new { error = "Liga nie istnieje" });
        
        return CreatedAtAction(nameof(GetById), new { leagueId, id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid leagueId, Guid id, [FromBody] UpdateRaceDto dto)
    {
        var race = await _service.GetByIdAsync(id);
        if (race is null || race.LeagueId != leagueId)
            return NotFound();

        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid leagueId, Guid id)
    {
        var race = await _service.GetByIdAsync(id);
        if (race is null || race.LeagueId != leagueId)
            return NotFound();

        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
