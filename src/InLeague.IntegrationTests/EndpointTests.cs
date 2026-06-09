using InLeague.Application.Features.Auth.DTOs;
using InLeague.Application.Features.Leagues.DTOs;
using InLeague.Application.Features.Drivers.DTOs;
using InLeague.Application.Features.Karts.DTOs;
using InLeague.Application.Features.Races.DTOs;
using InLeague.Application.Features.RaceResults.DTOs;
using InLeague.Domain.Features.Races.Enums;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InLeague.IntegrationTests
{
    public class AuthTests : BaseIntegrationTest
    {
        public AuthTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task Auth_FullFlow()
        {
            var email = $"flow_{Guid.NewGuid()}@example.com";
            var regReq = new RegisterRequestDto { Email = email, Password = "Password123!", FirstName = "First", LastName = "Last" };

            // 1. Register
            var regResp = await _client.PostAsJsonAsync("/api/Auth/register", regReq);
            regResp.StatusCode.Should().Be(HttpStatusCode.Created);

            // 2. Login
            var loginResp = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto { Email = email, Password = "Password123!" });
            loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResp.Content.ReadFromJsonAsync<AuthResponseDto>();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // 3. Me
            var meResp = await _client.GetAsync("/api/Auth/me");
            meResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var meResult = await meResp.Content.ReadFromJsonAsync<UserInfoDto>();
            meResult!.Email.Should().Be(email.ToLower());
        }
    }

    public class LeaguesTests : BaseIntegrationTest
    {
        public LeaguesTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task Leagues_FullCRUD()
        {
            await AuthenticateAsync($"leagues_{Guid.NewGuid()}@test.com");

            // 1. Create
            var createReq = new CreateLeagueDto { Name = "Valid League Name", StartDate = DateOnly.FromDateTime(DateTime.Now) };
            var createResp = await _client.PostAsJsonAsync("/api/Leagues", createReq);
            createResp.StatusCode.Should().Be(HttpStatusCode.Created);
            var league = await createResp.Content.ReadFromJsonAsync<LeagueDto>();

            // 2. Get All
            var getAllResp = await _client.GetAsync("/api/Leagues");
            var all = await getAllResp.Content.ReadFromJsonAsync<List<LeagueDto>>();
            all.Should().Contain(l => l.Id == league!.Id);

            // 3. Get By Id
            var getByIdResp = await _client.GetAsync($"/api/Leagues/{league!.Id}");
            getByIdResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // 4. Update
            var updateReq = new UpdateLeagueDto { Name = "Updated League Name", IsActive = false };
            var updateResp = await _client.PutAsJsonAsync($"/api/Leagues/{league.Id}", updateReq);
            updateResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // 5. Delete
            var deleteResp = await _client.DeleteAsync($"/api/Leagues/{league.Id}");
            deleteResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 6. Verify Deleted
            var verifyResp = await _client.GetAsync($"/api/Leagues/{league.Id}");
            verifyResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

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

            // Create
            var createResp = await _client.PostAsJsonAsync($"/api/leagues/{league!.Id}/races", new CreateRaceDto { Name = "Grand Prix", RaceDate = DateTime.UtcNow, NumberOfLaps = 10 });
            createResp.StatusCode.Should().Be(HttpStatusCode.Created);
            var race = await createResp.Content.ReadFromJsonAsync<RaceDto>();

            // Get By League
            (await _client.GetAsync($"/api/leagues/{league.Id}/races")).StatusCode.Should().Be(HttpStatusCode.OK);
            
            // Get By Id
            (await _client.GetAsync($"/api/leagues/{league.Id}/races/{race!.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);

            // Update
            (await _client.PutAsJsonAsync($"/api/leagues/{league.Id}/races/{race.Id}", new UpdateRaceDto { Name = "Grand Prix Updated" })).StatusCode.Should().Be(HttpStatusCode.OK);

            // Delete
            (await _client.DeleteAsync($"/api/leagues/{league.Id}/races/{race.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }

    public class RaceResultsTests : BaseIntegrationTest
    {
        public RaceResultsTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task Results_FullCRUD()
        {
            await AuthenticateAsync($"results_{Guid.NewGuid()}@test.com");

            // Setup
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

            // Create Result
            var req = new CreateRaceResultDto { DriverId = d.Id, KartId = k.Id, LapTimeMs = 100, TotalTimeMs = 1000, StartingPosition = 1, FinishingPosition = 1, LapsCompleted = 1, Status = ResultStatus.Finished };
            var createResp = await _client.PostAsJsonAsync($"/api/races/{r.Id}/results", req);
            createResp.StatusCode.Should().Be(HttpStatusCode.Created);
            var res = await createResp.Content.ReadFromJsonAsync<RaceResultDto>();

            // Get By Race
            (await _client.GetAsync($"/api/races/{r.Id}/results")).StatusCode.Should().Be(HttpStatusCode.OK);

            // Get By Id (New endpoint)
            var getByIdResp = await _client.GetAsync($"/api/races/{r.Id}/results/{res!.Id}");
            getByIdResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var getByIdResult = await getByIdResp.Content.ReadFromJsonAsync<RaceResultDto>();
            getByIdResult.Should().NotBeNull();
            getByIdResult!.DriverName.Should().Be("Fast Driver");
            getByIdResult.KartNumber.Should().Be(k.Number);

            // Update
            (await _client.PutAsJsonAsync($"/api/races/{r.Id}/results/{res!.Id}", new UpdateRaceResultDto { Status = ResultStatus.DNF })).StatusCode.Should().Be(HttpStatusCode.OK);

            // Delete
            (await _client.DeleteAsync($"/api/races/{r.Id}/results/{res.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
