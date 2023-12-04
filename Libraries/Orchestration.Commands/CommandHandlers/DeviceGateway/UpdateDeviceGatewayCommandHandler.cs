using MediatR;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.DeviceGateway;

namespace Orchestration.Commands.CommandHandlers.DeviceGateway;

public class UpdateDeviceGatewayCommandHandler : CommandHandlerBase, IRequestHandler<UpdateDeviceGatewayCommandModel, Unit>
{
    public UpdateDeviceGatewayCommandHandler(OrchestrationDataContext context) : base(context)
    {
    }

    public async Task<Unit> Handle(UpdateDeviceGatewayCommandModel request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new CommandException("Request cannot be null.");
        if (context.DeviceGateways == null)
            throw new CommandException("Table 'DeviceGateway' does not exists.");

        var deviceGateway = await context.DeviceGateways
            .FindAsync(new object?[] { request.Id, cancellationToken }, cancellationToken: cancellationToken);
        if (deviceGateway == null)
            throw new CommandException($"DeviceGateway with id = {request.Id} does not exists.");

        deviceGateway.Update(request);
        
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}