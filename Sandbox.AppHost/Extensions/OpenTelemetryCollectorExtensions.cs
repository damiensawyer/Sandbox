using Aspire.Hosting.Lifecycle;

namespace Sandbox.AppHost.Extensions;

public static class OpenTelemetryCollectorExtensions
{
    private const string DashboardOtlpUrlVariableName = "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL";
    private const string DashboardOtlpKeyVariableName = "AppHost:OtlpApiKey";

    /// <summary>
    /// Adds the OpenTelemetry Collector to the application.
    /// Inspired by Glenn Versweyveld https://github.com/Depechie/OpenTelemetryGrafana/tree/aspire
    /// Inspired by Martin Thwaites https://github.com/practical-otel/opentelemetry-aspire-collector/tree/main
    /// Inspired by Aspire Samples https://github.com/dotnet/aspire-samples/blob/main/samples/Metrics/MetricsApp.AppHost/OpenTelemetryCollector/OpenTelemetryCollectorResourceBuilderExtensions.cs
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IResourceBuilder<OpenTelemetryCollectorResource> AddOpenTelemetryCollector(this IDistributedApplicationBuilder builder, string otelConfig)
    {
        var collectorResource = new OpenTelemetryCollectorResource("otelcollector");
        var dashboardUrl = builder.Configuration[DashboardOtlpUrlVariableName] ?? "";
        var isHttps = dashboardUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase);
        var dashboardOtlpEndpoint = new HostUrl(dashboardUrl);

        var otel = builder
            .AddResource(collectorResource)
            .WithImage("ghcr.io/open-telemetry/opentelemetry-collector-releases/opentelemetry-collector-contrib", "latest")
            .WithEndpoint(port: 4317, targetPort: 4317, name: OpenTelemetryCollectorResource.GRPCEndpointName, scheme: isHttps ? "https" : "http")
            .WithEndpoint(port: 4318, targetPort: 4318, name: OpenTelemetryCollectorResource.HTTPEndpointName, scheme: isHttps ? "https" : "http")
            .WithBindMount(otelConfig, "/etc/otelcol-contrib/config.yaml")
            .WithEnvironment("ASPIRE_OTLP_ENDPOINT", $"{dashboardOtlpEndpoint}")
            .WithEnvironment("ASPIRE_API_KEY", builder.Configuration[DashboardOtlpKeyVariableName])
            .WithEnvironment("ASPIRE_INSECURE", isHttps ? "false" : "true");

        otel.ApplicationBuilder.Services.TryAddLifecycleHook<OltpEndpointVariableHook>();

        return otel;
    }
}
