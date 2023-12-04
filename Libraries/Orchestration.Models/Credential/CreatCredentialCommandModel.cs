using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Orchestration.Models.Credential;

public class CreatCredentialCommandModel
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string UserName { get; set; }
    
    public string? KeyPassword { get; set; }
    
    //TODO: Rename to Data content
    public string KeyData { get; set; } = null!;

}