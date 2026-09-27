using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;
using System.Reflection;

namespace NoMoreTears.Shared.CQS.Extensions;

/// <summary>
///     Provides extension methods for registering CQS handlers and related infrastructure.
/// </summary>
public static class HandlerExtensions
{
    /// <summary>
    ///     Supported CQS handler interface types.
    /// </summary>
    public static readonly Type[] CqsHandlerInterfaces =
    [
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>)
    ];

    /// <param name="services">Service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers CQS infrastructure components from the specified assembly,
        ///     including handlers and validators.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCqsHandlersFromAssembly(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            services
                .AddHandlersFromAssembly(assembly)
                .AddAuthorizationHandlersFromAssembly(assembly)
                .AddValidatorsFromAssembly(assembly);

            return services;
        }

        /// <summary>
        ///     Scans the specified assembly and registers all CQS and MediatR handlers.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddHandlersFromAssembly(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var handlerTypes = assembly
                .GetTypes()
                .Where(t => t.IsConcreteType())
                .Select(t => new
                {
                    Implementation = t,
                    Interfaces = t.GetInterfaces()
                        .Where(i =>
                            i.IsGenericType &&
                            !i.IsOpenGenericType() &&
                            (
                                CqsHandlerInterfaces.Contains(i.GetGenericTypeDefinition()) ||
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)
                            ))
                })
                .Where(x => x.Interfaces.Any());

            foreach (var handler in handlerTypes)
            {
                foreach (var @interface in handler.Interfaces)
                {
                    services.AddScoped(@interface, handler.Implementation);
                }
            }

            return services;
        }

        /// <summary>
        ///     Scans the specified assembly and registers all <see cref="IAuthorizationHandler{TRequest}"/>.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddAuthorizationHandlersFromAssembly(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var authorizationHandlerType = typeof(IAuthorizationHandler<>);

            var handlerTypes = assembly
                .GetTypes()
                .Where(t => t.IsConcreteType())
                .Select(t => new
                {
                    Implementation = t,
                    Interfaces = t.GetInterfaces()
                        .Where(i =>
                            i.IsGenericType &&
                            !i.IsOpenGenericType() &&
                            i.GetGenericTypeDefinition() == authorizationHandlerType)
                })
                .Where(x => x.Interfaces.Any());

            foreach (var handler in handlerTypes)
            {
                foreach (var @interface in handler.Interfaces)
                {
                    services.AddScoped(@interface, handler.Implementation);
                }
            }

            return services;
        }
    }
}