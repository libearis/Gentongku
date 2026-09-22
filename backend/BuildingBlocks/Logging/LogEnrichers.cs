using Serilog.Core;
using Serilog.Events;

namespace BuildingBlocks.Logging;

/// <summary>
/// Tags every log event with the Gentongku application name and a machine name,
/// so the Postgres sink (logs.application_log) and Seq stream can both be filtered
/// consistently across the modular monolith. Registered from Host/Program.cs.
/// See AGENTS.md section 9 (Logging & observability).
/// </summary>
public sealed class ApplicationEnricher : ILogEventEnricher
{
    private const string ApplicationName = "Gentongku.Api";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Application", ApplicationName));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("MachineName", Environment.MachineName));
    }
}

/// <summary>
/// Enriches log events with the module name they originated from, when a module's
/// Infrastructure logger sets the "Module" property via LogContext.PushProperty.
/// Left as a thin marker/helper for module code to use consistently.
/// </summary>
public static class ModuleLogContext
{
    public const string ModulePropertyName = "Module";
}
