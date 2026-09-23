using Serilog.Core;
using Serilog.Events;

namespace BuildingBlocks.Logging;

// Tags log events so the Postgres sink (logs.application_log) and Seq stream stay filterable consistently.
public sealed class ApplicationEnricher : ILogEventEnricher
{
    private const string ApplicationName = "Gentongku.Api";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Application", ApplicationName));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("MachineName", Environment.MachineName));
    }
}

// Module code sets this property via LogContext.PushProperty(ModuleLogContext.ModulePropertyName, ...).
public static class ModuleLogContext
{
    public const string ModulePropertyName = "Module";
}
