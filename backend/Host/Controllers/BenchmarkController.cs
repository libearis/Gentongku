using Benchmark.Application.Abstractions;
using Benchmark.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gentongku.Api.Controllers;

// TODO: real strategy execution — see Benchmark.Infrastructure/Services/BenchmarkService.cs.
[ApiController]
[Route("api/benchmark")]
public class BenchmarkController(IBenchmarkService benchmarkService) : ControllerBase
{
    [HttpPost("read")]
    public async Task<IActionResult> RunRead(ReadBenchmarkRequest request, CancellationToken ct) =>
        Ok(await benchmarkService.RunReadBenchmarkAsync(request, ct));

    [HttpPost("write")]
    public async Task<IActionResult> RunWrite(WriteBenchmarkRequest request, CancellationToken ct) =>
        Ok(await benchmarkService.RunWriteBenchmarkAsync(request, ct));
}
