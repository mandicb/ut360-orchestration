using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Swashbuckle.AspNetCore.Annotations;

namespace Orchestration.Models.Device;

[SwaggerSchema("BaseDeviceCommandModel")]
public class BaseDeviceCommandModel
{
    public int Id { get; set; }
    public string Name { get; init; } = null!;
    public string HostName { get; init; } = null!;
    public int Port { get; init; }
}

