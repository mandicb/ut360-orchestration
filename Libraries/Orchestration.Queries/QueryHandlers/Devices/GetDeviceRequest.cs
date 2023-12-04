using MediatR;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;

namespace Orchestration.Queries.QueryHandlers.Devices;

public class GetDeviceRequest : IRequest<DeviceViewModel>
{
    public int Id { get; set; }
    public bool Decrypt { get; set; } 

    public GetDeviceRequest(int id, bool  decrypt)
    {
        Id = id;
        Decrypt = decrypt;
    } 
}