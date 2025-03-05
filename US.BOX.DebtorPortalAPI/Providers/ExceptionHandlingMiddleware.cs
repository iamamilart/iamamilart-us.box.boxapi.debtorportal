using Microsoft.IdentityModel.Tokens;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.DebtorPortalAPI.Providers
{
 public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomtException ex) // Handle custom exception
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (SecurityTokenException ex) // Handle SecurityTokenException
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (ArgumentNullException ex) // Handle ArgumentNullException
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (ArgumentException ex) // Handle other argument-related bad requests
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (OperationCanceledException ex) {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (Exception ex) // Handle all other exceptions
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "An error occurred. Please try again later.",
                details = ex.Message
            });
        }
    }
}
 
}
