using MediatR;

namespace Orchestration.Commands.CommandHandlers.Devices;

public class DeleteDeviceCommand : IRequest<int>
{
    public int Id { get; set; }

    public DeleteDeviceCommand(int id)
    {
        Id = id;
    }
}
