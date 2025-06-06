global using System.Reflection;
global using FluentValidation;
global using Itmo.TinyStorage.Application.Shared.Common.Abstractions;
global using Itmo.TinyStorage.Application.Shared.Common.Abstractions.Cqrs.Command;
global using Itmo.TinyStorage.Application.Shared.Common.Abstractions.Cqrs.Query;
global using Itmo.TinyStorage.Application.Shared.Common.Behaviors;
global using Itmo.TinyStorage.Application.Shared.Common.Exceptions;
global using Itmo.TinyStorage.Application.Shared.Common.Extensions;
global using Itmo.TinyStorage.Auth;
global using Itmo.TinyStorage.Domain.Aggregates.Items;
global using Itmo.TinyStorage.Domain.Core.Exceptions;
global using Itmo.TinyStorage.Infrastructure;
global using Itmo.TinyStorage.Infrastructure.Items;
global using JetBrains.Annotations;
global using MediatR;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
