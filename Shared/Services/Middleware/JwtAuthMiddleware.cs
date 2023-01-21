using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;
        public JwtAuthMiddleware(RequestDelegate next, IJwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jwtModel = _jwtService.ValidateToken(context.Request);
            context.Items.Add("jwtModel", jwtModel);
            await _next(context);
        }
    }
}
