using Serilog;
using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace Startup;

public static class LoggerSetup
{
    public static WebApplicationBuilder AddSuperAwesomeLoggingConfig(this WebApplicationBuilder builder)
    {
        var expression = new ExpressionTemplate(
            "\n" +
            "[{@t:HH:mm:ss}] " +
            "{SourceContext}[0] " +
            "{#if SourceFile is not null}{#if SourceFile <> ''}" +
            "\u001b[34mFile: {SourceFile}, Line: {LineNumber}\u001b[0m" +
            "{#else}" +
            "No source information" +
            "{#end}{#end}" +
            "\n" +
            "{@l:u3} {@m}" +
            "\n" +
            "{@x:l}",
            theme: TemplateTheme.Literate);

        var isGoogleCloud = IsRunningOnGoogleCloud();
        var loggerConf = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.With<CallerEnricher>();

        if (isGoogleCloud)
        {
            loggerConf.WriteTo.GoogleCloudLogging();
        }
        else
        {
            loggerConf.WriteTo.Console(expression);
        }

        var logger = loggerConf.CreateLogger();
        Log.Logger = logger;
        builder.Host.UseSerilog();
        builder.Logging.ClearProviders();
        
        return builder;
    }

    private static bool IsRunningOnGoogleCloud()
    {
        return Environment.GetEnvironmentVariable("K_SERVICE") != null || // Cloud Run
               Environment.GetEnvironmentVariable("GAE_SERVICE") != null || // App Engine
               Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST") != null || // GKE
               File.Exists("/proc/self/cgroup") && File.ReadAllText("/proc/self/cgroup").Contains("kubepods"); // Additional GKE check
    }


}