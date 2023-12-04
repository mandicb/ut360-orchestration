using MediatR;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.Device;
using Orchestration.Models.DeviceGateway;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.Commands.CommandHandlers.Devices;

public class CreateDeviceCommandHandler : CommandHandlerBase, IRequestHandler<CreateDeviceCommandModel, int>
{
    private ICryptoService _cryptoService;
    public CreateDeviceCommandHandler(OrchestrationDataContext context, ICryptoService cryptoService) : base(context)
    {
        _cryptoService = cryptoService;
    }

    public async Task<int> Handle(CreateDeviceCommandModel request, CancellationToken cancellationToken)
    {
        if(context.Devices == null)
            throw new CommandException("Table 'Device' does not exists.");
        if (request == null)
            throw new CommandException("Request cannot be null.");

        var newDevice  = DataContext.Models.Device.Create(request);
        
        //check dg
        var deg = request.DeviceGatewayId  == null ? context.DeviceGateways.FirstOrDefault() : context.DeviceGateways.FirstOrDefault(x => x.Id == request.DeviceGatewayId);
        if (deg == null)
        {
            throw new CommandException("Add device gateway first");
        }
        newDevice.DeviceGateway = deg;
        
        //check credentials
        var credentials = context.Credentials.FirstOrDefault(x => request.Credential != null && x.Id == request.Credential.Id);
        if (credentials != null)
        {
            newDevice.Credential = credentials;
        }
        else
        {
            if (request.Credential != null)
            {
                var d =  DataContext.Models.Credential.Create(request.Credential, _cryptoService);
                newDevice.Credential = d;
            }
        }

        await context.Devices.AddAsync(newDevice, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return newDevice.Id;    
    }
}