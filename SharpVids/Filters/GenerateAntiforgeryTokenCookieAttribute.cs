using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SharpVids.Filters;

// From: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0
[AttributeUsage(AttributeTargets.Method)]
public sealed class GenerateAntiforgeryTokenCookieAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var antiforgery = context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();

        var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);

        // Append the antiforgery token in a javascript readable way
        context.HttpContext.Response.Cookies.Append(
            "RequestVerificationToken",
            tokens.RequestToken!,
            new CookieOptions() { HttpOnly = false }
        );
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
