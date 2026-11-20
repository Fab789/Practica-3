using System.Text.Json;

namespace PAW.MinimalAPI.Services;

public class TipoCambioService : ITipoCambioService
{
    private readonly HttpClient _http;

    public TipoCambioService(HttpClient http) => _http = http;

    public async Task<TipoCambio?> GetAsync()
    {
        try
        {
            using var stream = await _http.GetStreamAsync("https://api.hacienda.go.cr/indicadores/tc/dolar");
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;
            var compra = root.GetProperty("compra").GetProperty("valor").GetDecimal();
            var venta = root.GetProperty("venta").GetProperty("valor").GetDecimal();
            var fecha = root.GetProperty("venta").GetProperty("fecha").GetString() ?? "";
            return new TipoCambio(compra, venta, fecha);
        }
        catch
        {
            return null;
        }
    }
}
