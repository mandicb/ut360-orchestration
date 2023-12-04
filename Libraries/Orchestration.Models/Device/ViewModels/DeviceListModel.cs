namespace Orchestration.Models.Device.ViewModels;

/// <summary>
///  Device list vciew
/// </summary>
public class DeviceListModel
{
    /// <example>1</example>>
    public int Id { get; set; }
    
    /// <example>Device1</example>>
    public string Name { get; set; } = null!;

    /// <example>172.1.1.0</example>>
    public string HostName { get; set; } = null!;
    
    /// <example>4000</example>>
    public int Port { get; set; }
    /// <example>Credential1</example> 
    public string CredentialName { get; set; } = null!;
    
    /// <example>1</example> 
    public int DeviceGatewayId { get; set; }
}