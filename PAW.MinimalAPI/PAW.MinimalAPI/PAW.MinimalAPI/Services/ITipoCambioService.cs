namespace PAW.MinimalAPI.Services;

public record TipoCambio(decimal Compra, decimal Venta, string Fecha);

public interface ITipoCambioService
{
    Task<TipoCambio?> GetAsync();
}
