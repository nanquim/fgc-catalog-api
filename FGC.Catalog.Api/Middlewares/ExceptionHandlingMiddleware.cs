using System.Net;
using System.Text.Json;

namespace FGC.Catalog.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Falha de autenticação | Method: {Method} | Path: {Path}",
                context.Request.Method, context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.Unauthorized, "Credenciais inválidas");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Requisição inválida | Method: {Method} | Path: {Path}",
                context.Request.Method, context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado | Method: {Method} | Path: {Path}",
                context.Request.Method, context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "Ocorreu um erro interno.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}
