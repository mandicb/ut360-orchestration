using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestration.Commands.CommandHandlers.DeviceGateway;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.Exceptions;

namespace Orchestration.Commands.CommandHandlers.Devices;

public class DeleteDeviceCommandHandler : CommandHandlerBase, IRequestHandler<DeleteDeviceCommand, int>
{
    public DeleteDeviceCommandHandler(OrchestrationDataContext context) : base(context)
    {
    }

    public async Task<int> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var credentials = await context.Credentials.FirstOrDefaultAsync(cancellationToken);

        if (context.Devices == null)
            throw new CommandException("Table 'Device' does not exists.");
        if (request == null)
            throw new CommandException("Request cannot be null.");

        var deviceGateway = await context.Devices
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (deviceGateway == null)
            throw new CommandException($"DeviceGateway with id = {request.Id} does not exists.");

        var numberOfDeletedDgs = await context.Devices.Where(x => x.Id == deviceGateway.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        credentials =  await context.Credentials.FirstOrDefaultAsync(cancellationToken);
        return numberOfDeletedDgs;
    }
}