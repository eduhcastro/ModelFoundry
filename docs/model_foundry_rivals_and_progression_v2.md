# Model Foundry — Sistema de Concorrentes e Progressão de Mercado
**Versão:** 0.2  
**Status:** base de design para o protótipo Unity  
**Escopo:** campanha, rivais, mercados, TechPulse e regras de simulação

---

## 1. Decisão de produto

*Model Foundry* é um management sim sobre fundar e escalar uma empresa de IA. O jogador deve conseguir vencer por uma estratégia própria — eficiência, produtos verticais, segurança, dados, distribuição, modelos abertos ou pesquisa de fronteira — e não apenas por acumular dinheiro.

Os concorrentes existem para criar:
- contexto de mercado;
- pressão e oportunidades;
- histórias no TechPulse;
- decisões de posicionamento;
- sensação de uma indústria evoluindo.

Eles **não** devem funcionar como uma lista de chefes com força fixa nem como empresas que trapaceiam para sempre superar o jogador.

### Objetivos de design

1. O jogador começa pequeno, mas tem rotas críveis para liderar um nicho ou o mercado.
2. Cada rival tem vantagens, limites e uma identidade legível.
3. A comparação é feita por **mercado e produto**, não por uma “média global” de qualidade.
4. Todo grande movimento de rival gera ao menos uma oportunidade de resposta.
5. O mundo avança no tempo, mas os resultados variam entre campanhas.
6. As referências à indústria real são abstratas; o universo, as marcas e os produtos do jogo são fictícios.

---

## 2. Regra obrigatória de marcas e logos

Os arquivos atuais em `MyAssets/Rivals/` devem ser tratados como **referências privadas de desenvolvimento**, não como assets de lançamento.

Arquivos recebidos:
- `openai.png`
- `anthropic.png`
- `grok.png`
- `haploLogo.png`
- `pixflow.jpg`
- `freepik.jpg`
- `minilist.jpg`
- `company-logo-designed.jpg`

### Política de uso

- Não publicar esses logos, nomes ou identidades visuais de terceiros em build, Steam page, trailer, screenshots ou marketing.
- Não usar nomes que confundam o jogador sobre uma relação, licença ou endosso de empresas reais.
- Antes de uma demo pública, substituir todos por logos originais criados para *Model Foundry* ou licenciados de forma explícita.
- Manter referências externas fora de `Assets/`, por exemplo em `Reference/DoNotShip/`, e ignorá-las no Git. Isso reduz o risco de um asset entrar no build por engano.
- Fazer uma checagem de marca antes de fixar qualquer nome comercial final. Os nomes abaixo são nomes internos de trabalho.

### Mapeamento temporário de referência

| ID | Empresa fictícia exibida no jogo | Arquivo atual de referência | Papel no mercado | Status para lançamento |
|---|---|---:|---|---|
| `RIV_NEURAFORGE` | NeuraForge | `openai.png` | Laboratório de fronteira | Substituir logo |
| `RIV_AEGIS` | Aegis Research | `anthropic.png` | Segurança e enterprise | Substituir logo |
| `RIV_PULSEFRAME` | Pulseframe | `grok.png` | Consumer, dados em tempo real e hype | Substituir logo |
| `RIV_HAPLOWORKS` | HaploWorks | `haploLogo.png` | Automação vertical B2B | Verificar origem/licença; preferir substituir |
| `RIV_PRISMFLOW` | Prismflow | `pixflow.jpg` | Criação, vídeo e multimodalidade | Verificar origem/licença; preferir substituir |
| `RIV_VECTORIA` | Vectoria | `freepik.jpg` | Plataforma de conteúdo visual e dados | Substituir logo |
| `RIV_MINICORE` | MiniCore | `minilist.jpg` | IA pequena, on-device e eficiência | Verificar origem/licença; preferir substituir |
| `RIV_CLOUDHARBOR` | Cloudharbor | `company-logo-designed.jpg` | Nuvem, GPUs e infraestrutura | Verificar origem/licença; preferir substituir |

> **Nota:** o arquivo de referência não determina a identidade final da empresa. Nome, símbolo, paleta e personalidade devem ser próprios.

---

## 3. Estrutura de mercado

