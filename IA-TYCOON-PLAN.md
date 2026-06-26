## Direção visual: o que devemos absorver de *Game Dev Tycoon*

A imagem que você mandou representa muito bem a referência certa de “jogo de gestão divertido”, mas há uma distinção importante:

* O visual de *Game Dev Tycoon* é mais **2.5D isométrico / diorama**, não um 3D realista.
* As salas parecem um escritório em miniatura, aberto para o jogador observar.
* Os personagens são simples, mas têm animações suficientes para dar vida: caminhar, digitar, discutir no quadro, gravar vídeo, testar algo, comemorar ou ficar sobrecarregados.
* A maior parte da legibilidade vem de **cores fortes, silhuetas claras, objetos grandes e UI muito direta**.
* O jogo não depende de gráficos complexos. Ele vende a fantasia por meio de progresso visível: sair da garagem, ganhar salas, contratar pessoas, abrir R&D, laboratório secreto, estúdio de vídeo etc.

A página oficial confirma que a proposta central é justamente começar em uma garagem, crescer para escritórios maiores, contratar equipe, pesquisar tecnologias, criar projetos especiais e desbloquear laboratórios. Ela também é classificada como management, simulation, strategy, economy e isometric/2D. ([Loja Steam][1])

Para o nosso projeto, eu não copiaria arte, interface, layout, nomes, personagens ou assets deles. Mas devemos usar a mesma **linguagem de produto**:

> escritório em corte isométrico + personagens animados + salas especializadas + menus de gestão claros + progresso muito visível.

A diferença seria que o jogador não cria jogos. Ele funda e escala um laboratório de IA.

---

# Conceito do nosso jogo

## Nome provisório

**Model Foundry**
Alternativas:

* **Frontier Labs**
* **Neural Empire**
* **Compute & Co.**
* **Inference Inc.**
* **Singularity Labs**
* **Model Tycoon**
* **The AI Lab**

Eu evitaria “AI Tycoon” porque já existem jogos usando esse nome ou variações muito próximas.

## Fantasia principal

O jogador começa em 2016 ou 2017 com uma pequena startup de tecnologia e uma equipe minúscula.

No início, ele faz coisas simples:

* chatbot para atendimento;
* classificação de imagens;
* ferramenta de OCR;
* recomendador;
* geração de texto simples;
* modelos de voz;
* automação para empresas.

Com o crescimento, ele passa por fases equivalentes à evolução real da indústria:

1. **Startup de automação**
2. **Empresa de machine learning**
3. **Laboratório de deep learning**
4. **Empresa de APIs de IA**
5. **Criadora de foundation models**
6. **Operadora de datacenters**
7. **Laboratório de AGI / superinteligência**

O objetivo não seria “clicar até ter mais dinheiro”, mas tomar decisões que criam uma identidade para a empresa:

* ser uma empresa fechada e enterprise;
* competir em modelos abertos;
* focar em custo e eficiência;
* ser a líder em multimodalidade;
* construir uma IA muito segura;
* correr riscos para lançar primeiro;
* vender infraestrutura;
* virar uma empresa de pesquisa;
* vender produtos próprios para consumidores.

---

# Estrutura de gameplay

## Loop principal

O ciclo deve ser curto, visível e viciante:

1. Você recebe um problema ou oportunidade.
2. Escolhe um projeto de IA.
3. Monta a equipe.
4. Decide o orçamento.
5. Escolhe dados, infraestrutura e estratégia.
6. A equipe trabalha visualmente nas salas.
7. O treinamento termina.
8. O modelo recebe métricas.
9. Você lança, vende, melhora ou abandona.
10. A empresa cresce e desbloqueia novas decisões.

Exemplo:

> Uma empresa pede um modelo de atendimento para bancos.

O jogador pode:

* usar um modelo menor e barato;
* comprar dados licenciados;
* usar dados públicos e correr risco legal;
* contratar especialista em segurança;
* priorizar latência;
* priorizar qualidade;
* vender como API;
* instalar on-premise;
* abrir os pesos;
* lançar rapidamente antes de um concorrente.

Cada escolha altera métricas reais do jogo.

---

# Métricas centrais

O coração do game precisa ser simples de entender, mas profundo quando o jogador quiser otimizar.

