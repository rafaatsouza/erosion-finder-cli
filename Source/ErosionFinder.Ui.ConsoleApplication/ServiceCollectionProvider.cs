using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ErosionFinder.Ui.ConsoleApplication
{
    static class ServiceCollectionProvider
    {
        private static readonly ServiceProvider serviceProvider = GetServiceProvider();

        private static ServiceProvider GetServiceProvider()
         => new ServiceCollection()
                .AddErosionFinder()
                .AddLogging(c =>
                {
                    c.AddSerilog(LoggerConfigurationProvider.LoggerConfiguration.CreateLogger());
                })
                .BuildServiceProvider();

        public static T GetService<T>() where T : class
            => serviceProvider.GetService<T>();
    }
}