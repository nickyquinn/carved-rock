using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;

namespace CarvedRock.Api
{
    public static class ConfigureRequestLogging
    {
        public static IApplicationBuilder UseCustomRequestLogging(this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = ExcludeHealthChecks;
            });
        }

        private static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex) =>
            ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
            ? LogEventLevel.Error
            : IsHealthCheckEndpoint(ctx)
            ? LogEventLevel.Verbose
            : LogEventLevel.Information;

        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var userAgent = ctx.Request.Headers["User-Agent"].FirstOrDefault() ?? "";
            return ctx.Request.Path.Value.EndsWith("health", StringComparison.CurrentCultureIgnoreCase) ||
                userAgent.Contains("HealthCheck", StringComparison.InvariantCultureIgnoreCase);
        }
    }
    
}

