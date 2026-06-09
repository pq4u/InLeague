using InLeague.Data;
using InLeague.Domain.Features.Races.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Leagues.AnyAsync()) return;

        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId  = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var adminRole = await context.Roles.FindAsync(adminRoleId);
        var userRole  = await context.Roles.FindAsync(userRoleId);

        if (adminRole is null || userRole is null) return;

        // Uzytkownicy
        var adminId = Guid.NewGuid();
        var userId  = Guid.NewGuid();

        // Liga
        var leagueId = Guid.NewGuid();
        var league = new League
        {
            Id          = leagueId,
            Name        = "Kartingowy Puchar Polski 2026",
            Description = "Ogólnokrajowe amatorskie i profesjonalne rozgrywki kartingowe na najpopularniejszych torach w Polsce.",
            StartDate   = new DateOnly(2026, 4, 1),
            EndDate     = new DateOnly(2026, 10, 31),
            IsActive    = true,
            CreatedAt   = DateTime.UtcNow
        };

        context.Leagues.Add(league);

        // Zawodnicy (10 realnych polskich kierowców wyścigowych / kartingowych)
        var robertKubicaId = Guid.NewGuid();
        var karolBaszId = Guid.NewGuid();
        var kacperSztukaId = Guid.NewGuid();
        var tymoteuszKucharczykId = Guid.NewGuid();
        var maciejGladyszId = Guid.NewGuid();
        var janPrzyrowskiId = Guid.NewGuid();
        var romanBilinskiId = Guid.NewGuid();
        var gustawWisniewskiId = Guid.NewGuid();
        var klaraKowalczykId = Guid.NewGuid();
        var mateuszKaprzykId = Guid.NewGuid();

        var drivers = new List<Driver>
        {
            new() { Id = robertKubicaId, FirstName = "Robert", LastName = "Kubica", RacingNumber = "74", DateOfBirth = new DateOnly(1984, 12, 7), CreatedAt = DateTime.UtcNow },
            new() { Id = karolBaszId, FirstName = "Karol", LastName = "Basz", RacingNumber = "10", DateOfBirth = new DateOnly(1989, 11, 25), CreatedAt = DateTime.UtcNow },
            new() { Id = kacperSztukaId, FirstName = "Kacper", LastName = "Sztuka", RacingNumber = "14", DateOfBirth = new DateOnly(2006, 1, 29), CreatedAt = DateTime.UtcNow },
            new() { Id = tymoteuszKucharczykId, FirstName = "Tymoteusz", LastName = "Kucharczyk", RacingNumber = "18", DateOfBirth = new DateOnly(2006, 2, 26), CreatedAt = DateTime.UtcNow },
            new() { Id = maciejGladyszId, FirstName = "Maciej", LastName = "Gładysz", RacingNumber = "9", DateOfBirth = new DateOnly(2008, 4, 28), CreatedAt = DateTime.UtcNow },
            new() { Id = janPrzyrowskiId, FirstName = "Jan", LastName = "Przyrowski", RacingNumber = "5", DateOfBirth = new DateOnly(2008, 9, 17), CreatedAt = DateTime.UtcNow },
            new() { Id = romanBilinskiId, FirstName = "Roman", LastName = "Biliński", RacingNumber = "17", DateOfBirth = new DateOnly(2004, 3, 4), CreatedAt = DateTime.UtcNow },
            new() { Id = gustawWisniewskiId, FirstName = "Gustaw", LastName = "Wiśniewski", RacingNumber = "13", DateOfBirth = new DateOnly(2006, 9, 8), CreatedAt = DateTime.UtcNow },
            new() { Id = klaraKowalczykId, FirstName = "Klara", LastName = "Kowalczyk", RacingNumber = "27", DateOfBirth = new DateOnly(2011, 9, 15), CreatedAt = DateTime.UtcNow },
            new() { Id = mateuszKaprzykId, FirstName = "Mateusz", LastName = "Kaprzyk", RacingNumber = "11", DateOfBirth = new DateOnly(2001, 8, 30), CreatedAt = DateTime.UtcNow }
        };

        context.Drivers.AddRange(drivers);

        // Uzytkownicy ze stałymi danymi logowania i powiązaniami z kierowcami
        var admin = new User
        {
            Id           = adminId,
            Email        = "admin@karting.pl",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName    = "Admin",
            LastName     = "Karting",
            CreatedAt    = DateTime.UtcNow,
            IsActive     = true,
            UserRoles    = new List<UserRole> { new() { UserId = adminId, RoleId = adminRoleId } }
        };

        var regularUser = new User
        {
            Id           = userId,
            Email        = "user@karting.pl",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User1234!"),
            FirstName = "Robert",
            LastName     = "Kubica",
            CreatedAt    = DateTime.UtcNow,
            IsActive     = true,
            DriverId     = robertKubicaId,
            UserRoles    = new List<UserRole> { new() { UserId = userId, RoleId = userRoleId } }
        };

        context.Users.AddRange(admin, regularUser);

        // Przypisanie administratora do ligi (LeagueAdmin)
        var leagueAdmin = new LeagueAdmin
        {
            LeagueId = leagueId,
            UserId = adminId
        };
        context.LeagueAdmins.Add(leagueAdmin);

        // Gokarty (prawdziwe modele gokartów rentalowych i profesjonalnych)
        var karts = new List<Kart>
        {
            new() { Id = Guid.NewGuid(), Number = "K01", Model = "Sodi RT10", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K02", Model = "Sodi RT10", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K03", Model = "Sodi RX8", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K04", Model = "Sodi RX8", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K05", Model = "Sodi SR5", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K06", Model = "Sodi SR5", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K07", Model = "CRG Centurion", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K08", Model = "CRG Centurion", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K09", Model = "Birel ART N35", Category = "Senior", IsActive = true },
            new() { Id = Guid.NewGuid(), Number = "K10", Model = "Birel ART N35", Category = "Senior", IsActive = true }
        };

        context.Karts.AddRange(karts);

        // Wyścigi na prawdziwych polskich torach
        var race1Id = Guid.NewGuid();
        var race2Id = Guid.NewGuid();
        var race3Id = Guid.NewGuid();
        var race4Id = Guid.NewGuid();
        var race5Id = Guid.NewGuid();

        var races = new List<Race>
        {
            new()
            {
                Id          = race1Id,
                LeagueId    = leagueId,
                Name        = "Runda 1 — Tor Poznań",
                Location    = "Przeźmierowo (Tor Poznań)",
                RaceDate    = new DateTime(2026, 4, 18, 13, 0, 0),
                NumberOfLaps= 20,
                Notes       = "Inauguracyjna runda sezonu na kultowym torze w Poznaniu. Szybka nitka, wymagające zakręty.",
                CreatedAt   = DateTime.UtcNow
            },
            new()
            {
                Id          = race2Id,
                LeagueId    = leagueId,
                Name        = "Runda 2 — Autodrom Słomczyn",
                Location    = "Słomczyn k. Grójca",
                RaceDate    = new DateTime(2026, 5, 16, 14, 0, 0),
                NumberOfLaps= 22,
                Notes       = "Druga runda na nowoczesnym obiekcie rallycrossowym i kartingowym. Bardzo dobra przyczepność.",
                CreatedAt   = DateTime.UtcNow
            },
            new()
            {
                Id          = race3Id,
                LeagueId    = leagueId,
                Name        = "Runda 3 — Kartodrom Bydgoszcz",
                Location    = "Bydgoszcz",
                RaceDate    = new DateTime(2026, 6, 20, 12, 0, 0),
                NumberOfLaps= 25,
                Notes       = "Trzecia runda w samym sercu Bydgoszczy. Techniczny tor z wieloletnią tradycją.",
                CreatedAt   = DateTime.UtcNow
            },
            new()
            {
                Id          = race4Id,
                LeagueId    = leagueId,
                Name        = "Runda 4 — Wallrav Race Center",
                Location    = "Stary Kisielin (Zielona Góra)",
                RaceDate    = new DateTime(2026, 7, 18, 15, 0, 0),
                NumberOfLaps= 20,
                Notes       = "Letnia runda na jednym z najbardziej znanych torów kartingowych w zachodniej Polsce.",
                CreatedAt   = DateTime.UtcNow
            },
            new()
            {
                Id          = race5Id,
                LeagueId    = leagueId,
                Name        = "Runda 5 — Autodrom Pomorze",
                Location    = "Pszczółki k. Gdańska",
                RaceDate    = new DateTime(2026, 8, 22, 14, 0, 0),
                NumberOfLaps= 24,
                Notes       = "Piąta runda na pomorskim obiekcie. Długa prosta startowa i zróżnicowana sekwencja zakrętów.",
                CreatedAt   = DateTime.UtcNow
            }
        };

        context.Races.AddRange(races);

        // Definicje wyników dla każdej rundy (kierowca -> pozycja ukończenia).
        // Używamy z góry zaplanowanych pozycji ukończenia, aby wyniki były spójne i realistyczne.
        // Pozycje startowe są celowo nieco przesunięte, aby zasymulować wyprzedzanie.
        // DNF (Did Not Finish) i DNS (Did Not Start) urozmaicają wyniki.

        // Runda 1: Poznań (laps: 20, baseLapTimeMs: 52000)
        var finishOrderRace1 = new List<(Guid DriverId, ResultStatus Status, int? FinishPos, int StartPos, int LapsCompleted)>
        {
            (karolBaszId, ResultStatus.Finished, 1, 3, 20),
            (robertKubicaId, ResultStatus.Finished, 2, 1, 20),
            (tymoteuszKucharczykId, ResultStatus.Finished, 3, 2, 20),
            (kacperSztukaId, ResultStatus.Finished, 4, 4, 20),
            (maciejGladyszId, ResultStatus.Finished, 5, 6, 20),
            (janPrzyrowskiId, ResultStatus.Finished, 6, 5, 20),
            (romanBilinskiId, ResultStatus.Finished, 7, 8, 20),
            (gustawWisniewskiId, ResultStatus.Finished, 8, 7, 20),
            (mateuszKaprzykId, ResultStatus.Finished, 9, 9, 20),
            (klaraKowalczykId, ResultStatus.DNF, null, 10, 11) // Engine failure on lap 11
        };

        // Runda 2: Słomczyn (laps: 22, baseLapTimeMs: 49500)
        var finishOrderRace2 = new List<(Guid DriverId, ResultStatus Status, int? FinishPos, int StartPos, int LapsCompleted)>
        {
            (robertKubicaId, ResultStatus.Finished, 1, 2, 22),
            (kacperSztukaId, ResultStatus.Finished, 2, 1, 22),
            (karolBaszId, ResultStatus.Finished, 3, 4, 22),
            (tymoteuszKucharczykId, ResultStatus.Finished, 4, 3, 22),
            (romanBilinskiId, ResultStatus.Finished, 5, 7, 22),
            (maciejGladyszId, ResultStatus.Finished, 6, 5, 22),
            (janPrzyrowskiId, ResultStatus.Finished, 7, 6, 22),
            (klaraKowalczykId, ResultStatus.Finished, 8, 10, 22),
            (gustawWisniewskiId, ResultStatus.DNF, null, 8, 15), // Spin on lap 15
            (mateuszKaprzykId, ResultStatus.DNS, null, 9, 0) // Travel issues
        };

        // Runda 3: Bydgoszcz (laps: 25, baseLapTimeMs: 44000)
        var finishOrderRace3 = new List<(Guid DriverId, ResultStatus Status, int? FinishPos, int StartPos, int LapsCompleted)>
        {
            (tymoteuszKucharczykId, ResultStatus.Finished, 1, 2, 25),
            (karolBaszId, ResultStatus.Finished, 2, 1, 25),
            (maciejGladyszId, ResultStatus.Finished, 3, 4, 25),
            (robertKubicaId, ResultStatus.Finished, 4, 3, 25),
            (janPrzyrowskiId, ResultStatus.Finished, 5, 6, 25),
            (kacperSztukaId, ResultStatus.Finished, 6, 5, 25),
            (mateuszKaprzykId, ResultStatus.Finished, 7, 10, 25),
            (romanBilinskiId, ResultStatus.Finished, 8, 7, 25),
            (gustawWisniewskiId, ResultStatus.Finished, 9, 8, 25),
            (klaraKowalczykId, ResultStatus.Finished, 10, 9, 25)
        };

        // Runda 4: Wallrav (laps: 20, baseLapTimeMs: 47000)
        var finishOrderRace4 = new List<(Guid DriverId, ResultStatus Status, int? FinishPos, int StartPos, int LapsCompleted)>
        {
            (kacperSztukaId, ResultStatus.Finished, 1, 2, 20),
            (robertKubicaId, ResultStatus.Finished, 2, 1, 20),
            (karolBaszId, ResultStatus.Finished, 3, 3, 20),
            (tymoteuszKucharczykId, ResultStatus.Finished, 4, 4, 20),
            (maciejGladyszId, ResultStatus.Finished, 5, 5, 20),
            (janPrzyrowskiId, ResultStatus.Finished, 6, 6, 20),
            (klaraKowalczykId, ResultStatus.Finished, 7, 10, 20),
            (gustawWisniewskiId, ResultStatus.Finished, 8, 9, 20),
            (romanBilinskiId, ResultStatus.DNF, null, 7, 4), // Collision on lap 4
            (mateuszKaprzykId, ResultStatus.Finished, 10, 8, 20)
        };

        // Runda 5: Pomorze (laps: 24, baseLapTimeMs: 46000)
        var finishOrderRace5 = new List<(Guid DriverId, ResultStatus Status, int? FinishPos, int StartPos, int LapsCompleted)>
        {
            (karolBaszId, ResultStatus.Finished, 1, 2, 24),
            (tymoteuszKucharczykId, ResultStatus.Finished, 2, 1, 24),
            (robertKubicaId, ResultStatus.Finished, 3, 3, 24),
            (kacperSztukaId, ResultStatus.Finished, 4, 4, 24),
            (maciejGladyszId, ResultStatus.Finished, 5, 5, 24),
            (janPrzyrowskiId, ResultStatus.Finished, 6, 6, 24),
            (romanBilinskiId, ResultStatus.Finished, 7, 7, 24),
            (mateuszKaprzykId, ResultStatus.Finished, 8, 8, 24),
            (gustawWisniewskiId, ResultStatus.Finished, 9, 9, 24),
            (klaraKowalczykId, ResultStatus.Finished, 10, 10, 24)
        };

        var raceResults = new List<RaceResult>();
        var random = new Random(42); // Seedowany generator, aby wyniki były w 100% deterministyczne przy każdym seedowaniu

        void AddResultsForRace(Guid raceId, int totalLaps, long baseLapTimeMs, List<(Guid DriverId, ResultStatus Status, int? FinishPos, int StartPos, int LapsCompleted)> order)
        {
            // Przypisanie gokartów do kierowców w tej rundzie (pomieszane)
            var shuffledKarts = new List<Kart>(karts);
            // Proste tasowanie deterministyczne
            for (int i = shuffledKarts.Count - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                var temp = shuffledKarts[i];
                shuffledKarts[i] = shuffledKarts[k];
                shuffledKarts[k] = temp;
            }

            for (int i = 0; i < order.Count; i++)
            {
                var entry = order[i];
                var kart = shuffledKarts[i % shuffledKarts.Count];

                long lapTimeMs = 0;
                long totalTimeMs = 0;
                string? notes = null;

                if (entry.Status == ResultStatus.DNS)
                {
                    notes = "Nie wystartował — problemy logistyczne.";
                }
                else
                {
                    // Ustalenie najszybszego okrążenia na podstawie pozycji (najlepsi kierowcy jadą nieco szybciej)
                    int performanceBonus = entry.FinishPos.HasValue ? (10 - entry.FinishPos.Value) * 120 : 200;
                    lapTimeMs = baseLapTimeMs - performanceBonus + random.Next(-300, 300);

                    // Średni czas okrążenia jest o ok. 0.8-1.5 sekundy gorszy od rekordu
                    long avgLapTimeMs = lapTimeMs + random.Next(800, 1500);
                    totalTimeMs = entry.LapsCompleted * avgLapTimeMs;

                    if (entry.Status == ResultStatus.DNF)
                    {
                        notes = $"Nie ukończył — wycofanie na okrążeniu {entry.LapsCompleted}.";
                    }
                }

                raceResults.Add(new RaceResult
                {
                    Id                = Guid.NewGuid(),
                    RaceId            = raceId,
                    DriverId          = entry.DriverId,
                    KartId            = kart.Id,
                    LapTimeMs         = lapTimeMs,
                    TotalTimeMs       = totalTimeMs,
                    StartingPosition  = entry.StartPos,
                    FinishingPosition = entry.FinishPos,
                    LapsCompleted     = entry.LapsCompleted,
                    Status            = entry.Status,
                    Notes             = notes,
                    CreatedAt         = DateTime.UtcNow
                });
            }
        }

        AddResultsForRace(race1Id, 20, 52000, finishOrderRace1);
        AddResultsForRace(race2Id, 22, 49500, finishOrderRace2);
        AddResultsForRace(race3Id, 25, 44000, finishOrderRace3);
        AddResultsForRace(race4Id, 20, 47000, finishOrderRace4);
        AddResultsForRace(race5Id, 24, 46000, finishOrderRace5);

        context.RaceResults.AddRange(raceResults);

        await context.SaveChangesAsync();
    }
}

