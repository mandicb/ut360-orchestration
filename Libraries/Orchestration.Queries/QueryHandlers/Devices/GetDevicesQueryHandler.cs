using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Queries.Infrastructure;

namespace Orchestration.Queries.QueryHandlers.Devices;

public class GetDevicesQueryHandler : QueryHandlerBase, IRequestHandler<GetDevicesRequest, List<DeviceListModel>>
{
    public GetDevicesQueryHandler(OrchestrationDataContext context) : base(context)
    {
    }

    public async Task<List<DeviceListModel>> Handle(GetDevicesRequest request, CancellationToken cancellationToken)
    {
        if(context.Devices == null)
            throw new QueryException("Table 'Devices' does not exists.");

        var devices = await context.Devices.AsNoTracking()
            .Include(x => x.DeviceGateway)
            .Include(x => x.Credential)
            .Select(x => new DeviceListModel()
        {
            Id = x.Id,
            Name = x.Name,
            HostName = x.HostName,
            Port = x.Port,
            CredentialName = x.Credential.Name,
            DeviceGatewayId = x.DeviceGateway.Id
            
        }).ToListAsync(cancellationToken);

        return devices;
    }
}