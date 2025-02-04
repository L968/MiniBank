using MiniBank.Api.Infrastructure;
using MiniBank.MigrationService;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddMySqlDbContext<AppDbContext>("minibank-mysqldb");

IHost host = builder.Build();
await host.RunAsync();
