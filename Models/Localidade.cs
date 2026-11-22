using System.Text.Json.Serialization;

namespace IbgeNomes.Models;

/// <summary>Estado (UF) retornado por /api/v1/localidades/estados.</summary>
public class Estado
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("sigla")] public string Sigla { get; set; } = "";
    [JsonPropertyName("nome")] public string Nome { get; set; } = "";

    public override string ToString() => $"{Nome} ({Sigla})";
}

/// <summary>Município retornado por /api/v1/localidades/estados/{uf}/municipios.</summary>
public class Municipio
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("nome")] public string Nome { get; set; } = "";

    public override string ToString() => Nome;
}
