using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace EventsourcingSandbox.Setup;

public class ApplicationFixture : IAsyncLifetime
{
    private readonly DatabaseFixture _database = new();
    private ApplicationFactory<Program>? _applicationFactory;
    
    public async Task InitializeAsync()
    {
        await _database.InitializeAsync();
        _applicationFactory = new ApplicationFactory<Program>(_database.ConnectionString);
    }

    public async Task DisposeAsync()
    {
        await _database.Stop();
    }
    
    public HttpClient CreateClient()
    {
        if(_applicationFactory is null) throw new ArgumentNullException(nameof(_applicationFactory));
        return _applicationFactory.CreateClient();
    }

    public async Task Reset()
    {
        if (_applicationFactory is null) return;
        var docStore = _applicationFactory.Services.GetRequiredService<IDocumentStore>();
        await docStore.Advanced.ResetAllData();
    }
}

[CollectionDefinition(ApplicationCollection.Key)]
public class ApplicationCollection : ICollectionFixture<ApplicationFixture>
{
    public const string Key = "Application collection";
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}