| Métrica             | O que representa                              |
| ------------------- | --------------------------------------------- |
| **Cash**            | Dinheiro disponível                           |
| **Runway**          | Quantos meses a empresa sobrevive sem receita |
| **Compute**         | Capacidade de treinamento/inferência          |
| **Data**            | Volume e qualidade dos dados                  |
| **Talent**          | Qualidade da equipe                           |
| **Research**        | Capacidade de inovar                          |
| **Model Quality**   | Resultado técnico do modelo                   |
| **Inference Cost**  | Quanto custa servir usuários                  |
| **Latency**         | Velocidade da resposta                        |
| **Safety**          | Risco de comportamento problemático           |
| **Trust**           | Confiança de empresas e usuários              |
| **Hype**            | Atenção pública e imprensa                    |
| **Energy**          | Custo energético/datacenter                   |
| **Technical Debt**  | Sistemas frágeis e custo de manutenção        |
| **Regulatory Risk** | Risco jurídico e governamental                |

O jogador não deve ver vinte números logo de início. No começo aparecem apenas:

* dinheiro;
* reputação;
* qualidade;
* custo;
* equipe.

As métricas mais técnicas entram conforme a empresa evolui.

---

# Salas do escritório

A graça visual virá de ver a empresa mudar fisicamente.

## Fase inicial: garagem

* uma mesa;
* notebook;
* quadro branco;
* servidor barulhento;
* café;
* fundador trabalhando sozinho;
* primeiros freelancers.

## Fase startup

* sala de engenharia;
* pequena sala de reuniões;
* sala de dados;
* rack de GPU;
* espaço de suporte;
* estúdio de demo.

## Fase laboratório

* R&D Lab;
* sala de avaliação;
* sala de segurança/alignment;
* sala de dados/licenciamento;
* sala de vendas enterprise;
* sala de relações públicas;
* NOC / operations center;
* datacenter próprio.

## Fase hyperscale

* prédio corporativo;
* datacenter modular;
* GPU cluster;
* centro de crise;
* war room;
* laboratório secreto;
* sala do conselho;
* escritório internacional;
* campus de pesquisa.

Cada sala desbloqueia sistemas novos, não apenas bônus numérico.

Exemplo:

| Sala           | Sistema desbloqueado                          |
| -------------- | --------------------------------------------- |
| Data Lab       | Limpeza, curadoria e compra de datasets       |
| R&D Lab        | Novas arquiteturas e avanços científicos      |
| GPU Cluster    | Treino de modelos maiores                     |
| Evaluation Lab | Benchmarks, red teaming e qualidade           |
| Safety Lab     | Alignment, filtros e redução de incidentes    |
| Sales Floor    | Contratos enterprise                          |
| PR Room        | Gestão de hype, imprensa e crises             |
| Data Center    | Infraestrutura própria e inferência escalável |
| Board Room     | Investidores, IPO, fusões e aquisição         |

---

# Personagens e animações

Os bonecos precisam ser simples, estilizados e baratos de produzir.

Não devemos mirar em personagens realistas ou rigs complexos. A melhor direção é:

* corpo baixo-poly;
* cabeça levemente grande;
* roupas e acessórios por profissão;
* poucos detalhes faciais;
* animações curtas em loop;
* leitura clara mesmo com câmera distante.

## Classes de funcionários

* Founder / CEO
* Machine Learning Engineer
* Research Scientist
* Data Engineer
* MLOps Engineer
* Backend Engineer
* Product Manager
* Security Researcher
* AI Safety Researcher
* Sales Executive
* Lawyer / Compliance
* Community Manager
* Infrastructure Engineer
* GPU Technician
* Recruiter
* Finance Lead

## Animações úteis

* caminhar;
* sentar e digitar;
* trabalhar em quadro branco;
* apontar para gráfico;
* conversar em reunião;
* carregar servidor/equipamento;
* operar rack;
* comemorar lançamento;
* entrar em pânico durante incidente;
* dormir na mesa;
* tomar café;
* testar demo;
* apresentar para investidores.

Essas animações fazem o jogo parecer vivo mesmo quando o sistema por trás é relativamente simples.

---

# Estilo artístico recomendado

## Não usar “realismo 3D”

