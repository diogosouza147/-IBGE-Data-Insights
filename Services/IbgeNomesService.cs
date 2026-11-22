using IbgeNomes.Models;

namespace IbgeNomes.Services;

/// <summary>Cliente da API de Nomes (Censo 2022) do IBGE.</summary>
public class IbgeNomesService
{
    private const string Base = "https://servicodados.ibge.gov.br/api/v3/nomes/2022/localidade";
    private readonly HttpClient _http;

    public IbgeNomesService(HttpClient http) => _http = http;

    public async Task<ResultadoNomes> ObterPorLocalidadeAsync(int idLocalidade, CancellationToken ct = default)
    {
        var json = await _http.GetStringAsync($"{Base}/{idLocalidade}", ct);
        return NomesJsonParser.Parse(json);
    }
}
