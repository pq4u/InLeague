using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InLeague.IntegrationTests;

public class AuthTests : BaseIntegrationTest
{
    public AuthTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Auth_FullFlow()
    {
        var email = $"flow_{Guid.NewGuid()}@example.com";
        var regReq = new RegisterRequestDto { Email = email, Password = "Password123!", FirstName = "First", LastName = "Last" };

        var regResp = await _client.PostAsJsonAsync("/api/Auth/register", regReq);
        regResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginResp = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto { Email = email, Password = "Password123!" });
        loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResult = await loginResp.Content.ReadFromJsonAsync<AuthResponseDto>();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var meResp = await _client.GetAsync("/api/Auth/me");
        meResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var meResult = await meResp.Content.ReadFromJsonAsync<UserInfoDto>();
        meResult!.Email.Should().Be(email.ToLower());
    }
}