Para esse projeto, realismo seria caro, lento de produzir e pouco necessário.

A direção ideal é:

* câmera isométrica ortográfica;
* salas em miniatura;
* paredes cortadas;
* objetos grandes e reconhecíveis;
* materiais simples;
* iluminação suave;
* sombras discretas;
* cores fortes por departamento;
* personagens low-poly cartunizados;
* UI clara e quase “board game”.

Referência de sensação:

> Uma mistura de *Game Dev Tycoon*, *Two Point Hospital*, *Startup Company* e um escritório futurista de IA.

## Identidade por departamento

| Área           | Direção visual                                 |
| -------------- | ---------------------------------------------- |
| Engenharia     | azul, monitores, cabos, dashboards             |
| Pesquisa       | roxo, quadros, fórmulas, papers                |
| Dados          | amarelo, storage, pipelines, telas de ingestão |
| Segurança      | vermelho escuro, alertas, red team             |
| Vendas         | verde, apresentações, contratos                |
| Infraestrutura | cinza e azul, racks, ventilação, cabos         |
| PR             | laranja, câmeras, imprensa, redes sociais      |
| Conselho       | preto, madeira, telas de métricas              |

---

# Engine recomendada

## Recomendação principal: Unity 6.3 LTS + C#

Eu usaria:

* **Engine:** Unity 6.3 LTS
* **Linguagem:** C#
* **Renderização:** URP
* **UI:** UI Toolkit
* **Modelagem:** Blender
* **Animação:** Unity Animator
* **Controle de versão:** Git + Git LFS
* **Distribuição inicial:** Steam para Windows
* **Áudio:** FMOD ou Unity Audio, inicialmente Unity Audio
* **Save:** JSON local; Steam Cloud depois

Unity 6.3 LTS tem suporte de longo prazo até dezembro de 2027, e a própria Unity recomenda LTS para projetos que vão entrar em produção e precisam de estabilidade. ([Unity][2])

Para esse jogo, Unity faz sentido porque:

* o projeto é essencialmente um jogo de PC para Steam;
* temos um ambiente 3D leve, mas com muitos objetos e UI;
* C# é bom para sistemas de simulação, economia e dados;
* existe muito material, plugins e assets para escritório, low-poly, animação e UI;
* é mais fácil contratar ou encontrar gente que conheça Unity/C#;
* Steam e Unity têm integração madura;
* a engine aguenta esse estilo de jogo com muita folga.

O UI Toolkit também serve para construir UI de runtime, com arquivos de estrutura e estilo próprios; isso é bom para os muitos menus, cards, tooltips, painéis e dashboards que esse jogo exigirá. ([Unity Docs][3])

## Por que não Unreal Engine?

Unreal seria excesso para esse projeto.

Ele é excelente para:

* gráficos realistas;
* FPS;
* mundos grandes;
* cinematics;
* personagens de alta fidelidade.

Mas nosso jogo precisa de:

* produtividade;
* UI densa;
* economia;
* simulação;
* câmera isométrica;
* arte estilizada;
* build leve;
* tempo de produção curto.

Unity é mais adequada.

## E Godot?

Godot seria uma segunda boa opção, especialmente se o objetivo for evitar dependência de licença e manter tudo open source.

Mas eu só escolheria Godot se uma destas condições fosse prioridade:

* orçamento extremamente baixo;
* projeto menor;
* equipe já confortável com Godot;
* desejo de código e pipeline totalmente open source;
* intenção de usar uma estética muito simples.

Para uma produção comercial de Steam com UI grande, assets comprados, animações, vários sistemas e possibilidade de contratar freelancers, eu iria de Unity.

---

# Arquitetura técnica

A regra principal é: **o jogo deve ser data-driven**.

Não podemos hardcodar modelos, funcionários, eventos, tecnologias e empresas em scripts espalhados.

## Estrutura sugerida

```text
Assets/
  Art/
    Characters/
    Environments/
    Props/
    UI/
    VFX/
  Audio/
  Prefabs/
    Rooms/
    Employees/
    Props/
    UI/
  Scripts/
    Core/
    Simulation/
    Economy/
    Employees/
    Projects/
    Models/
    Research/
    Events/
    UI/
    Save/
  ScriptableObjects/
    Employees/
    Rooms/
    Technologies/
    Projects/
    Events/
    Models/
    Competitors/
  Scenes/
    MainMenu/
    Garage/
    Office_01/
    Office_02/
    Campus/
```

