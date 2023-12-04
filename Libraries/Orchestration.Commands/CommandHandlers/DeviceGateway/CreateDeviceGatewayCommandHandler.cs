using MediatR;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.DeviceGateway;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.Commands.CommandHandlers.DeviceGateway;

public class CreateDeviceGatewayCommandHandler  : CommandHandlerBase, IRequestHandler<CreateDeviceGatewayCommandModel, int>
{
    private ICryptoService _cryptoService;
    
    public CreateDeviceGatewayCommandHandler(OrchestrationDataContext context, ICryptoService cryptoService) : base(context)
    {
        _cryptoService = cryptoService;
    }

    public async Task<int> Handle(CreateDeviceGatewayCommandModel request, CancellationToken cancellationToken)
    {
        if(context.DeviceGateways == null)
            throw new CommandException("Table 'DeviceGateway' does not exists.");
        if (request == null)
            throw new CommandException("Request cannot be null.");

        var newDeviceGateway  = DataContext.Models.DeviceGateway.Create(request, _cryptoService);
        
         await context.DeviceGateways.AddAsync(newDeviceGateway, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return newDeviceGateway.Id;
    }
}