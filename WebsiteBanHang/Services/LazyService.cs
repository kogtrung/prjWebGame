using System;
using Microsoft.Extensions.DependencyInjection;

namespace WebGame.Services
{
    /// <summary>
    /// Service that creates a Lazy<T> that resolves the given service when the Value property is accessed
    /// </summary>
    /// <typeparam name="T">The type of service to resolve</typeparam>
    public class LazyService<T> : Lazy<T> where T : class
    {
        public LazyService(IServiceProvider serviceProvider)
            : base(() => serviceProvider.GetRequiredService<T>())
        {
        }
    }
} 