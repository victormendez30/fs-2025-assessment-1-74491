using Microsoft.Azure.Cosmos;
using fs_2025_assessment_1_74491.Services;

namespace fs_2025_assessment_1_74491.Startup;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IStationService, StationService>();
        builder.Services.AddHostedService<StationUpdateBackgroundService>();

        var endpointUri = builder.Configuration["CosmosDb:EndpointUri"];
        var primaryKey = builder.Configuration["CosmosDb:PrimaryKey"];

        if (!string.IsNullOrWhiteSpace(endpointUri) &&
            !string.IsNullOrWhiteSpace(primaryKey))
        {
            var cosmosClient = new CosmosClient(endpointUri, primaryKey);
            builder.Services.AddSingleton(cosmosClient);
            builder.Services.AddSingleton<IStationServiceV2, CosmosStationService>();
        }
    }
}
