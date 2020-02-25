using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace StreamOutputWebApp.Extensions
{
    public static class WebHostEnvironmentExtensions
    {
        public static bool IsDevelopment(this IWebHostEnvironment hostEnvironment)
        {
            if (hostEnvironment is null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return string.Equals(
                hostEnvironment.EnvironmentName,
                Environments.Development,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}