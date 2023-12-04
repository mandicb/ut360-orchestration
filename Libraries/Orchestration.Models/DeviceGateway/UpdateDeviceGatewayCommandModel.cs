using MediatR;

namespace Orchestration.Models.DeviceGateway;

public class UpdateDeviceGatewayCommandModel : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string HostName { get; set; } = null!;
    public int Port { get; set; } 
}
