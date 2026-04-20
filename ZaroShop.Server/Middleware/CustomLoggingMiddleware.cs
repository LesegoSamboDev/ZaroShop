public class CustomLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public CustomLoggingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"[LOG] Requesting: {context.Request.Path} at {DateTime.Now}");

        await _next(context);
    }
}