global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Linq;
global using Microsoft.EntityFrameworkCore;

// Domain namespaces
global using InLeague.Domain.Features.Leagues;
global using InLeague.Domain.Features.Drivers;
global using InLeague.Domain.Features.Karts;
global using InLeague.Domain.Features.Races;
global using InLeague.Domain.Features.Races.Enums;
global using InLeague.Domain.Features.Users;
global using InLeague.Domain.Features.Users.Exceptions;

// Application namespaces
global using InLeague.Application.Common.Interfaces;
global using InLeague.Application.Features.Auth.Interfaces;
global using InLeague.Application.Features.Drivers.Interfaces;
global using InLeague.Application.Features.Karts.Interfaces;
global using InLeague.Application.Features.Leagues.Interfaces;
global using InLeague.Application.Features.Races.Interfaces;
global using InLeague.Application.Features.RaceResults.Interfaces;

// Infrastructure namespaces
global using InLeague.Infrastructure.Common;
global using InLeague.Infrastructure.Features.Auth.Repositories;
global using InLeague.Infrastructure.Features.Drivers.Repositories;
global using InLeague.Infrastructure.Features.Karts.Repositories;
global using InLeague.Infrastructure.Features.Leagues.Repositories;
global using InLeague.Infrastructure.Features.Races.Repositories;
global using InLeague.Infrastructure.Features.RaceResults.Repositories;
