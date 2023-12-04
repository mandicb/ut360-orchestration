using Orchestration.Exceptions;
using Orchestration.Models.Device;

namespace Orchestration.DataContext.Models;

public class Device : EntityBase
{
    public Device()
    {
    }
    private Device(CreateDeviceCommandModel cmd) : base(cmd.Id)
    {
        Name = cmd.Name;
        HostName = cmd.HostName;
        Port = cmd.Port;
    }

    public string Name { get; private set; } = null!;
    public string HostName { get; private set; } = null!;
    public int Port { get; set; }
    public Credential Credential { get;  set; } = null!;

    public DeviceGateway DeviceGateway { get;  set; } = null!;
    
    public static Device Create(CreateDeviceCommandModel cmd)
    {
        return new Device(cmd);
    }
}