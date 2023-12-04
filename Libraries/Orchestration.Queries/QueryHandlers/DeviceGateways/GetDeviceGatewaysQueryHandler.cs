using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Models.DeviceGateway;
using Orchestration.Queries.Infrastructure;

namespace Orchestration.Queries.QueryHandlers.DeviceGateways;

public class GetDeviceGatewaysQueryHandler : QueryHandlerBase, IRequestHandler<GetDeviceGatewaysRequest, 
    List<DeviceGatewayViewModel>>
{
    public GetDeviceGatewaysQueryHandler(OrchestrationDataContext context)
        : base(context) { }

    public async Task<List<DeviceGatewayViewModel>> Handle(GetDeviceGatewaysRequest request, CancellationToken cancellationToken)
    {
        if (context.DeviceGateways == null)
            throw new QueryException("Table 'DeviceGateway' does not exists.");

        var deviceGateways = await context.DeviceGateways
            .AsNoTracking()
            .Include(x => x.Devices)
            .Select(x => new DeviceGatewayViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Port = x.Port,
                HostName = x.HostName,
               
            }).ToListAsync(cancellationToken);
        return deviceGateways;

    }
}