namespace AzureFunctionsTests.Tests.Tests;

public class IntegrationTest
{
    [Fact]
    public async Task GetHttpFuncAppHttpTrigger()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AzureFunctionsTest_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
    
        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("http-funcapp");
        await resourceNotificationService.WaitForResourceHealthyAsync("http-funcapp").WaitAsync(TimeSpan.FromSeconds(120));
        var response = await httpClient.GetAsync("/api/MyHttpTrigger");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expectedOutput = "Welcome to Azure Functions!";
        var actualOutput = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedOutput, actualOutput);

        await app.StopAsync();
    }
}
