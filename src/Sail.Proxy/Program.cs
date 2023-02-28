using Sail.Proxy;

public static class Program
{
    public static void Main(string[] args)
    {


        Host.CreateDefaultBuilder(args)
            .ConfigureWebHost(_ => { })
            .ConfigureLogging(_ => { })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); }).Build().Run();
    }
}