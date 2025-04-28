using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EventsourcingSandbox.Setup;

public class ApplicationFactory<TProgram>(string databaseConnectionString) : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testConfigurationSettings = new Dictionary<string, string?>()
        {
            { "ConnectionStrings:Marten", databaseConnectionString }
        };
        
        var testConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(testConfigurationSettings)
            .Build();
        
        builder.UseConfiguration(testConfiguration);
        
        base.ConfigureWebHost(builder);
    }
}