O jogo não terá um único placar de “melhor IA”. Cada lançamento pertence a um mercado com critérios diferentes.

### Mercados jogáveis

| Mercado | Exemplos de produtos | Métricas mais importantes |
|---|---|---|
| Assistentes e automação | chatbots, copilots, atendimento | qualidade, custo, confiabilidade, integração |
| Enterprise e operações | OCR, classificação, previsão, workflows | segurança, precisão, suporte, compliance |
| Criatividade e mídia | imagem, vídeo, voz, design | qualidade estética, velocidade, licença, controle |
| Visão e indústria | inspeção, robótica, análise visual | precisão, latência, robustez, hardware |
| Modelos de fundação | LLMs, multimodais, reasoning | capacidade, contexto, custo de inferência, segurança |
| Edge e dispositivos | modelos locais, IA embarcada | eficiência, tamanho, privacidade, bateria |
| Infraestrutura | cloud, GPUs, serving, dados | disponibilidade, preço, capacidade, energia |

### Pontuação de produto por mercado

Cada produto recebe uma nota interna de 0 a 100 por dimensão. A nota comercial deve variar conforme o mercado.

```text
ProductScore =
  Capability
  + Reliability
  + CostEfficiency
  + SafetyAndCompliance
  + DistributionFit
  + UserExperience
```

A ponderação muda conforme o segmento. Um modelo para banco valoriza segurança e confiabilidade; uma ferramenta criativa valoriza qualidade visual e velocidade; um modelo on-device valoriza eficiência.

**Regra:** um lançamento do jogador abaixo da fronteira técnica ainda pode vencer se entregar melhor custo, integração, privacidade, UX, nicho ou distribuição.

---

## 4. Campanha e eras tecnológicas

### Duração padrão

- Início: janeiro de **2016**
- Fim da campanha principal: dezembro de **2032**
- Após o fim: modo sandbox com eventos gerados
- Escala: um mês de jogo é a unidade econômica principal; semanas existem para animações, tarefas e alertas curtos.

A campanha usa anos como referência, mas é uma história alternativa. O jogador pode antecipar certos avanços, atrasá-los ou forçar o mercado a reagir a uma descoberta própria.

### Regras da evolução global

Cada era possui:
- uma data mínima;
- tecnologias habilitadas;
- um `MarketFrontier` esperado;
- eventos de transição;
- novas classes de concorrente;
- novas demandas de clientes.

A data abre possibilidades; ela não entrega poder automático. Para aproveitar uma nova era, a empresa precisa de equipe, pesquisa, dados, compute e caixa.

### Eras

| Era | Período-base | Tema | O que muda para o jogador |
|---|---|---|---|
| 1. IA aplicada | 2016–2017 | automação, classificação, visão básica | primeiros contratos, datasets pequenos, cloud cara |
| 2. Deep learning operacional | 2018–2020 | modelos especializados e pipelines | MLOps, dados proprietários, GPU clusters pequenos |
| 3. Geração e plataformas | 2021–2022 | geração de texto, imagem, voz e APIs | produtos de plataforma, riscos de conteúdo, escala |
| 4. Modelos de fundação | 2023–2025 | LLMs, multimodalidade e open weights | custos altos, benchmarks, contratos grandes, competição global |
| 5. Reasoning e agentes | 2026–2028 | raciocínio, ferramentas e fluxos autônomos | segurança, observabilidade, autonomia e incidentes |
| 6. Ecossistemas autônomos | 2029–2032 | sistemas de múltiplos agentes e infraestrutura própria | datacenter, governança, energia, aquisições e liderança de mercado |

### Transição de era

Para manter a campanha viva, uma era avança quando:

```text
Calendário atingiu a data mínima
E
ocorreu um evento de mercado ou descoberta relevante
E
ao menos uma empresa atingiu a capacidade técnica requerida
```

O jogador pode ser essa empresa. Se ele fizer uma descoberta antes dos rivais, recebe uma janela de liderança e os demais precisam reagir, em vez de receberem imediatamente a mesma tecnologia.

---

## 5. Empresas rivais

### 5.1 NeuraForge — laboratório de fronteira

