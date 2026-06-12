using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InLeague.IntegrationTests;

public class LeaguesTests : BaseIntegrationTest
{
    public LeaguesTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Leagues_FullCRUD()
    {
        await AuthenticateAsync($"leagues_{Guid.NewGuid()}@test.com");

        var createReq = new CreateLeagueDto { Name = "Valid League Name", StartDate = DateOnly.FromDateTime(DateTime.Now) };
        var createResp = await _client.PostAsJsonAsync("/api/Leagues", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var league = await createResp.Content.ReadFromJsonAsync<LeagueDto>();

        var getAllResp = await _client.GetAsync("/api/Leagues");
        var all = await getAllResp.Content.ReadFromJsonAsync<List<LeagueDto>>();
        all.Should().Contain(l => l.Id == league!.Id);

        var getByIdResp = await _client.GetAsync($"/api/Leagues/{league!.Id}");
        getByIdResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateReq = new UpdateLeagueDto { Name = "Updated League Name", IsActive = false };
        var updateResp = await _client.PutAsJsonAsync($"/api/Leagues/{league.Id}", updateReq);
        updateResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResp = await _client.DeleteAsync($"/api/Leagues/{league.Id}");
        deleteResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifyResp = await _client.GetAsync($"/api/Leagues/{league.Id}");
        verifyResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}