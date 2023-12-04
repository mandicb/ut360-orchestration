using System.Net;

namespace Orchestration.Models;

/// <summary>
/// Base response model
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseResponseModel<T>
{
    
    /// <example>200</example>
    public ulong StatusCode { get; set; } = 200;
    
    
    /// <example>Ok</example>
    public string StatusCodeDescription { get; set; } = $"{HttpStatusCode.OK}";
    
    public T? Payload { get; set; }
   
    /// <summary>
    /// <example>Internal server error</example>
    /// </summary>
    public string? Error { get; set; }
        
        
    public BaseResponseModel() {}

    public BaseResponseModel(T? payload)
    {
        Payload = payload;
    }
}