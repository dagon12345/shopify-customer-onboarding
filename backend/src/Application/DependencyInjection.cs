using System.Reflection;
using Application.Common.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<ISender, Sender>();
        services.AddCommandHandlersFromAssembly(assembly);

        return services;
    }

    private static void AddCommandHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(ICommandHandler<,>);

        var handlerImplementations =
            from type in assembly.GetTypes()
            where !type.IsAbstract && !type.IsInterface
            from @interface in type.GetInterfaces()
            where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == handlerInterfaceType
            select (Service: @interface, Implementation: type);

        foreach (var (service, implementation) in handlerImplementations)
        {
            services.AddScoped(service, implementation);
        }
    }
}
