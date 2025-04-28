using EventsourcingSandbox.Setup;

namespace EventsourcingSandbox;

[Collection(ApplicationCollection.Key)]
public class UnitTest1(ApplicationFixture fixture) : BaseApplicationTest(fixture)
{
    [Fact]
    public async Task Test1()
    {
        var test = await Client.GetAsync("/", CancellationToken.None);
    }
}