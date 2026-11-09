using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoMoreTears.Shared.Postgres.Messaging;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Registration.Configuration;

namespace NoMoreTears.Shared.Postgres.Extensions;

/// <summary>
///     Wires the MassTransit transactional outbox and inbox to the Postgres <see cref="DbContext"/> of a module.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             Bus outbox: events published by the module outside a consumer (for example from command handlers)
///             are written to the <c>OutboxMessage</c> table in the same transaction as the business data
///             and delivered to the broker by a background service of the context.
///         </item>
///         <item>
///             Consumer outbox and inbox: receive endpoints of the module record consumed messages in <c>InboxState</c>
///             (duplicates are ignored) and keep messages published by the consumer until its transaction commits.
///         </item>
///     </list>
///     All modules of a host share one bus, and MassTransit binds its scoped outbox provider to one context only;
///     therefore every module publishes through its own <see cref="OutboxEventPublisher{TModule, TDbContext}"/>.
/// </remarks>
public static class OutboxExtensions
{
    /// <summary>
    ///     Delay between polls of the outbox delivery service.
    /// </summary>
    public static readonly TimeSpan DefaultQueryDelay = TimeSpan.FromSeconds(1);

    /// <summary>
    ///     How long consumed message ids are kept to detect duplicates.
    /// </summary>
    public static readonly TimeSpan DefaultDuplicateDetectionWindow = TimeSpan.FromMinutes(30);

    /// <summary>
    ///     Adds the <c>InboxState</c>, <c>OutboxMessage</c> and <c>OutboxState</c> tables to the model.
    ///     Call it in <c>OnModelCreating</c> of every context used with <c>UsePostgresOutbox</c>.
    /// </summary>
    public static ModelBuilder AddTransactionalOutbox(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        return modelBuilder;
    }

    /// <summary>
    ///     Uses <typeparamref name="TDbContext"/> as the transactional outbox of the module <typeparamref name="TModule"/>
    ///     and as the inbox/outbox of the receive endpoints of its consumers.
    ///     <see cref="IEventPublisher{TModule}"/> then publishes through this outbox.
    /// </summary>
    public static RabbitMqModuleBuilder<TModule> UsePostgresOutbox<TModule, TDbContext>(
        this RabbitMqModuleBuilder<TModule> module,
        Action<IEntityFrameworkOutboxConfigurator>? configure = null)
        where TModule : class
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(module);

        module
            .ConfigureBus(registration => registration.AddEntityFrameworkOutbox<TDbContext>(outbox =>
            {
                outbox.UsePostgres();
                outbox.UseBusOutbox();
                outbox.QueryDelay = DefaultQueryDelay;
                outbox.DuplicateDetectionWindow = DefaultDuplicateDetectionWindow;

                configure?.Invoke(outbox);
            }))
            .ConfigureEndpoints((context, endpoint) => endpoint.UseEntityFrameworkOutbox<TDbContext>(context));

        module.Services.TryAddScoped<EntityFrameworkScopedBusContextProvider<IBus, TDbContext>>();
        module.Services.Replace(ServiceDescriptor.Scoped<IEventPublisher<TModule>, OutboxEventPublisher<TModule, TDbContext>>());

        return module;
    }
}
