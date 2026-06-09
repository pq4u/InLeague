global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Linq;

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
global using InLeague.Application.Features.Auth.DTOs;
global using InLeague.Application.Features.Auth.Services;
global using InLeague.Application.Features.Auth.Validators;
global using InLeague.Application.Features.Drivers.DTOs;
global using InLeague.Application.Features.Drivers.Services;
global using InLeague.Application.Features.Karts.DTOs;
global using InLeague.Application.Features.Karts.Services;
global using InLeague.Application.Features.Leagues.DTOs;
global using InLeague.Application.Features.Leagues.Services;
global using InLeague.Application.Features.Races.DTOs;
global using InLeague.Application.Features.Races.Services;
global using InLeague.Application.Features.RaceResults.DTOs;
global using InLeague.Application.Features.RaceResults.Services;

// API Feature namespaces
global using InLeague.Features.Auth;
global using InLeague.Features.Drivers;
global using InLeague.Features.Karts;
global using InLeague.Features.Leagues;
global using InLeague.Features.Races;
global using InLeague.Features.RaceResults;
