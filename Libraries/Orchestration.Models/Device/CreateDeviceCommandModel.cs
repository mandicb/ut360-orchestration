using MediatR;
using Orchestration.Models.Credential;

namespace Orchestration.Models.Device;

/// <summary>
/// 
/// </summary>
public class CreateDeviceCommandModel : BaseDeviceCommandModel, IRequest<int>
{

    //TODO: NAME VALIDATION UNIQ??
    public int? DeviceGatewayId { get; init; }

    public string? CredentialName { get; set; }
    public CreatCredentialCommandModel? Credential { get; set; } 

}
