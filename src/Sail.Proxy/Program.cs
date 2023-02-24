using Sail.Proxy;

public static class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHost(_ => { })
            .ConfigureAppConfiguration(config => { config.AddJsonFile("/app/config/yarp.json", optional: true); })
            .ConfigureLogging(_ => {  })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); }).Build().Run();
    }
}