## Sistemas principais

```text
GameManager
 ├── TimeSystem
 ├── EconomySystem
 ├── EmployeeSystem
 ├── ProjectSystem
 ├── ModelTrainingSystem
 ├── ResearchSystem
 ├── DataSystem
 ├── ComputeSystem
 ├── EventSystem
 ├── CompetitorSystem
 ├── ReputationSystem
 ├── SaveSystem
 └── UIStateSystem
```

## Dados configuráveis

Cada funcionário, projeto, tecnologia e modelo deve existir como um asset configurável.

Exemplo conceitual:

```csharp
public class ModelDefinition
{
    public string id;
    public string displayName;

    public ModelFamily family;
    public int parameterTier;

    public float requiredCompute;
    public float requiredDataQuality;
    public float trainingDurationDays;

    public float baseCapability;
    public float baseInferenceCost;
    public float baseLatency;

    public float safetyRisk;
    public float hypePotential;
}
```

Assim conseguimos criar conteúdo novo sem reescrever sistemas.

---

# Como adaptar a lógica de Game Dev Tycoon para IA

| Game Dev Tycoon         | Nosso jogo                              |
| ----------------------- | --------------------------------------- |
| Tema do jogo            | Área de aplicação da IA                 |
| Gênero                  | Tipo de modelo                          |
| Plataforma              | Canal de distribuição                   |
| Engine                  | Stack de treinamento                    |
| Gameplay/Story/Graphics | Qualidade, custo, velocidade, segurança |
| Bugs                    | Alucinações, falhas, incidentes         |
| Fans                    | Usuários, devs, clientes, comunidade    |
| Research                | Papers, arquiteturas, hardware, tooling |
| Especial projects       | Foundation models, datacenter, AGI lab  |
| Game report             | Benchmark report e postmortem técnico   |
| Publisher               | VC, cliente enterprise, parceiro cloud  |
| Competitor              | Laboratório rival                       |

Exemplo de projeto:

```text
Produto:
Assistente jurídico para empresas

Modelo:
LLM especializado

Mercado:
B2B Enterprise

Estratégia:
Alta precisão + privacidade

Dados:
Base jurídica licenciada

Deploy:
On-premise

Risco:
Custo alto, mercado lento, alta exigência legal
```

---

# Primeira versão jogável: MVP

Não devemos começar tentando fazer “OpenAI Tycoon completo”.

A primeira versão precisa provar que o jogo é divertido em 20 a 30 minutos.

## Escopo do MVP

### Cenário

Uma garagem e uma empresa com:

* 1 fundador;
* 1 desenvolvedor;
* 1 pequena GPU;
* 2 tipos de clientes;
* 3 projetos;
* 4 métricas;
* 1 concorrente;
* 1 sistema de upgrade de escritório.

### Projetos iniciais

1. Classificador de e-mails
2. Chatbot de atendimento
3. Detector de imagem

### Decisões por projeto

* orçamento;
* qualidade versus velocidade;
* comprar ou não dados melhores;
* contratar freelancer;
* usar cloud ou máquina própria;
* lançar rápido ou testar mais.

### Métricas visíveis

* dinheiro;
* reputação;
* qualidade;
* custo mensal;
* usuários/clientes.

### Salas do MVP

* garagem;
* mini escritório;
* pequeno R&D Lab.

### Eventos do MVP

* cliente reclama de respostas ruins;
* servidor cai;
* concorrente lança produto parecido;
* investidor pede reunião;
* empresa oferece contrato grande;
* funcionário pede aumento;
* vazamento de dados;
* benchmark ruim;
* viralização inesperada.

---

# Ordem de desenvolvimento

## Fase 0 — Pré-produção, 2 semanas

Objetivo: validar o conceito antes de produzir arte.

Entregáveis:

* game design document curto;
* lista de métricas;
* mapa de progressão;
* wireframe dos menus;
* protótipo de loop de projeto;
* documento visual;
* escolha de nome provisório;
* repositório Git;
* Unity configurada.

## Fase 1 — Vertical slice, 4 a 6 semanas

