using InLeague.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using InLeague.Application.Features.Auth.DTOs;
using System.Net.Http.Json;

namespace InLeague.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly CustomWebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;

        protected BaseIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        protected async Task AuthenticateAsync(string email = "test@test.com", string password = "Password123!", bool asAdmin = true)
        {
            // Register
            await _client.PostAsJsonAsync("/api/Auth/register", new RegisterRequestDto 
            { 
                Email = email, 
                Password = password,
                FirstName = "Test",
                LastName = "User"
            });

            if (asAdmin)
            {
                using var scope = _factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
                var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

                if (user != null && adminRole != null)
                {
                    db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = adminRole.Id });
                    await db.SaveChangesAsync();
                }
            }

            // Login to get token with roles
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto 
            { 
                Email = email, 
                Password = password 
            });

            var result = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);
        }

        protected void ClearDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
