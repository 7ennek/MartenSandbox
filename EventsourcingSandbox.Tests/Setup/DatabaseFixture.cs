using Testcontainers.PostgreSql;

namespace EventsourcingSandbox.Setup;

public class DatabaseFixture
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    public string ConnectionString => _postgreSqlContainer.GetConnectionString();
    
    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public async Task Stop()
    {
        await _postgreSqlContainer.StopAsync();
    }
}