using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.DeviceGateway;
using Orchestration.Queries.Infrastructure;
using Utrust360.Crypto.Model;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.Queries.QueryHandlers.DeviceGateways;

public class GetDeviceGatewayQueryHandler :  QueryHandlerBase, IRequestHandler<GetDeviceGatewayRequest, DeviceGatewayViewModel>
{
    private ICryptoService _cryptoService;
    public GetDeviceGatewayQueryHandler(OrchestrationDataContext context, ICryptoService cryptoService) : base(context)
    {
        _cryptoService = cryptoService;
    }

    public async Task<DeviceGatewayViewModel> Handle(GetDeviceGatewayRequest request, CancellationToken cancellationToken)
    {
        if (context.DeviceGateways == null)
            throw new QueryException("Table 'DeviceGateway' does not exists.");

        var deviceGateway = request.Id == null  ? await context.DeviceGateways.FirstOrDefaultAsync(cancellationToken: cancellationToken) : await context.DeviceGateways.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);


        if (deviceGateway == null)
        {
            throw new QueryException("DeviceGateway not exists");
        }
        
        var response = new DeviceGatewayViewModel()
        {
            Id = deviceGateway.Id,
            Name = deviceGateway.Name,
            HostName = deviceGateway.HostName,
            Port = deviceGateway.Port,
            Password = _cryptoService.Decrypt(deviceGateway.Password, ProtectionKeys.KeyType.DeviceGateway.ToString()),
            Certificate = deviceGateway.Certificate
        };
        
        return response;
    }
}