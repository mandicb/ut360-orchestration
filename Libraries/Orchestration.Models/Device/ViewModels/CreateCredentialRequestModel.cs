using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Orchestration.Models.Device.ViewModels;

/// <summary>
/// Create credential model
/// </summary>
public partial class CreateCredentialRequestModel
{
    [JsonIgnore]
    public int Id { get; set; }
    
    /// <example>credential1</example>
    public string Name { get; init; } = null!;
    
    
    /// <example>admin</example>
    public string UserName { get; set; }  = null!;
    
    
    /// <example>12334245 </example>
    [DefaultValue(null)]
    public string? KeyPassword { get; set; }


    /// <example>keyData base64 encoded string</example>


    private string _keyData;

    public string KeyData
    {
        get => _keyData;
        set
        {
            var r  = KeyRegex();
            _keyData = value;
            _keyData = r.Replace(KeyData, "\n").Replace("\t", "").Replace("\r", "");
        }
    }

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex KeyRegex();
}

