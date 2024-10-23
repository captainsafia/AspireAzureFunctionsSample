using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Builder;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddAzureCosmosClient("cosmos-db");

builder.AddServiceDefaults();

var host = builder.Build();

host.Run();
