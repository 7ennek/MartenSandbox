using System.Net.Http.Json;
using EventsourcingSandbox.API;
using EventsourcingSandbox.Helpers;
using EventsourcingSandbox.Setup;
using Shouldly;
using Wolverine.Marten;

namespace EventsourcingSandbox;

[Collection(ApplicationCollection.Key)]
public class CategorisingIncidents(ApplicationFixture fixture) : BaseApplicationTest(fixture)
{
    [Fact]
    public async Task categorising_should_be_possible()
    {
        var logged = new IncidentLogged(Some.Guid, new Contact(ContactChannel.Email), Some.String, Some.Guid);
        var incidentId = await StartStream(logged);

        var response = await Client.PostAsJsonAsync($"/api/incidents/{incidentId}/category", new
        {
            Category = IncidentCategory.Software,
            CategorisedBy = Some.Guid,
            Version = 1
        });
        response.EnsureSuccessStatusCode();
        
        var result = await Fetch<Incident>(incidentId);
        result.Status.ShouldBe(IncidentStatus.Pending);        
    }
}