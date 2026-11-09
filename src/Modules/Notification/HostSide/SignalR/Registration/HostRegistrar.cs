using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Applications.Services.Messaging;
using NoMoreTears.Modules.Notification.Contracts.Events;
using NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Identity;
using NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;
using NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Notifications;
using NoMoreTears.Modules.Notification.HostSide.SignalR.Realtime;
using NoMoreTears.Modules.Notification.Providers;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Authorization.Registration;
using NoMoreTears.Shared.HostSide.SignalR.Registration;
using NoMoreTears.Shared.Providers.Identity.Registration;
using NoMoreTears.Shared.RabbitMq.Registration;
using NoMoreTears.Shared.RabbitMq.Registration.Configuration;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Registration;

/// <summary>
///     Registers the SignalR host.
/// </summary>
public static class HostRegistrar
{
    private const string ConnectionStringName = "Postgres";
    internal const string CorsPolicyName = "Clients";
    private const string CorsSectionName = "Cors:AllowedOrigins";

    extension(IServiceCollection services)
    {
        public IServiceCollection AddSignalRHost(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            // Tokens are validated exactly as in WebApi, including revoked sessions.
            services
                .AddIdentityProvider(configuration, identity => identity
                    .UsePostgres(blueprint => blueprint.UseConnectionString(ConnectionStringName))
                    .UseJwtBearer())
                .AddAuthorization();

            services
                .AddRealtime(configuration)
                .AddSingleton<IRealtimeNotifier, HubRealtimeNotifier>();

            services
                // Hub calls and consumers pass the recipient explicitly; handlers run on behalf of the system.
                .AddUserSessions(_ => new CurrentUserSession())
                .AddNotificationDataAccess(blueprint => blueprint.UseConnectionString(ConnectionStringName))
                .AddNotificationApplications(configuration)
                .AddNotificationEmailDelivery()
                .AddNotificationMessaging(AddConsumers)
                .AddRabbitMq(configuration);

            var allowedOrigins = configuration.GetSection(CorsSectionName).Get<string[]>() ?? [];

            services.AddCors(cors => cors.AddPolicy(CorsPolicyName, policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));

            services.AddHealthChecks();
            return services;
        }
    }

    private static void AddConsumers(RabbitMqModuleBuilder<INotificationOutbox> consumers)
    {
        consumers
            // Internal events: real-time delivery after the commit.
            .AddConsumer<NotificationCreatedEvent, NotificationCreatedEventConsumer>()
            .AddConsumer<NotificationsChangedEvent, NotificationsChangedEventConsumer>()
            // Identity: emails.
            .AddConsumer<EmailConfirmationRequestedEvent, EmailConfirmationRequestedEventConsumer>()
            .AddConsumer<PasswordResetRequestedEvent, PasswordResetRequestedEventConsumer>()
            .AddConsumer<MfaCodeIssuedEvent, MfaCodeIssuedEventConsumer>()
            // Masters: in-app notifications of the master.
            .AddConsumer<MasterSubmittedForVerificationEvent, MasterSubmittedForVerificationEventConsumer>()
            .AddConsumer<MasterVerifiedEvent, MasterVerifiedEventConsumer>()
            .AddConsumer<MasterVerificationRejectedEvent, MasterVerificationRejectedEventConsumer>()
            .AddConsumer<MasterVerificationRevokedEvent, MasterVerificationRevokedEventConsumer>()
            .AddConsumer<MasterVerificationRestoredEvent, MasterVerificationRestoredEventConsumer>()
            .AddConsumer<MasterCategoryVerifiedEvent, MasterCategoryVerifiedEventConsumer>()
            .AddConsumer<MasterCategoryRejectedEvent, MasterCategoryRejectedEventConsumer>()
            .AddConsumer<MasterServiceAreaVerifiedEvent, MasterServiceAreaVerifiedEventConsumer>()
            .AddConsumer<MasterServiceAreaRejectedEvent, MasterServiceAreaRejectedEventConsumer>()
            .AddConsumer<MasterVerificationHoldPlacedEvent, MasterVerificationHoldPlacedEventConsumer>()
            .AddConsumer<MasterVerificationHoldReleasedEvent, MasterVerificationHoldReleasedEventConsumer>()
            .AddConsumer<MasterSanctionAppliedEvent, MasterSanctionAppliedEventConsumer>()
            .AddConsumer<MasterSanctionLiftedEvent, MasterSanctionLiftedEventConsumer>()
            .AddConsumer<MasterFileUploadCompletedEvent, MasterFileUploadCompletedEventConsumer>()
            .AddConsumer<MasterFileUploadFailedEvent, MasterFileUploadFailedEventConsumer>();
    }
}