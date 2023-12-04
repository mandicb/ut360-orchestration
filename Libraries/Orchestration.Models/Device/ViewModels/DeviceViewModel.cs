using Orchestration.Models.Credential;
using Orchestration.Models.DeviceGateway;

namespace Orchestration.Models.Device.ViewModels;

/// <summary>
/// Device view
/// </summary>
public class DeviceViewModel
{
    public int Id { get; set; } 
  
    public string Name { get; set; } = null!;
    public string HostName { get; set; } = null!; 
    public int Port { get; set; }   

    public DeviceGatewayViewModel DeviceGateway { get; set; }
    
    public CreateCredentialRequestModel Credential { get; set; }
}