| Campo | Definição |
|---|---|
| ID | `RIV_NEURAFORGE` |
| Arquétipo | Frontier / agressiva |
| Entrada | 2016 |
| Mercados | modelos de fundação, APIs, agentes |
| Vantagens | pesquisa, captação, lançamento e marca |
| Fraquezas | custo operacional, incidentes públicos, foco excessivo em frontier |
| Curva de força | Tier 2 em 2016 → Tier 4 em 2022 → Tier 5 em 2025 |
| Resposta típica | lançamento rápido, benchmark, corte de preço seletivo, recrutamento |

A NeuraForge é a referência técnica do mercado em momentos específicos, mas não deve dominar todos os segmentos. Ela pode ignorar contratos médios, produtos locais e integrações específicas — espaço natural para o jogador.

### 5.2 Aegis Research — segurança e enterprise

| Campo | Definição |
|---|---|
| ID | `RIV_AEGIS` |
| Arquétipo | cautelosa / research-first |
| Entrada | 2021 |
| Mercados | enterprise, modelos de fundação, segurança |
| Vantagens | confiança, compliance, retenção de clientes |
| Fraquezas | lançamentos mais lentos, custo alto, menor apelo consumer |
| Curva de força | ausente → Tier 3 em 2022 → Tier 5 em 2027 |
| Resposta típica | auditoria, lançamento seguro, parceria corporativa, relatório técnico |

A Aegis é forte quando o mercado sofre incidentes. Ela não deve punir o jogador por ser ousado; ela deve converter medo do mercado em contratos premium.

### 5.3 Pulseframe — dados em tempo real e produto consumer

| Campo | Definição |
|---|---|
| ID | `RIV_PULSEFRAME` |
| Arquétipo | bold / viral |
| Entrada | 2023 |
| Mercados | consumer, busca, assistentes e agentes |
| Vantagens | distribuição, velocidade, hype e dados de comportamento |
| Fraquezas | reputação volátil, privacidade, estabilidade de produto |
| Curva de força | ausente → Tier 3 em 2024 → Tier 4 em 2026 |
| Resposta típica | posts provocativos, lançamento beta, integração inesperada, polêmica |

A Pulseframe deve movimentar o TechPulse, mas não ser “dinheiro infinito”. Em troca de crescimento rápido, ela acumula risco de crise e instabilidade.

### 5.4 HaploWorks — automação vertical

| Campo | Definição |
|---|---|
| ID | `RIV_HAPLOWORKS` |
| Arquétipo | pragmática / B2B |
| Entrada | 2016 |
| Mercados | OCR, atendimento, workflows, previsão |
| Vantagens | contratos recorrentes, implantação, custo disciplinado |
| Fraquezas | pouca inovação de fronteira, produto menos chamativo |
| Curva de força | Tier 2 → Tier 3 |
| Resposta típica | desconto por contrato, expansão regional, parceria de integração |

HaploWorks é o rival inicial mais importante. Ela ensina que produto útil e bem vendido pode vencer tecnologia mais sofisticada.

### 5.5 Prismflow — criação e multimodalidade

| Campo | Definição |
|---|---|
| ID | `RIV_PRISMFLOW` |
| Arquétipo | criativa / trend-driven |
| Entrada | 2018 |
| Mercados | imagem, vídeo, áudio e ferramentas criativas |
| Vantagens | comunidade criativa, lançamentos visuais, viralidade |
| Fraquezas | questões de direitos, moderação e fidelidade de clientes |
| Curva de força | Tier 2 → Tier 4 |
| Resposta típica | desafio criativo, lançamento de feature, campanha comunitária |

### 5.6 Vectoria — conteúdo, dados e distribuição

| Campo | Definição |
|---|---|
| ID | `RIV_VECTORIA` |
| Arquétipo | plataforma / marketplace |
| Entrada | 2017 |
| Mercados | mídia, dados licenciados, ferramentas criativas |
| Vantagens | distribuição, catálogo, parcerias de dados |
| Fraquezas | dependência de creators e disputas de licenciamento |
| Curva de força | Tier 2 → Tier 3 |
| Resposta típica | licenciamento exclusivo, bundle, campanha de parceiros |

