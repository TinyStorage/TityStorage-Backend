using Itmo.TinyStorage.Application.Shared.Common.Behaviors;
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;

namespace Itmo.TinyStorage.Application.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddTinyStorageApplicationSharedLayer(this IServiceCollection services)
    {
        services
            .AddAuthorizersFromAssemblyContaining(typeof(DependencyInjection))
            .AddValidatorsFromAssemblyContaining(typeof(DependencyInjection))
            .AddMediatR();

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var assemblies = Assembly
            .GetEntryAssembly()!
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .ToArray();

        services.AddMediatR(configuration => configuration
            .RegisterServicesFromAssemblies(assemblies)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(ValidatorBehavior<,>))
            .AddOpenBehavior(typeof(AuthorizationBehavior<,>))
            .AddOpenBehavior(typeof(TransactionBehavior<,>)));

        return services;
    }

    private static IServiceCollection AddAuthorizersFromAssemblyContaining(
        this IServiceCollection services,
        Type type,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var assignableTypes = type.Assembly.GetTypesAssignableTo(typeof(IAuthorizer<>));
        foreach (var assignableType in assignableTypes)
        {
            foreach (var implementedInterface in assignableType.ImplementedInterfaces)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Scoped:
                        services.AddScoped(implementedInterface, assignableType);
                        break;
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(implementedInterface, assignableType);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(implementedInterface, assignableType);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                }
            }
        }

        return services;
    }

    private static IReadOnlyCollection<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
    {
        var typeInfoList = assembly.DefinedTypes
            .Where(typeInfo => typeInfo is { IsClass: true, IsAbstract: false }
                               && typeInfo != compareType
                               && typeInfo.GetInterfaces()
                                   .Any(type => type.IsGenericType
                                                && type.GetGenericTypeDefinition() == compareType))
            .ToArray();

        return typeInfoList;
    }
}