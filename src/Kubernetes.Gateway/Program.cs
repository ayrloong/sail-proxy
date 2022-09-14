using Microsoft.Rest;
using Sail.Kubernetes.Gateway;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var serilog = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();

        ServiceClientTracing.IsEnabled = true;

        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config => { config.AddJsonFile("/app/config/sail.json", optional: true); })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(serilog, dispose: false);
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .Build()
            .RunAsync().ConfigureAwait(false);
    }
}