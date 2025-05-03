using EventsourcingSandbox.API;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Oakton;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMarten(opts =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Marten");
        opts.Connection(connectionString!);
        opts.DatabaseSchemaName = "incidents";
        
        opts.Projections.Add<LoggedIncidentProcessor>(ProjectionLifecycle.Async);
    })
    .UseLightweightSessions()
    // This adds configuration with Wolverine's transactional outbox and
    // Marten middleware support to Wolverine
    .IntegrateWithWolverine()
    .PublishEventsToWolverine("Everything")
    .AddAsyncDaemon(DaemonMode.Solo);

builder.Host.UseWolverine(opts =>
{
    // This is almost an automatic default to have
    // Wolverine apply transactional middleware to any
    // endpoint or handler that uses persistence services
    opts.Policies.AutoApplyTransactions();
});

// To add Wolverine.HTTP services to the IoC container
builder.Services.AddWolverineHttp();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapWolverineEndpoints();

// This is using the Oakton library for command running
// await app.RunOaktonCommands(args);
await app.RunAsync();