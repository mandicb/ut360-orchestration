using System.ComponentModel.DataAnnotations;

namespace Orchestration.Models.DeviceGateway;

public class CreateDeviceGatewayViewModel
{
    //TODO REMOVE  CREATE SAM AS ON DEVICE
    [StringLength(150, ErrorMessage = "Must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
    public string Name { get; set; } = null!;
    
    [HostNameValidation("Must be a valid hostname or IP address")]
    [Required(AllowEmptyStrings =false,ErrorMessage ="Must be a valid hostname or IP address")]
    public string HostName { get; set; } = null!;
    
    
    [Range(1, 65536, ErrorMessage = "Port number must be between 1 to 65536.")]
    public int Port { get; set; }
    
    [StringLength(150, ErrorMessage = "Must be at least {2} and at most {1} characters long.", MinimumLength = 1)]   
    public string Password { get; set; } = null!;

}