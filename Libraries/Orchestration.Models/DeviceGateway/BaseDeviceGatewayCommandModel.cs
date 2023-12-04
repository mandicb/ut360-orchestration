using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Orchestration.Models.DeviceGateway;


//TODO: REMOVE VALIDATION!!!
public class BaseDeviceGatewayCommandModel
{
    
   
    public int Id { get; set; }
    
    
    [StringLength(150, ErrorMessage = "Must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
    public string Name { get; set; } = null!;
    
    [HostNameValidation("Must be a valid hostname or IP address")]
    [Required(AllowEmptyStrings =false,ErrorMessage ="Must be a valid hostname or IP address")]
    public string HostName { get; set; } = null!;
    
    
    [Range(1, 65536, ErrorMessage = "Port number must be between 1 to 65536.")]
    public int Port { get; set; }
    
    [StringLength(150, ErrorMessage = "Must be at least {2} and at most {1} characters long.", MinimumLength = 1)]   
    public string Password { get; set; } = null!;

    public byte[] Certificate { get; set; } = null!;

}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class HostNameValidationAttribute : ValidationAttribute
{
    static readonly Regex ValidHostnameRegex = new(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)+([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$", RegexOptions.IgnoreCase);
    static readonly Regex ValidIPv6Regex = new(@"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$", RegexOptions.IgnoreCase);

    public HostNameValidationAttribute(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public override bool IsValid(object value)
    {
        return ValidHostnameRegex.IsMatch((string)value) || ValidIPv6Regex.IsMatch((string)value);
    }
}   