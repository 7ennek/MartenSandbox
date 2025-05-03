using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace EventsourcingSandbox.API;

public record LoggedIncidentTodo(Guid Id, bool Reported);

public class LoggedIncidentProcessor : SingleStreamProjection<LoggedIncidentTodo>
{
    public LoggedIncidentTodo Create(IEvent<IncidentLogged> @event)
    {
        return new LoggedIncidentTodo(@event.StreamId, false);
    }

    public override ValueTask RaiseSideEffects(IDocumentOperations operations, IEventSlice<LoggedIncidentTodo> slice)
    {
        var projection = slice.Aggregate;
        if (projection is null || projection.Reported) return ValueTask.CompletedTask;
        slice.PublishMessage(new NotifyResponsible(projection.Id));
        return ValueTask.CompletedTask;
    }
}

public record NotifyResponsible(Guid IncidentId);

public static class NotifyResponsibleHandler
{
    public static Task<LoggedIncidentTodo?> LoadAsync(NotifyResponsible e, IQuerySession session,
        CancellationToken cancellationToken)
        => session.LoadAsync<LoggedIncidentTodo>(e.IncidentId, cancellationToken);
    
    public static Task Handle(NotifyResponsible command, LoggedIncidentTodo todo, IDocumentSession session)
    {
        //do something
        //...
        
        session.Update(todo with { Reported = true });
        return Task.CompletedTask;
    }
}