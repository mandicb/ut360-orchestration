using System.Net;
using Newtonsoft.Json;
using Orchestration.Exceptions;
using Orchestration.Models;
using Serilog;

namespace Orchestration.API.Middleware;

public class ControllerExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ControllerExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var response = context.Response;
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}: {ex.StackTrace}");
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.ContentType = "application/json; charset=utf-8";
            var result = new BaseResponseModel<object>()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                StatusCodeDescription = $"{HttpStatusCode.BadRequest}"
            };

            switch (ex)
            {
                case QueryException:
                case CommandException:
                case ModelException:
                case CustomException:
                    result.Error = ex.Message;
                    break;
                default:
                    result.StatusCode = (int)HttpStatusCode.InternalServerError;
                    result.StatusCodeDescription = $"{HttpStatusCode.InternalServerError}";
                    result.Error = "Something went wrong, please contact system administrator.";
                    break;
            }
            

            await response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
public static class ControllerExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseControllerExceptionHandlingMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ControllerExceptionHandlingMiddleware>();
    }
}