namespace HotelBookingAPI.Middleware;

public class CustomAuthMiddleware
{
    private readonly RequestDelegate _next;

    public CustomAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\": \"You need to log in to access this resource.\"}");
        }

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\": \"You do not have permission to access this resource.\"}");
        }
    }

}
