using MediatR;

namespace Orchestration.Commands.CommandHandlers.DeviceGateway;

public class DeleteDeviceGatewayCommand : IRequest<int>
{
    public int Id { get; set; }

    public DeleteDeviceGatewayCommand(int id)
    {
        Id = id;
    }
}