namespace Ordering.Application;

// Flat mocked rates — no real courier integration, matches the checkout payment methods being mocked too.
public static class ExpeditionRates
{
    private static readonly Dictionary<string, decimal> Rates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["JNE"] = 15_000m,
        ["JNT"] = 14_000m,
        ["SiCepat"] = 12_000m,
        ["GoSend"] = 20_000m,
    };

    public static IReadOnlyDictionary<string, decimal> All => Rates;

    public static decimal? Resolve(string courier) => Rates.GetValueOrDefault(courier);
}
