using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InLeague.IntegrationTests;

public class RaceResultsTests : BaseIntegrationTest
{
    public RaceResultsTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Results_FullCRUD()
    {
        await AuthenticateAsync($"results_{Guid.NewGuid()}@test.com");

        var lResp = await _client.PostAsJsonAsync("/api/Leagues", new CreateLeagueDto { Name = "League For Results", StartDate = DateOnly.FromDateTime(DateTime.Now) });
        lResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var l = (await lResp.Content.ReadFromJsonAsync<LeagueDto>())!;

        var rResp = await _client.PostAsJsonAsync($"/api/leagues/{l.Id}/races", new CreateRaceDto { Name = "Race For Results", RaceDate = DateTime.UtcNow, NumberOfLaps = 10 });
        rResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var r = (await rResp.Content.ReadFromJsonAsync<RaceDto>())!;

        var dResp = await _client.PostAsJsonAsync("/api/Drivers", new CreateDriverDto { FirstName = "Fast", LastName = "Driver" });
        dResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var d = (await dResp.Content.ReadFromJsonAsync<DriverDto>())!;

        var kResp = await _client.PostAsJsonAsync("/api/Karts", new CreateKartDto { Number = "Kart-" + Guid.NewGuid().ToString().Substring(0,3) });
        kResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var k = (await kResp.Content.ReadFromJsonAsync<KartDto>())!;

        var req = new CreateRaceResultDto { DriverId = d.Id, KartId = k.Id, LapTimeMs = 100, TotalTimeMs = 1000, StartingPosition = 1, FinishingPosition = 1, LapsCompleted = 1, Status = ResultStatus.Finished };
        var createResp = await _client.PostAsJsonAsync($"/api/races/{r.Id}/results", req);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var res = await createResp.Content.ReadFromJsonAsync<RaceResultDto>();

        (await _client.GetAsync($"/api/races/{r.Id}/results")).StatusCode.Should().Be(HttpStatusCode.OK);

        var getByIdResp = await _client.GetAsync($"/api/races/{r.Id}/results/{res!.Id}");
        getByIdResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var getByIdResult = await getByIdResp.Content.ReadFromJsonAsync<RaceResultDto>();
        getByIdResult.Should().NotBeNull();
        getByIdResult!.DriverName.Should().Be("Fast Driver");
        getByIdResult.KartNumber.Should().Be(k.Number);

        (await _client.PutAsJsonAsync($"/api/races/{r.Id}/results/{res!.Id}", new UpdateRaceResultDto { Status = ResultStatus.DNF })).StatusCode.Should().Be(HttpStatusCode.OK);

        (await _client.DeleteAsync($"/api/races/{r.Id}/results/{res.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}