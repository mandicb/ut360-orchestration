using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.Exceptions;

namespace Orchestration.Commands.CommandHandlers.DeviceGateway;

public class DeleteDeviceGatewayCommandHandler : CommandHandlerBase, IRequestHandler<DeleteDeviceGatewayCommand, int>
{
    public DeleteDeviceGatewayCommandHandler(OrchestrationDataContext context) : base(context)
    {
    
    }

    public async Task<int> Handle(DeleteDeviceGatewayCommand request, CancellationToken cancellationToken)
    {
        if (context.DeviceGateways == null)
            throw new CommandException("Table 'DeviceGateway' does not exists.");
        if (request == null)
            throw new CommandException("Request cannot be null.");

        var deviceGateway = await context.DeviceGateways
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (deviceGateway == null)
            throw new CommandException($"DeviceGateway with id = {request.Id} does not exists.");

        var numberOfDeletedDgs = await context.DeviceGateways.Where(x => x.Id == deviceGateway.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return numberOfDeletedDgs;
    }
}
