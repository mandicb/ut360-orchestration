
using System.Runtime.CompilerServices;
using Orchestration.Exceptions;
using Orchestration.Models.DeviceGateway;
using Utrust360.Crypto.Model;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.DataContext.Models;

public  class DeviceGateway : EntityBase
{
    public DeviceGateway(){}

    private DeviceGateway(CreateDeviceGatewayCommandModel cmd) : base(cmd.Id)
    {
        Name = cmd.Name;
        HostName = cmd.HostName;
        Port = cmd.Port;
        Password = cmd.Password;
        Certificate = cmd.Certificate;
    }

    public byte[] Certificate { get; set; } = null!;

    public string Name { get; private set; } = null!;
    public string HostName { get; private set; }  = null!;
    public int Port { get; set; }
    public string Password { get; set; } = null!;
    private readonly List<Device> _devices = new();
    public IReadOnlyCollection<Device> Devices => _devices;
    
    

    public static DeviceGateway Create(CreateDeviceGatewayCommandModel cmd, ICryptoService cryptoService)
    {
        // if (cmd.Id == Guid.Empty)
        //     throw new ModelException("Id cannot be empty.");
        cmd.Password = cryptoService.Encrypt(cmd.Password, ProtectionKeys.KeyType.DeviceGateway.ToString());
        return new DeviceGateway(cmd);
    }
    
    public void Update(UpdateDeviceGatewayCommandModel cmd)
    {
        Name = cmd.Name!;
        Port = cmd.Port;
        HostName = cmd.HostName;
    }

}