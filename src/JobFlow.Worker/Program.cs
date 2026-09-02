using JobFlow.Application;
using JobFlow.Application.Workers;
using JobFlow.Infrastructure;
using JobFlow.Infrastructure.Configuration;
using JobFlow.Worker;

DotEnv.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IJobProcessor, JobProcessor>();
builder.Services.AddHostedService<JobConsumer>();

var host = builder.Build();
host.Run();
