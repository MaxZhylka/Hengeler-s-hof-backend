using System.Net;
using System.Text.Json;

namespace Hengeler.API.Middleware;
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
  private readonly RequestDelegate _next = next;

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex);
    }
  }

  private static Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";

    var (statusCode, message) = exception switch
    {
      ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
      InvalidOperationException => (HttpStatusCode.Conflict, exception.Message),
      KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
      _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
    };

    context.Response.StatusCode = (int)statusCode;

    var result = JsonSerializer.Serialize(new
    {
      error = message,
      statusCode = (int)statusCode
    });

    Console.WriteLine(message);
    return context.Response.WriteAsync(result);
  }
}
