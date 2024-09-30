using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = FunctionsWebApplicationBuilder.CreateBuilder();

builder.AddServiceDefaults();

var host = builder.Build();

host.Run();
