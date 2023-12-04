using MediatR;
using Orchestration.Models;
using Orchestration.Models.DeviceGateway;

namespace Orchestration.Queries.QueryHandlers.DeviceGateways;

public class GetDeviceGatewayRequest  : IRequest<DeviceGatewayViewModel>
{
    public int? Id { get; }

    public GetDeviceGatewayRequest(int? id)
    {
        Id = id;
    }
}