using Microsoft.Extensions.Configuration;

public static class AppsettingReader
{
    public static T ReadSection<T>(string sectionName)
    {
        var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables();
        var configurationRoot = builder.Build();

        return configurationRoot.GetSection(sectionName).Get<T>();
    }
}