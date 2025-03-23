using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace HotelBookingAPI.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint != null && endpoint.Metadata.Any(meta => meta is Microsoft.AspNetCore.Authorization.AuthorizeAttribute))
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    //  Check if the response has already started before modifying it
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json"; //  Add this only if response hasn't started
                        await context.Response.WriteAsJsonAsync(new { Message = "You are not authenticated!" });
                        return;
                    }
                }

                var roles = endpoint.Metadata
                    .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                    .SelectMany(attr => attr.Roles?.Split(',') ?? Enumerable.Empty<string>())
                    .ToList();

                if (roles.Any() && !roles.Any(role => context.User.IsInRole(role)))
                {
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json"; 
                        await context.Response.WriteAsJsonAsync(new { Message = "You are not authorized to access this resource!" });
                        return;
                    }
                }
            }

            await _next(context); //  Pass to the next middleware
        }
    }
}