Vectoria não precisa criar o melhor modelo; ela pode reduzir a vantagem de um laboratório rival por possuir dados ou distribuição.

### 5.7 MiniCore — eficiência e on-device

| Campo | Definição |
|---|---|
| ID | `RIV_MINICORE` |
| Arquétipo | eficiente / engenharia-first |
| Entrada | 2019 |
| Mercados | edge, dispositivos, inferência barata |
| Vantagens | latência, custo, privacidade e hardware |
| Fraquezas | capacidade de fronteira, dependência de parceiros de device |
| Curva de força | Tier 2 → Tier 4 |
| Resposta típica | compressão, modelo compacto, contrato de hardware |

MiniCore cria uma rota válida contra a corrida por parâmetros: vencer por custo e distribuição local.

### 5.8 Cloudharbor — infraestrutura e energia

| Campo | Definição |
|---|---|
| ID | `RIV_CLOUDHARBOR` |
| Arquétipo | infraestrutura / plataforma |
| Entrada | 2016 |
| Mercados | cloud, compute, serving, energia |
| Vantagens | capital, capacidade, datacenter e contratos |
| Fraquezas | não possui o melhor produto final; exposição a energia e regulação |
| Curva de força | Tier 4 → Tier 5 |
| Resposta típica | mudança de preço de GPU, novo datacenter, parceria exclusiva, capacidade limitada |

A Cloudharbor é mais rica que os laboratórios de modelo em muitos momentos, mas não disputa diretamente a reputação do jogador em cada produto. Ela muda o custo e a disponibilidade de compute para todos.

---

## 6. Força, riqueza e incerteza

### Não usar uma hierarquia financeira fixa

A frase “empresa X é sempre a mais rica” reduz a rejogabilidade e torna a economia previsível. Em vez disso, cada empresa possui faixas de capital e uma estratégia financeira.

| Faixa | Significado |
|---|---|
| Baixa | precisa de receita ou rodada em breve |
| Estável | consegue financiar projetos normais |
| Alta | suporta apostas de médio prazo |
| Estratégica | consegue influenciar mercado, contratar e comprar capacidade |
| Hiperescala | pode construir infraestrutura e absorver choques |

O jogo mostra essa informação por sinais visíveis:
- tamanho dos escritórios;
- anúncios de rodada;
- expansão de datacenter;
- contratação em massa;
- postura em contratos;
- posts no TechPulse;
- relatórios de mercado.

O valor exato de caixa rival fica oculto, exceto quando vazamentos, IPOs ou reportagens o revelam.

### Atributos internos de cada empresa

```text
Capital
Research
Compute
DataAccess
Operations
Distribution
BrandTrust
SafetyMaturity
DomainCapability[mercado]
CurrentStrategy
FinancialRunway
ActiveProjects
RecentIncidents
```

O **Tier** é só um resumo de UI. O resultado real vem da combinação desses atributos.

---

## 7. Motor de decisão dos concorrentes

A cada mês, cada empresa ativa escolhe uma ação estratégica. Uma empresa não pode executar duas ações grandes em sequência sem custo, cooldown ou capacidade disponível.

### Processo mensal

```text
1. Atualizar caixa, receita, custos e projetos em andamento.
2. Medir oportunidades e ameaças por mercado.
3. Identificar a maior prioridade da empresa.
4. Selecionar uma ação permitida.
5. Resolver resultado, impacto de mercado e TechPulse.
6. Aplicar cooldowns e criar pistas para o jogador.
```

### Prioridades possíveis

- proteger participação de mercado;
- responder a um lançamento do jogador;
- recuperar reputação;
- levantar capital;
- reduzir custo;
- entrar em novo mercado;
- adquirir talento ou empresa;
- lançar modelo;
- publicar pesquisa;
- fechar parceria;
- criar padrão aberto;
- sobreviver a crise.

### Ações principais

