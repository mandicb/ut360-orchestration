using MediatR;
using Orchestration.Models;
using Orchestration.Models.DeviceGateway;

namespace Orchestration.Queries.QueryHandlers.DeviceGateways;

public class GetDeviceGatewaysRequest :  PaginationRequestModel, IRequest<List<DeviceGatewayViewModel>>
{
    
}