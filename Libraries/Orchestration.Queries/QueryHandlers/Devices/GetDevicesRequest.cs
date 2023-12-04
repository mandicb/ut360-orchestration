using MediatR;
using Orchestration.Models;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;

namespace Orchestration.Queries.QueryHandlers.Devices;

public class GetDevicesRequest : PaginationRequestModel, IRequest<List<DeviceListModel>>
{
}
