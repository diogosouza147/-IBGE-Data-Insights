using System.Diagnostics;
using System.Text.Json;
using IbgeNomes.Models;

namespace IbgeNomes.Services;

/// <summary>
/// Converte o JSON da API de Nomes em ResultadoNomes. Percorre a árvore e coleta todo objeto que
/// tenha "nome" + ("frequencia" | "ranking"), herdando o contexto (sexo / sobrenome) dos pais.
/// Se o IBGE mudar o formato, só este arquivo precisa ser ajustado.
/// </summary>
public static class NomesJsonParser
{
    public static ResultadoNomes Parse(string json)
    {
        Debug.WriteLine($"[IBGE Nomes] {(json.Length > 2000 ? json[..2000] : json)}");
        var r = new ResultadoNomes();
        using var doc = JsonDocument.Parse(json);
        Walk(doc.RootElement, null, false, r);

        Ordenar(r.Homens); Ordenar(r.Mulheres); Ordenar(r.Sobrenomes);
        return r;
    }

    private static void Walk(JsonElement el, string? sexo, bool sobrenome, ResultadoNomes r)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Array:
                foreach (var child in el.EnumerateArray()) Walk(child, sexo, sobrenome, r);
                break;

            case JsonValueKind.Object:
                // contexto herdado
                foreach (var p in el.EnumerateObject())
                {
                    var n = p.Name.ToLowerInvariant();
                    if (p.Value.ValueKind == JsonValueKind.String)
                    {
                        var v = p.Value.GetString()?.ToLowerInvariant() ?? "";
                        if (n is "sexo" or "genero")
                        {
                            if (v is "m" or "masculino" or "homens") sexo = "M";
                            else if (v is "f" or "feminino" or "mulheres") sexo = "F";
                        }
                        else if (n is "grupo" or "tipo" or "categoria" && v.Contains("sobrenome")) sobrenome = true;
                    }
                }

                if (EhItem(el, out var item))
                {
                    if (sobrenome) r.Sobrenomes.Add(item);
                    else if (sexo == "M") r.Homens.Add(item);
                    else if (sexo == "F") r.Mulheres.Add(item);
                    return;
                }

                foreach (var p in el.EnumerateObject())
                {
                    var n = p.Name.ToLowerInvariant();
                    var s = sexo; var sob = sobrenome;
                    if (n.Contains("sobrenome")) sob = true;
                    else if (n.Contains("masculin") || n.Contains("homem") || n.Contains("homens")) s = "M";
                    else if (n.Contains("feminin") || n.Contains("mulher")) s = "F";
                    Walk(p.Value, s, sob, r);
                }
                break;
        }
    }

    private static bool EhItem(JsonElement o, out NomeFrequencia item)
    {
        item = new NomeFrequencia();
        if (!Tem(o, "nome", out var nome) || nome.ValueKind != JsonValueKind.String) return false;
        var temFreq = Tem(o, "frequencia", out var f);
        var temRank = Tem(o, "ranking", out var rk) || Tem(o, "rank", out rk);
        if (!temFreq && !temRank) return false;

        item.Nome = Capitalizar(nome.GetString() ?? "");
        if (temFreq && f.TryGetInt64(out var freq)) item.Frequencia = freq;
        if (temRank && rk.TryGetInt32(out var pos)) item.Posicao = pos;
        return true;
    }

    private static bool Tem(JsonElement o, string nome, out JsonElement v)
    {
        foreach (var p in o.EnumerateObject())
            if (p.Name.Equals(nome, StringComparison.OrdinalIgnoreCase)) { v = p.Value; return true; }
        v = default; return false;
    }

    private static void Ordenar(List<NomeFrequencia> l)
    {
        l.Sort((a, b) => a.Posicao > 0 && b.Posicao > 0 ? a.Posicao.CompareTo(b.Posicao) : b.Frequencia.CompareTo(a.Frequencia));
        for (int i = 0; i < l.Count; i++) if (l[i].Posicao == 0) l[i].Posicao = i + 1;
        var max = l.Count > 0 ? l.Max(x => x.Frequencia) : 0;
        foreach (var x in l) x.Proporcao = max > 0 ? (double)x.Frequencia / max : 0;
    }

    private static string Capitalizar(string s)
        => s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..].ToLowerInvariant();
}
