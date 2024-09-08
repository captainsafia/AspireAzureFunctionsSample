using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .AddServiceDefaults()
    .ConfigureFunctionsWebApplication()
    .Build();

host.Run();
