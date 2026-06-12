using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InLeague.IntegrationTests;

public class KartsTests : BaseIntegrationTest
{
    public KartsTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Karts_FullCRUD()
    {
        await AuthenticateAsync($"karts_{Guid.NewGuid()}@test.com");

        var createReq = new CreateKartDto { Number = "K-" + Guid.NewGuid().ToString().Substring(0,4) };
        var createResp = await _client.PostAsJsonAsync("/api/Karts", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var kart = await createResp.Content.ReadFromJsonAsync<KartDto>();

        (await _client.GetAsync("/api/Karts")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.GetAsync($"/api/Karts/{kart!.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.PutAsJsonAsync($"/api/Karts/{kart.Id}", new UpdateKartDto { Model = "New Model" })).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.DeleteAsync($"/api/Karts/{kart.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}