| Ação | Efeito | Janela para o jogador |
|---|---|---|
| Lançar modelo | aumenta fronteira de um mercado | diferenciar por nicho, preço ou confiança |
| Reduzir preço | pressiona margem | focar em premium, eficiência ou bundle |
| Abrir pesos | derruba barreira técnica | adaptar, especializar ou vender serviço |
| Assinar parceria | reforça distribuição ou dados | buscar outro parceiro ou verticalizar |
| Anunciar benchmark | gera hype e expectativa | desafiar, ignorar ou lançar avaliação própria |
| Contratar equipe-chave | aumenta capacidade futura | contraoferta, contratação alternativa ou automação |
| Investir em datacenter | reduz custo futuro | travar capacidade cloud antes da mudança |
| Sofrer incidente | derruba confiança | oferecer alternativa segura e ganhar contratos |
| Fazer acquihire | fortalece uma especialidade | comprar concorrente menor ou defender talentos |
| Lançar campanha | aumenta demanda no setor | aproveitar tráfego e competir em comunicação |

### Regras de justiça

- Um rival só reage diretamente ao jogador quando ambos atuam no mesmo mercado.
- Ação de resposta possui cooldown; o rival não pode “copiar” toda inovação do jogador no mês seguinte.
- Empresas têm pontos cegos definidos pelo arquétipo.
- Grandes empresas são lentas em mercados pequenos; startups são rápidas, mas frágeis.
- O jogador recebe sinais prévios de movimentos grandes por rumores, vagas, TechPulse, contratos e relatórios.

---

## 8. Fronteira de mercado e reação ao jogador

Cada mercado possui uma `MarketFrontierScore`, calculada pelos melhores produtos ativos daquele setor.

```text
MarketFrontierScore = melhor combinação de capacidade, confiabilidade,
custo, segurança e distribuição no setor.
```

A reputação de um lançamento do jogador depende de:

```text
Reação do mercado =
  adequação ao segmento
  + diferença para a fronteira
  + promessa de marketing
  + estabilidade de lançamento
  + preço
  + reputação prévia
  + cobertura do TechPulse
```

### Exemplo

O jogador lança um chatbot jurídico com capacidade menor que a NeuraForge, mas:
- custo 40% inferior;
- dados jurídicos licenciados;
- compliance alto;
- implantação on-premise;
- suporte excelente.

Resultado: o produto pode ser líder em **enterprise jurídico**, mesmo sem vencer o benchmark geral.

---

## 9. TechPulse

TechPulse é uma rede fictícia de notícias, posts e reações do ecossistema. Seu objetivo é contar histórias e sinalizar mudanças, não substituir a simulação.

### Tipos de publicação

| Fonte | Função |
|---|---|
| Empresa | lançamento, contratação, parceria, resultado e crise |
| Imprensa | contexto, investigação, ranking e análise |
| Cliente | satisfação, reclamação, caso de uso, migração |
| Funcionário | cultura, rumor, burnout, orgulho, vazamento |
| Pesquisador | paper, benchmark, crítica técnica |
| Creator | reação a produto criativo ou trend |
| Investidor | rodada, confiança, cobrança por retorno |
| Regulador | investigação, consulta, multa, exigência |
| Comunidade | memes, elogios, dúvidas e discussões leves |

### Regras de geração

- Uma publicação precisa estar ligada a um evento real do simulador.
- O tom depende de autor, segmento, qualidade, hype e confiança.
- Rumores devem ser claramente marcados como rumor e possuir chance de desmentido.
- Posts de concorrentes devem antecipar consequências: “nova região cloud”, “vaga de research lead”, “teaser de modelo”, “reajuste de API”.
- Posts de usuários devem variar entre reação técnica, reação empresarial e comentário casual. Não devem ser repetitivos nem sempre positivos/negativos.

---

## 10. Progressão de dificuldade

| Modo | Rivalidade | Economia | Recuperação |
|---|---|---|---|
| Startup | rivais menos agressivos, pistas claras | margem mais segura | contratos e investidores mais disponíveis |
| Founder | padrão | padrão | recuperação limitada |
| Frontier | rivais reagem mais cedo e executam melhor | custos altos | decisões ruins cobram mais |
| Simulação | mercado mais volátil e informação incompleta | capital caro | sem proteção contra falência |

A dificuldade não deve dar “bônus invisível de qualidade” aos rivais. Ela deve alterar planejamento, caixa, velocidade de reação, informação disponível e tolerância de mercado.

---

## 11. Escopo de implementação

### Protótipo inicial — obrigatório

