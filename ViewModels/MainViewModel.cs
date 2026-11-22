using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IbgeNomes.Models;
using IbgeNomes.Services;

namespace IbgeNomes.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IbgeLocalidadesService _localidades;
    private readonly IbgeNomesService _nomes;
    private CancellationTokenSource? _cts;

    public MainViewModel(IbgeLocalidadesService localidades, IbgeNomesService nomes)
    {
        _localidades = localidades;
        _nomes = nomes;
    }

    public List<string> Tipos { get; } = new() { "Estado", "Município" };
    public ObservableCollection<Estado> Estados { get; } = new();
    public ObservableCollection<Municipio> Municipios { get; } = new();
    public ObservableCollection<NomeFrequencia> Homens { get; } = new();
    public ObservableCollection<NomeFrequencia> Mulheres { get; } = new();
    public ObservableCollection<NomeFrequencia> Sobrenomes { get; } = new();

    private string _tipo = "Estado";
    public string Tipo
    {
        get => _tipo;
        set
        {
            if (!Set(ref _tipo, value)) return;
            OnChanged(nameof(MostrarMunicipios));
            Limpar();
            // ao voltar para Estado, consulta o estado já escolhido
            if (value == "Estado" && EstadoSelecionado != null) _ = CarregarNomesAsync(EstadoSelecionado.Id, EstadoSelecionado.Nome);
            else if (value == "Município" && EstadoSelecionado != null) _ = CarregarMunicipiosAsync(EstadoSelecionado.Id);
        }
    }

    private int _aba;
    /// <summary>0 = Homens, 1 = Mulheres, 2 = Sobrenomes.</summary>
    public int AbaIndex
    {
        get => _aba;
        set { if (Set(ref _aba, value)) OnChanged(nameof(ListaAtual)); }
    }

    public ObservableCollection<NomeFrequencia> ListaAtual => AbaIndex switch
    {
        0 => Homens,
        1 => Mulheres,
        _ => Sobrenomes
    };

    public bool MostrarMunicipios => Tipo == "Município";

    private Estado? _estado;
    public Estado? EstadoSelecionado
    {
        get => _estado;
        set
        {
            if (!Set(ref _estado, value) || value == null) return;
            if (MostrarMunicipios) _ = CarregarMunicipiosAsync(value.Id);
            else _ = CarregarNomesAsync(value.Id, value.Nome);
        }
    }

    private Municipio? _municipio;
    public Municipio? MunicipioSelecionado
    {
        get => _municipio;
        set
        {
            if (!Set(ref _municipio, value) || value == null) return;
            _ = CarregarNomesAsync(value.Id, value.Nome);
        }
    }

    private bool _ocupado;
    public bool Ocupado { get => _ocupado; set => Set(ref _ocupado, value); }

    private string _titulo = "Selecione uma localidade";
    public string Titulo { get => _titulo; set => Set(ref _titulo, value); }

    private string _erro = "";
    public string Erro { get => _erro; set { Set(ref _erro, value); OnChanged(nameof(TemErro)); } }
    public bool TemErro => !string.IsNullOrEmpty(Erro);

    public async Task InicializarAsync()
    {
        try
        {
            Ocupado = true; Erro = "";
            Preencher(Estados, await _localidades.ObterEstadosAsync());
        }
        catch (Exception ex) { Erro = $"Falha ao carregar estados: {ex.Message}"; }
        finally { Ocupado = false; }
    }

    private async Task CarregarMunicipiosAsync(int estadoId)
    {
        try
        {
            Ocupado = true; Erro = "";
            Municipios.Clear(); _municipio = null; OnChanged(nameof(MunicipioSelecionado));
            Preencher(Municipios, await _localidades.ObterMunicipiosAsync(estadoId));
        }
        catch (Exception ex) { Erro = $"Falha ao carregar municípios: {ex.Message}"; }
        finally { Ocupado = false; }
    }

    private async Task CarregarNomesAsync(int idLocalidade, string rotulo)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        try
        {
            Ocupado = true; Erro = "";
            var r = await _nomes.ObterPorLocalidadeAsync(idLocalidade, ct);
            if (ct.IsCancellationRequested) return;
            Preencher(Homens, r.Homens);
            Preencher(Mulheres, r.Mulheres);
            Preencher(Sobrenomes, r.Sobrenomes);
            Titulo = $"Nomes mais comuns em {rotulo}";
            if (r.Homens.Count + r.Mulheres.Count + r.Sobrenomes.Count == 0)
                Erro = "A API não retornou nomes para esta localidade.";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { Erro = $"Falha ao consultar nomes: {ex.Message}"; }
        finally { if (!ct.IsCancellationRequested) Ocupado = false; }
    }

    private void Limpar()
    {
        Homens.Clear(); Mulheres.Clear(); Sobrenomes.Clear();
        Titulo = "Selecione uma localidade"; Erro = "";
    }

    private static void Preencher<T>(ObservableCollection<T> destino, IEnumerable<T> itens)
    {
        destino.Clear();
        foreach (var i in itens) destino.Add(i);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));
    private bool Set<T>(ref T campo, T valor, [CallerMemberName] string? n = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
        campo = valor; OnChanged(n); return true;
    }
}
