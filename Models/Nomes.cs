namespace IbgeNomes.Models;

/// <summary>Um item de ranking: nome (ou sobrenome), frequência no Censo 2022 e posição.</summary>
public class NomeFrequencia
{
    public int Posicao { get; set; }
    public string Nome { get; set; } = "";
    public long Frequencia { get; set; }

    /// <summary>0..1 em relação ao nome mais frequente da lista (usado na barra).</summary>
    public double Proporcao { get; set; }

    public string Rotulo => Posicao switch { 1 => "🥇", 2 => "🥈", 3 => "🥉", _ => $"{Posicao}º" };

    public string FrequenciaFormatada => Frequencia > 0 ? $"{Frequencia:N0} pessoas" : "";
}

/// <summary>Resultado já separado para a UI.</summary>
public class ResultadoNomes
{
    public List<NomeFrequencia> Homens { get; set; } = new();
    public List<NomeFrequencia> Mulheres { get; set; } = new();
    public List<NomeFrequencia> Sobrenomes { get; set; } = new();
}
