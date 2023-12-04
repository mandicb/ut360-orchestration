using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.Credential;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Models.DeviceGateway;
using Orchestration.Queries.Infrastructure;
using Utrust360.Crypto.Model;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.Queries.QueryHandlers.Devices;

public class GetDeviceQueryHandler :  QueryHandlerBase, IRequestHandler<GetDeviceRequest, DeviceViewModel>
{
    private ICryptoService _cryptoService;
    
    public GetDeviceQueryHandler(OrchestrationDataContext context, ICryptoService cryptoService) : base(context)
    {
        _cryptoService = cryptoService;
    }

    public async Task<DeviceViewModel> Handle(GetDeviceRequest request, CancellationToken cancellationToken)
    {
        var deviceDg = await context.Devices.AsNoTracking()
            .Include(x => x.DeviceGateway)
            .Include(x => x.Credential)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (deviceDg == null)
        {
            throw new QueryException($"Device {request.Id} does not exists ");
        }
        
        var device = new DeviceViewModel()
        {
            Id = deviceDg.Id,
            Name = deviceDg.Name,
            HostName = deviceDg.HostName,
            Port = deviceDg.Port,
            DeviceGateway = new DeviceGatewayViewModel()
            {
                Id = deviceDg.DeviceGateway.Id,
                Name = deviceDg.DeviceGateway.Name,
                HostName = deviceDg.DeviceGateway.HostName,
                Port = deviceDg.DeviceGateway.Port,
                Certificate = deviceDg.DeviceGateway.Certificate,
                Password = request.Decrypt ? _cryptoService.Decrypt(deviceDg.DeviceGateway.Password, ProtectionKeys.KeyType.DeviceGateway.ToString()) : deviceDg.DeviceGateway.Password
            },
            
            Credential = new CreateCredentialRequestModel()
            {
                Id = deviceDg.Credential.Id,
                Name = deviceDg.Credential.Name,
                KeyData = deviceDg.Credential.KeyData,
                KeyPassword = deviceDg.Credential.KeyPassword,
                UserName = deviceDg.Credential.UserName
            }
            
        };

        return device;
    }
}