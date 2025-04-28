namespace EventsourcingSandbox.Setup;

public abstract class BaseApplicationTest(ApplicationFixture application) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await application.Reset();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    
    public HttpClient Client => application.CreateClient();
}