namespace US.BOX.DebtorPortalAPI.Providers
{
    public class CancellationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CancellationMiddleware> _logger;

        public CancellationMiddleware(RequestDelegate next, ILogger<CancellationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context == null)
                {
                    return;
                }
                else {
                    var xx = "sad";
                }

                context.RequestAborted.Register(() =>
                {
                    _logger.LogInformation("Request was cancelled by the client for path: {Path}", context.Request.Path);
                });

                await _next(context);
            }
            catch (OperationCanceledException)
            {
                 _logger.LogInformation("Operation cancelled for request path: {Path}", context.Request.Path);
                throw;
            }
        }
    }

    public static class CancellationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCancellationHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CancellationMiddleware>();
        }
    }
}
