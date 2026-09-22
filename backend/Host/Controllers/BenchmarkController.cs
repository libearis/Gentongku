using Benchmark.Application.Abstractions;
using Benchmark.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gentongku.Api.Controllers;

/// <summary>
/// Trivial passthrough endpoints proving the Benchmark module is wired
/// end-to-end. Real strategy execution is TODO — see Benchmark.Infrastructure/Services/BenchmarkService.cs.
/// </summary>
[ApiController]
[Route("api/benchmark")]
public class BenchmarkController : ControllerBase
{
    private readonly IBenchmarkService _benchmarkService;

    public BenchmarkController(IBenchmarkService benchmarkService) => _benchmarkService = benchmarkService;

    [HttpPost("read")]
    public async Task<IActionResult> RunRead(ReadBenchmarkRequest request, CancellationToken ct) =>
        Ok(await _benchmarkService.RunReadBenchmarkAsync(request, ct));

    [HttpPost("write")]
    public async Task<IActionResult> RunWrite(WriteBenchmarkRequest request, CancellationToken ct) =>
        Ok(await _benchmarkService.RunWriteBenchmarkAsync(request, ct));
}
