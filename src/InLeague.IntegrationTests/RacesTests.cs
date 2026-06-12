using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InLeague.IntegrationTests;

public class RacesTests : BaseIntegrationTest
{
    public RacesTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Races_FullCRUD()
    {
        await AuthenticateAsync($"races_{Guid.NewGuid()}@test.com");

        var leagueResp = await _client.PostAsJsonAsync("/api/Leagues", new CreateLeagueDto { Name = "Race League Name", StartDate = DateOnly.FromDateTime(DateTime.Now) });
        leagueResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var league = await leagueResp.Content.ReadFromJsonAsync<LeagueDto>();

        var createResp = await _client.PostAsJsonAsync($"/api/leagues/{league!.Id}/races", new CreateRaceDto { Name = "Grand Prix", RaceDate = DateTime.UtcNow, NumberOfLaps = 10 });
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var race = await createResp.Content.ReadFromJsonAsync<RaceDto>();

        (await _client.GetAsync($"/api/leagues/{league.Id}/races")).StatusCode.Should().Be(HttpStatusCode.OK);
            
        (await _client.GetAsync($"/api/leagues/{league.Id}/races/{race!.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);

        (await _client.PutAsJsonAsync($"/api/leagues/{league.Id}/races/{race.Id}", new UpdateRaceDto { Name = "Grand Prix Updated" })).StatusCode.Should().Be(HttpStatusCode.OK);

        (await _client.DeleteAsync($"/api/leagues/{league.Id}/races/{race.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}