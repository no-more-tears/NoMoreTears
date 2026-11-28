using MassTransit;
using MediatR;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.GrantMasterRole;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Identity.HostSide.Consumers.Masters;

/// <summary>
///     Grants the Master role once the master profile of the user is verified (<see cref="MasterVerifiedEvent"/>).
///     The master profile id equals the user id. The role is kept after a revocation: Masters, Dispatch and
///     Requests check the verification status of the profile themselves.
/// </summary>
public sealed class MasterVerifiedEventConsumer(
    ISender sender) : IEventConsumer<MasterVerifiedEvent>
{
    public async Task Consume(ConsumeContext<MasterVerifiedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await sender.Send(
            new GrantMasterRoleCommand(context.Message.MasterId),
            context.CancellationToken);
    }
}
