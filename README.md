# IBGE Nomes – Nomes mais comuns por localidade (Censo 2022)

Aplicativo .NET MAUI que consome **duas APIs públicas do IBGE em cadeia**:

1. **Localidades** – popula os seletores (Picker) de Estado e Município.
2. **Nomes (Censo 2022)** – ao selecionar a localidade, consulta
   `https://servicodados.ibge.gov.br/api/v3/nomes/2022/localidade/{id}` e exibe os nomes mais
   frequentes de **homens**, **mulheres** e os **sobrenomes**.

## Integrantes

-DIOGO FERNANDES SOUZA
-JUSSARA FARIA DE OLIVEIRA
## Arquitetura em camadas

| Camada | Pasta | Responsabilidade |
|---|---|---|
| Apresentação | `Views/`, `ViewModels/` | `MainPage.xaml` (Picker, CollectionView) e `MainViewModel` (estado da tela, encadeamento das consultas) |
| Serviços / Integração | `Services/` | `IbgeLocalidadesService` e `IbgeNomesService` (HttpClient) + `NomesJsonParser` (JSON → modelos) |
| Modelos | `Models/` | `Estado`, `Municipio`, `NomeFrequencia`, `ResultadoNomes` |

Fluxo: `Picker Estado` → `GET /v1/localidades/estados` → (se tipo = Município)
`GET /v1/localidades/estados/{id}/municipios` → ao escolher, `GET /v3/nomes/2022/localidade/{id}`
→ listas de Homens / Mulheres / Sobrenomes.

## Como executar

Requisitos: .NET 8 SDK, workload MAUI (`dotnet workload install maui`) e Android SDK/emulador
(ou Visual Studio 2022 com a carga de trabalho "Desenvolvimento para dispositivos móveis com .NET").

```bash
git clone <(https://github.com/diogosouza147/-IBGE-Data-Insights.git)>
cd IbgeNomes
dotnet build -t:Run -c Release
```

No Visual Studio: abra a pasta, escolha o emulador Android e execute (F5).
Só a pasta `Platforms/Android` está incluída; para iOS/Windows/Mac, gere as pastas de plataforma
com `dotnet new maui` e copie-as para o projeto.

## Uso

1. Escolha o **tipo** (Estado ou Município).
2. Escolha o estado (e o município, se for o caso).
3. As listas de nomes e sobrenomes são carregadas automaticamente.

## Observação

O formato do JSON de Nomes é lido de forma tolerante em `Services/NomesJsonParser.cs`. Se o IBGE
mudar a estrutura, só esse arquivo precisa de ajuste (o JSON bruto é logado em Debug).

##Vídeo de demonstração

https://drive.google.com/file/d/1IVBELl4utSebkqgt5xfzh7JSJzF1FriJ/view?usp=sharing
