using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrgChart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrgChart.API.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "API-Key";
        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.Headers.Append("Content-Type", "application/json");
                context.Response.StatusCode = 401;
                string response = JsonSerializer.Serialize(new { IsSuccess = false, Message = "API key is required!" });
                await context.Response.WriteAsync(response);

                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IOptionsMonitor<AppSettings>>();

            var apiKey = appSettings.CurrentValue.APIKey;

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.Headers.Append("Content-Type", "application/json");
                context.Response.StatusCode = 401;
                string response = JsonSerializer.Serialize(new { IsSuccess = false, Message = "Invalid API key!" });
                await context.Response.WriteAsync(response);
                return;
            }

            await _next(context);

            //context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        }
    }
}
