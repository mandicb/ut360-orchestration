using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Orchestration.Models.Orchestration;

public class CreateChsmCommandModel
{
    [Range(1, 32, ErrorMessage = "Slot number must be between 1 to 32")]
    public int Slot { get; set; } 
    public string? PublicAdminKey { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateVersion { get; set; }
}