Objetivo: uma pequena parte do jogo já com aparência final.

Entregáveis:

* uma sala de garagem;
* câmera isométrica;
* um personagem animado;
* uma estação de trabalho;
* um projeto de IA;
* barra de progresso;
* resultado com métricas;
* dinheiro;
* contratação de um funcionário;
* uma UI bonita;
* save/load simples.

No fim dessa fase, já dá para gravar um vídeo de Steam e mostrar a ideia.

## Fase 2 — Core loop, 6 a 10 semanas

Entregáveis:

* projetos com diferentes categorias;
* times e atributos;
* pesquisa;
* modelos;
* dados;
* compute;
* eventos;
* concorrentes;
* reputação;
* expansão de escritório;
* economia mais equilibrada.

## Fase 3 — Conteúdo e profundidade, 8 a 12 semanas

Entregáveis:

* mais salas;
* mais funcionários;
* mais modelos;
* mais eventos;
* árvore tecnológica;
* contratos enterprise;
* crises de reputação;
* safety/alignment;
* investimento;
* aquisição de concorrentes;
* datacenter.

## Fase 4 — Steam demo e polimento

Entregáveis:

* tutorial;
* achievements;
* balanceamento;
* acessibilidade;
* traduções;
* Steam Cloud;
* trailer;
* screenshots;
* página da Steam;
* demo jogável.

A Steam oferece funcionalidades como achievements, stats e cloud saves; o SDK é necessário para enviar builds para a plataforma, enquanto os demais recursos são opcionais. ([Steamworks][4])

---

# Primeiro sprint prático

Eu começaria exatamente assim.

## Dia 1

* criar projeto Unity 6.3 LTS;
* configurar URP;
* criar repositório Git;
* configurar Git LFS para `.fbx`, `.blend`, `.psd`, `.wav`, `.mp4`;
* criar cena `GaragePrototype`;
* adicionar câmera ortográfica isométrica;
* adicionar chão, paredes cortadas e uma mesa.

## Dia 2

* criar personagem low-poly temporário;
* implementar caminhada entre pontos;
* implementar animação idle e typing;
* criar estação de trabalho interativa.

## Dia 3

* criar painel de projeto;
* criar um projeto simples: “SupportBot v0.1”;
* iniciar projeto;
* mostrar uma barra de progresso;
* consumir dinheiro ao longo do tempo.

## Dia 4

* ao terminar, gerar resultado:

  * qualidade;
  * custo;
  * clientes;
  * reputação;
  * receita mensal.

## Dia 5

* criar contratação de um ML Engineer;
* aumentar velocidade de trabalho;
* mostrar o funcionário caminhando até uma mesa livre.

## Dia 6 e 7

* adicionar primeiro evento aleatório;
* criar save/load;
* gravar um vídeo curto;
* testar se o loop já é divertido sem arte final.

---

# Decisão objetiva

Eu seguiria com este stack:

```text
Engine: Unity 6.3 LTS
Linguagem: C#
Render: URP
UI: UI Toolkit
3D: Blender
Animação: Mixamo temporariamente + animações próprias depois
Versionamento: Git + Git LFS
Plataforma inicial: Windows / Steam
Save: JSON local
Steam posteriormente: Steamworks + Steam Cloud + Achievements
```

E o primeiro objetivo não seria “fazer o jogo inteiro”.

Seria criar em até seis semanas uma demo com:

> garagem + personagem trabalhando + um modelo sendo treinado + resultado de negócio + contratação + expansão para uma sala de R&D.

Quando essa versão estiver divertida, aí faz sentido investir pesado em assets próprios, modelagem de salas, eventos, árvore tecnológica e conteúdo de longo prazo.

[1]: https://store.steampowered.com/app/239820/Game_Dev_Tycoon/ "Game Dev Tycoon on Steam"
[2]: https://unity.com/releases/unity-6/support?utm_source=chatgpt.com "Unity 6 Releases & Support: LTS & Updates Releases"
[3]: https://docs.unity3d.com/6000.2/Documentation/Manual/UIElements.html?utm_source=chatgpt.com "UI Toolkit"
[4]: https://partner.steamgames.com/doc/sdk?utm_source=chatgpt.com "Steamworks SDK"
