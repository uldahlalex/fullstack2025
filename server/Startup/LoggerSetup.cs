using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace Startup;

public static class LoggerSetup
{
    public static WebApplicationBuilder AddSuperAwesomeLoggingConfig(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.With<CallerEnricher>()
            .WriteTo.Console(new ExpressionTemplate(
                "\n" + // Line break before each log entry
                "[{@t:HH:mm:ss}] " + // Time
                "{#if SourceFile is not null}{#if SourceFile <> ''}" +
                "\u001b[34mFile: {SourceFile}, Line: {LineNumber}\u001b[0m" + // Filename and line number in blue
                "{#else}" +
                "No source information" + // Alternative text when no source info
                "{#end}{#end}" +
                "\n" + // Line break after the header
                "{@l:u3} {@m}" + // Level and message on the next line
                "\n" + // Extra line break after the message
                "{@x:l}", // Exception details
                theme: TemplateTheme.Literate)).CreateLogger();
        builder.Host.UseSerilog();

        builder.Logging.ClearProviders();
        return builder;
    }
}