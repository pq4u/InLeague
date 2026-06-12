using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InLeague.IntegrationTests;

public class DriversTests : BaseIntegrationTest
{
    public DriversTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Drivers_FullCRUD()
    {
        await AuthenticateAsync($"drivers_{Guid.NewGuid()}@test.com");

        var createReq = new CreateDriverDto { FirstName = "John", LastName = "Doe", RacingNumber = "10" };
        var createResp = await _client.PostAsJsonAsync("/api/Drivers", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var driver = await createResp.Content.ReadFromJsonAsync<DriverDto>();

        (await _client.GetAsync("/api/Drivers")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.GetAsync($"/api/Drivers/{driver!.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.PutAsJsonAsync($"/api/Drivers/{driver.Id}", new UpdateDriverDto { FirstName = "Johnny" })).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.DeleteAsync($"/api/Drivers/{driver.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}