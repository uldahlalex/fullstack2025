using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Startup;

public class CallerEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var skip = 3;
        var maxFrames = 20;

        while (skip < maxFrames)
        {
            var stack = new StackFrame(skip, true);
            if (!stack.HasSource())
            {
                skip++;
                continue;
            }

            var method = stack.GetMethod();
            if (method?.DeclaringType != null)
            {
                var declaringType = method.DeclaringType.FullName ?? "";

                if (declaringType == typeof(CallerEnricher).FullName ||
                    declaringType.StartsWith("Serilog.") ||
                    declaringType.StartsWith("Microsoft.Extensions.Logging") ||
                    declaringType.Contains("LoggerExtensions"))
                {
                    skip++;
                    continue;
                }

                var fileName = stack.GetFileName();
                var lineNumber = stack.GetFileLineNumber();
                var methodName = method.Name;

                if (string.IsNullOrEmpty(fileName) || lineNumber <= 0)
                {
                    skip++;
                    continue;
                }

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "SourceFile", fileName));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "LineNumber", lineNumber));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "MemberName", methodName));

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "SourceContext", declaringType));

                break;
            }

            skip++;
        }
    }
}