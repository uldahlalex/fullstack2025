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
            "\n" + // Line break before each log entry
            "[{@t:HH:mm:ss}] " + // Time
            "{SourceContext}[0] " + // Add namespace and class name with [0]
            "{#if SourceFile is not null}{#if SourceFile <> ''}" +
            "\u001b[34mFile: {SourceFile}, Line: {LineNumber}\u001b[0m" + // Filename and line number in blue
            "{#else}" +
            "No source information" + // Alternative text when no source info
            "{#end}{#end}" +
            "\n" + // Line break after the header
            "{@l:u3} {@m}" + // Level and message on the next line
            "\n" + // Extra line break after the message
            "{@x:l}", // Exception details
            theme: TemplateTheme.Literate);

        GoogleCloudLoggingSinkOptions config = null;
        if (builder.Environment.IsProduction())
            config = new GoogleCloudLoggingSinkOptions();
        var loggerConf = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.With<CallerEnricher>();
        if (!builder.Environment.IsProduction())
            loggerConf.WriteTo.Console(expression);
        else
        {
            loggerConf.WriteTo.GoogleCloudLogging( );
        }

        var logger = loggerConf.CreateLogger();


        Log.Logger = logger;
        builder.Host.UseSerilog();

        builder.Logging.ClearProviders();
        return builder;
    }
}