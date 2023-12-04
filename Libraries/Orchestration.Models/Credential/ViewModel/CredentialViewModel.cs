namespace Orchestration.Models.Credential.ViewModel;

public class CredentialViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string UserName { get; set; }
    
    public string? KeyPassword { get; set; }
    
    public string KeyData { get; set; } = null!;
}