Usar apenas quatro empresas ativas:
- HaploWorks;
- Vectoria;
- NeuraForge;
- Cloudharbor.

Mercados ativos:
- automação/atendimento;
- enterprise;
- infraestrutura.

Período jogável do protótipo:
- 2016 a 2020.

Sistemas mínimos:
- calendário mensal;
- projeto do jogador;
- uma métrica por produto;
- orçamento e custo cloud;
- três ações de rival;
- TechPulse básico;
- uma resposta a lançamento;
- save/load.

### Fase 2

Adicionar:
- Prismflow;
- MiniCore;
- geração de imagem/áudio;
- contratação e retenção;
- dados e licenciamento;
- relatórios de mercado;
- competidores dinâmicos menores.

### Fase 3

Adicionar:
- Aegis Research;
- Pulseframe;
- modelos de fundação;
- open weights;
- avaliação, safety e incidentes;
- agentes;
- parcerias e aquisições;
- crises públicas maiores.

---

## 12. Modelo de dados recomendado para Unity

```csharp
public enum CompetitorArchetype
{
    Frontier,
    SafetyEnterprise,
    ConsumerViral,
    VerticalB2B,
    Creative,
    Marketplace,
    EfficientEdge,
    Infrastructure
}

public sealed class CompetitorDefinition
{
    public string id;
    public string displayName;
    public string logoKey;
    public CompetitorArchetype archetype;

    public int startYear;
    public string[] markets;

    public float startingCapital;
    public float startingResearch;
    public float startingCompute;
    public float startingDistribution;
    public float startingTrust;

    public string[] strengths;
    public string[] weaknesses;
}
```

```csharp
public sealed class CompetitorRuntimeState
{
    public float capital;
    public float revenue;
    public float runwayMonths;

    public float research;
    public float compute;
    public float dataAccess;
    public float operations;
    public float distribution;
    public float brandTrust;
    public float safetyMaturity;

    public Dictionary<string, float> marketCapability;
    public List<string> activeProjects;
    public List<string> activeCooldowns;
    public List<string> recentIncidents;
}
```

### Asset pipeline de logo

```text
Assets/
  Art/
    Brand/
      Rivals/
        neuraforge_logo.png
        aegis_logo.png
        pulseframe_logo.png
        haploworks_logo.png
        prismflow_logo.png
        vectoria_logo.png
        minicore_logo.png
        cloudharbor_logo.png

Reference/
  DoNotShip/
    third_party_logo_references/
```

O jogo deve carregar apenas os logos finais, nomeados pelo `logoKey`, a partir de `Assets/Art/Brand/Rivals/`.

---

## 13. Critérios de aprovação do sistema

Antes de considerar o sistema pronto:

- [ ] Nenhum logo ou nome de marca real aparece no build público.
- [ ] Cada rival possui pelo menos duas fraquezas jogáveis.
- [ ] Uma ação de rival sempre cria uma reação possível para o jogador.
- [ ] A comparação de produto é específica ao mercado.
- [ ] O jogador pode liderar pelo menos um segmento antes da Era 4.
- [ ] O TechPulse só publica fatos, rumores ou reações conectados à simulação.
- [ ] O protótipo funciona com quatro rivais antes de expandir para oito.
- [ ] O Tier é informativo; as variáveis reais determinam o resultado.
- [ ] A campanha continua possível mesmo se o jogador não competir em modelos de fronteira.
- [ ] Todo asset de marca e logo possui origem e licença registradas.

---

## 14. Decisões tomadas nesta versão

1. A campanha passa de 2016–2026 para 2016–2032, com sandbox posterior.
2. Concorrentes deixam de ser imitações diretas de empresas reais.
3. Os oito arquivos atuais são referências temporárias; não entram no lançamento.
4. A força rival deixa de ser fixa e passa a depender de atributos, caixa, mercado e decisões.
5. Cloudharbor é infraestrutura; ela afeta todos, mas não compete como laboratório em todo produto.
6. A qualidade deixa de ser global e passa a ser avaliada por mercado.
7. TechPulse passa a comunicar eventos reais, rumores e reações, em vez de gerar posts aleatórios.
8. O MVP começa com quatro empresas e três mercados para manter o escopo controlado.
