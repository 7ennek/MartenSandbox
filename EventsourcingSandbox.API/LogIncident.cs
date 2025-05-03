using Marten.Events;
using Wolverine.Http;
using Wolverine.Marten;

namespace EventsourcingSandbox.API;

public record LogIncident(
    Guid CustomerId,
    Contact Contact,
    string Description,
    Guid LoggedBy
);

public static class LogIncidentEndpoint
{
    [WolverinePost("/api/incidents")]
    public static (CreationResponse<Guid>, IStartStream) Post(LogIncident command)
    {
        var (customerId, contact, description, loggedBy) = command;

        var logged = new IncidentLogged(customerId, contact, description, loggedBy);
        var start = MartenOps.StartStream<Incident>(logged);

        var response = new CreationResponse<Guid>("/api/incidents/" + start.StreamId, start.StreamId);
        
        return (response, start);
    }
}

public static class IncidentLoggedHandler
{
    // Do something in the background, like assign it to someone,
    // send out emails or texts, alerts, whatever
    public static void Handle(IEvent<IncidentLogged> created, ILogger logger)
    {
        logger.LogInformation("Got a new IncidentLogged event for " + created.Id);
    }
}