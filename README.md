# FAA - FHIR Artifact Analyzer

## Descrição
O FHIR Artifact Analyzer é uma ferramenta para identificar, validar e facilitar a consulta acerca de artefatos do padrão FHIR (versão 4.0.1). Estes podem estar disponibilizados em uma entrada definida por um conjunto formado por guias de implementação (.tgz), diretórios e arquivos.

## Estrutura de organização no Github
- Main Branch: Branch principal, contém a versão estável da aplicação.
- Branch de desenvolvimento: Branch utilizada para o intermédio da aplicação.
- Branch de tarefas: Branch utilizada para o desenvolvimento de tarefas.
- Branch de correção: Branch utiliada para corregir erros na aplicação.

```mermaid
graph TD
    A[Branch de tarefas] -->|Após a finalização da tarefa| B[Branch de dev] -->|Análise da tarefa|
C[Branch Main] --> |Aplicação estável|
```
```mermaid
graph TD
   A[Branch de correção] -->|Após a finalização da correção| B[Branch de dev] -->|Análise da tarefa|
C[Branch Main] --> |Aplicação estável|

```

## Tecnologias utilizadas 

### Backend
- [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/tour-of-csharp/)
- [.NET Core](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
- [Entity Framework](https://learn.microsoft.com/pt-br/ef/core/)
- [HL7](https://www.nuget.org/packages/hl7.fhir.r4)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
- [CsvHelper](https://joshclose.github.io/CsvHelper/)

### Database
- [MySQL](https://www.mysql.com/)

### Frontend
- [Blazor](https://dotnet.microsoft.com/pt-br/apps/aspnet/web-apps/blazor)
- [JSInterop](https://learn.microsoft.com/pt-br/aspnet/core/blazor/javascript-interoperability/?view=aspnetcore-9.0)
- [D3.js](https://d3js.org/)

### Testes
- TDD

### Outras Tecnologias

| **Funcionalidade**       | **Descrição** | **Tecnologias** |
|-------------------------|--------------|----------------|
| **Entrada de Dados**    | Receber `.tgz`, `.json`, arquivos individuais e URLs | `HttpGet` (URLs), `API FormData` (Upload de arquivos `.zip`, `.tgz`, diretórios, arquivos individuais) |
| **Validação**          | Verificar URLs canônicas e conformidade com FHIR 4.0.1 | `Hl7.Fhir.R4` (ou similar), `System.Text.Json`, `System.Threading.Tasks` |
| **Organização**        | Indexação para busca eficiente e geração de grafos | Estrutura de dados para cache e indexação |
| **Estatísticas**       | Geração de dados estatísticos | |
| **Visualização**       | Interface web e linha de comando | `Blazor` (Web), `CLI Client` (Linha de comando) |
| **Exportação**         | Exportação dos resultados | JSON, CSV, PNG (grafo) |




## Requisitos funcionais
- Permitir busca por sequência de caractere/nome - em elementos para filtragem, como: nome artistas, url canônica, comentário, segurança...
- Permitir filtragem por tipo de artefato
- Permitir filtragem por status de validação
- Permitir filtragem por referências
- Permitir exibir relações de acordo com o argumento selecionado e o gráfico
- Permitir interação com o gráfico por meio de Zoom, pan, seleção de nó e conexão
- Permitir interação pelo uso de filtros (?)
- Permitir a geração dos metadados em formato JSON
- Permitir geração dos dados em tabela CSV
- Permitir geração de gráfico visula PNG

## Requisitos Não Funcionais
- Suportar até 10.000 artefatos (~50MB de dados).
- Processamento síncrono em tempo real para indexação e consulta.
- Garantir a integridade dos dados armazenados e processados.
- Usabilidade
- Interface intuitiva com feedback visual e elementos interativos.
- Permitir interação fluida com o grafo.
- Facilidade de busca e filtragem por diversos atributos.
- Exportação
- Geração de metadados em formato JSON.
- Exportação de tabelas em CSV.
- Exportação de grafos em PNG.
- Interface de linha de comando compatível com sistemas Linux, Windows e macOS.

## Arquitetura da aplicação - Clean Architecture
- A Clean Architecture é uma abordagem de design de software que prioriza a organização, manutenibilidade e testabilidade, enfatizando a separação das responsabilidades e o isolamento do domínio, alem de incorporar muitos dos princípios SOLID em sua estrutura e design. 

