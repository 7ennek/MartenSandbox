using EventsourcingSandbox.API;
using Marten;

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

    protected HttpClient Client => application.CreateClient();

    protected async Task<T> Fetch<T>(Guid streamId) where T : class
    {
        await using var session = application.CreateSession();
        return await session.Events.AggregateStreamAsync<T>(streamId) ?? throw new Exception($"Stream {streamId} not found");
    }

    protected async Task<Guid> StartStream<T>(T @event) where T : class
    {
        await using var session = application.CreateSession();
        var startStream = session.Events.StartStream<T>(@event);
        await session.SaveChangesAsync();
        return startStream.Id;
    }
}