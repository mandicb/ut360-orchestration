using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Orchestration.Models.Credential;

namespace Orchestration.Models.Device.ViewModels;

/// <summary>
/// create device model
/// </summary>
public class CreateDeviceRequestModel 
{
    
    /// <summary>
    /// ignored for device creation no needed
    /// </summary>
    [JsonIgnore]
    public int Id { get; set; }
    
    /// <example> device1</example>
    [StringLength(150, ErrorMessage = "Must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
    public string Name { get; init; } = null!;
    
    
    /// <example>172.1.1.0</example>
    [HostValidation("Must be a valid hostname or IP address")]
    [Required(AllowEmptyStrings =false,ErrorMessage ="Must be a valid hostname or IP address")]
    public string HostName { get; init; } = null!;
    
    
    /// <example>4000</example>
    [Range(1, 65536, ErrorMessage = "Port number must be between 1 to 65536.")]
    public int Port { get; init; }
    
    /// <example>1</example>
    public int? DeviceGatewayId { get; init; }
    
    
    ///<example>ExistingCredential</example>
    public string? CredentialName { get; set; }
    
    /// <summary>
    /// credentials
    /// </summary>
    public CreateCredentialRequestModel? Credential { get; set; }
}