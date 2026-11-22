using System.Net.Http.Json;
using IbgeNomes.Models;

namespace IbgeNomes.Services;

/// <summary>Cliente da API de Localidades do IBGE.</summary>
public class IbgeLocalidadesService
{
    private const string Base = "https://servicodados.ibge.gov.br/api/v1/localidades";
    private readonly HttpClient _http;

    public IbgeLocalidadesService(HttpClient http) => _http = http;

    public async Task<List<Estado>> ObterEstadosAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<List<Estado>>($"{Base}/estados?orderBy=nome", ct) ?? new();

    public async Task<List<Municipio>> ObterMunicipiosAsync(int estadoId, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<List<Municipio>>($"{Base}/estados/{estadoId}/municipios?orderBy=nome", ct) ?? new();
}
