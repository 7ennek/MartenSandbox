using System.Net.Http.Json;
using EventsourcingSandbox.API;
using EventsourcingSandbox.Helpers;
using EventsourcingSandbox.Setup;
using Shouldly;
using Wolverine.Http;

namespace EventsourcingSandbox;

[Collection(ApplicationCollection.Key)]
public class LoggingIncidents(ApplicationFixture fixture) : BaseApplicationTest(fixture)
{
    [Fact]
    public void logging_should_be_possible()
    {
        var contact = new Contact(ContactChannel.Email);
        var command = new LogIncident(Guid.NewGuid(), contact, "It's broken", Guid.NewGuid());

        // Pure function FTW!
        var (response, startStream) = LogIncidentEndpoint.Post(command);
        
        // Should only have the one event
        startStream.Events.ShouldBe([
            new IncidentLogged(command.CustomerId, command.Contact, command.Description, command.LoggedBy)
        ]);
        
        response.ShouldNotBeNull();
        response.Value.ShouldNotBe(Guid.Empty);
    }
    
    [Fact]
    public async Task logging_should_be_possible_through_endpoint()
    {
        var response = await Client.PostAsJsonAsync($"/api/incidents", new
        {
            CustomerId = Some.Guid,
            Contact = new {
                ContactChannel.Email,
            },
            Description = Some.String,
            LoggedBy = Some.Guid
        });
        response.EnsureSuccessStatusCode();

        var incidentId = await response.Content.ReadFromJsonAsync<CreationResponse<Guid>>();
        incidentId.ShouldNotBeNull();
        incidentId.Value.ShouldNotBe(Guid.Empty);
    }
}