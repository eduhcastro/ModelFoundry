# Implementation Log

## Fase 0 - Pré-produção

- **Concepção de Design**: Elaborado o Game Design Document inicial (*IA-TYCOON-PLAN.md*), definindo as métricas de simulação (Cash, Runway, Compute, Data, Reputation, Hype, etc.) e o loop principal do jogo.
- **Configuração de Jogo**: Criação do projeto na Unity `6000.5.0f1` utilizando o Universal Render Pipeline (URP) para suporte a gráficos 3D diorama otimizados.
- **Versionamento e Infraestrutura**: Inicialização do repositório Git com configuração de Git LFS para gerenciar de forma limpa texturas pesadas, modelos 3D (`.fbx`) e áudios.
- **Estruturação de Pastas**: Organização da árvore de diretórios padrão (`Assets/Scripts`, `Assets/Art`, `Assets/Prefabs`, `Assets/ScriptableObjects`, `Assets/Scenes`).

## Dia 1 - Garage Prototype

- Criada cena `Assets/Scenes/GaragePrototype.unity`.
- Adicionada camera ortografica isometrica.
- Adicionados chao, paredes cortadas, mesa, laptop, cadeira, rack GPU e quadro.
- Criada estrutura base: `Assets/Art`, `Assets/Prefabs`, `Assets/Scripts`, `Assets/ScriptableObjects`.
- Importado `POLYGON Office Pack` em `Assets/PolygonOffice`.
- Cena usa prefabs do pacote: desk, chair, computer setup, server rack.

## Dia 2 - Founder Movement

- Criado `PrototypeEmployeeAgent`: anda por waypoints e anima typing.
- Criado `PrototypeWorkstation`: click na estacao manda trabalhar.
- Cena agora tem founder low-poly, maos animadas, status light e pontos de caminhada.
- Loop visual: andar -> workstation -> typing -> andar.

## Revisao Dia 1/2

- Corrigido material rosa nos prefabs do POLYGON.
- Instancias do desk, chair, computer e server agora usam materiais URP locais.

## Dia 3 - Project Panel

- Criado `PrototypeProjectController`.
- UI mostra `SupportBot v0.1`, cash, botao Start e barra.
- Start inicia projeto, consome dinheiro e preenche progresso.
- Founder vai para typing quando projeto inicia.

## Revisao visual POLYGON

- Removidos modelos visuais gerados por codigo.
- Mapa recriado com prefabs `PolygonOffice`: piso, paredes, porta, desk, cadeira, computador, servidores, whiteboard, sofa, mesa, cafe, plantas.
- Founder agora usa personagem `SM_Chr_Developer_Male_01`.
- UI melhorada com slots `IMG_SLOT_*`.
- Placeholders criados em `Assets/Art/ImageSlots`.

## Design Overhaul — Revisão Completa (Dia 3.5)

### Core Systems criados

- **`GameDesignConstants.cs`** — Design system centralizado com paleta premium dark (purple/teal/gold), cores por departamento, constantes de animação, tipografia e layout.
- **`GameManager.cs`** — Singleton com estado completo do jogo: cash, reputation, quality, team, revenue, burn, runway. Sistema de eventos para UI reativa. Nome da empresa customizável.
- **`TimeController.cs`** — Sistema de tempo do jogo (Jan 2017+). 4 velocidades: Paused, Normal (4s/dia), Fast (2s), Ultra (1s). Eventos de dia/semana/mês.
- **`IsometricCameraController.cs`** — Câmera com WASD pan, scroll zoom, Q/E rotação 90°, edge pan, smooth lerp, bounds.

### UI Components criados

- **`UIAnimations.cs`** — Utilidades de animação: fade, scale, slide, pulse, color lerp, value counter. Easing curves (out cubic, out back).
- **`StylizedButton.cs`** — Botão premium com hover (scale 1.04x + brighten), press (scale 0.96x + darken), disabled (opacity). Variantes: Primary, Secondary, Danger, Accent.
- **`ResourceBar.cs`** — Barra de recurso animada com ícone + label + valor + fill. Auto-format K/M. Health gradient opcional.
- **`ToastNotification.cs`** — Notificações empilháveis com slide-in/fade-out. Categorias: Info, Success, Warning, Danger. Auto-subscribe ao GameManager.
- **`MainMenuController.cs`** — Menu principal com entrada animada (fade + slide), input de nome da empresa, transição de cena com fade-to-black.
- **`HUDController.cs`** — HUD completo: barra superior (empresa, data, velocidade), resource bars (cash/rep/quality/team), info financeira (burn, revenue, runway, clients).
- **`ProjectResultPanel.cs`** — Modal de resultado do projeto com star rating, métricas (quality/cost/clients/revenue/reputation), botões Accept/Refine/Abandon.

### Scripts refatorados

- **`PrototypeProjectController.cs`** — Reescrito: integra GameManager para cash, TimeController para pause, ProjectResultPanel para resultados. Cálculos de quality/clients/revenue com variância e bônus de equipe. Decisões: Accept (lança), Refine (melhora quality), Abandon (perde investimento).
- **`PrototypeEmployeeAgent.cs`** — 5 estados (Idle/Walking/Working/Celebrating/Panicking) com animações únicas. Integra Animator. Status light com color lerp suave. Responde a pause do jogo.
- **`PrototypeWorkstation.cs`** — Transição de cor suave na tela, flicker quando ativo, emission glow, error flash, suporte a múltiplas telas.

## Dia 4 - UI Integration, X Redesign & Scene Building

- **Criação Automatizada de Cenas**: Estruturado o script de editor `GaragePrototypeBootstrap.cs` para gerar de forma limpa e automatizada as cenas `MainMenu` e `GaragePrototype` (Gameplay).
- **Montagem do Canvas & HUD**: Criado Canvas completo com barra superior (nome da empresa, data, controles de velocidade), painel de recursos (com ícones reais 3D assados do pacote de ícones Synty), painel financeiro inferior e suporte a notificações Toast.
- **Redesenho do TechPulseUI ao Estilo X**: Redesenhada a interface do TechPulse para se assemelhar ao aplicativo X (Twitter): fundo preto puro, divisores de posts discretos (#2F3336), botão "Post" azul (#1D9BF0), tipografia limpa em branco e cinza claro, e feed de tweets de rivais IA.
- **Acoplamento de Referências e Fallback**: Conectados todos os campos do tipo `SerializeField` (botões de velocidade, barras de recursos, etc.) via serialização no script de bootstrap. Adicionado um resolvedor dinâmico em runtime no `HUDController.cs` para garantir acoplamento robusto do TechPulseUI.
- **Estabilização de Métricas**: Corrigido bug no `PrototypeProjectController.cs` onde as métricas eram recalculadas ao aceitar o projeto. O resultado (qualidade, custo, clientes, reputação e receita) agora é congelado ao finalizar e aplicado fielmente no GameManager.

## Dia 5 - Recruitment & Employee Mechanics

- **Contratação de ML Engineer**: Adicionado botão de contratação no `SummaryPanel` para recrutar um ML Engineer por $5.000 (aumentando burn em $1.200/mês e equipe em +1).
- **Mecânica de Estação de Trabalho & Pathfinding**:
  - Configurada uma segunda mesa (`Helper Desk` em `X = 1.1f`) no escritório com monitor, cadeira e computador.
  - Ao contratar, o personagem female developer (`ML_Engineer_Preview`) é ativado, inicializado dinamicamente via script com seus pontos de caminhada (café, whiteboard, approach, server) e caminha fisicamente para trabalhar na mesa vazia.
- **Aceleração do Treinamento**: Contratação do ML Engineer concede aumento permanente de 40% na velocidade de treinamento do modelo (reduz a duração em 40%).

## Dia 6 - Save/Load, Event System & TechPulse Integration

- **Sistema de Save/Load Local em JSON**: Criado `SaveLoadManager.cs` que salva o estado completo do jogo (recursos, burn rate, data, contratações e nível de upgrade de GPU) em um arquivo local `save.json` em `Application.persistentDataPath` e reconstrói o estado ao carregar.
- **Linha do Tempo de IA & Eventos Aleatórios**: Implementado `GameEventController.cs` contendo eventos históricos de marcos reais de IA (Transformer em Jun 2017, GPT-2 em Fev 2019, GPT-3 em Mai 2020, ChatGPT em Nov 2022, Agentic Shift em Dez 2025, Summit de IA em Jun 2026) que pausam o jogo e forçam escolhas com impactos na reputação, cash e qualidade.
- **Auto-Postagem no TechPulse**: Lançamentos de modelos de sucesso geram automaticamente publicações no feed social TechPulse anunciando a qualidade técnica do modelo.

## Fase 2 - Compute, Datasets & Model Selection (Dia 7)

- **GPU Upgrades & Cabinet Visuais**: Adicionado botão de upgrade de GPU ($10k upfront, +$300/mo burn) que ativa o segundo rack de servidores (`GPU Rack B`) no escritório e concede 20% de aceleração no treinamento de IA.
- **Seleção de Modelos & Fontes de Dados**:
  - Adicionadas opções de modelo: Vision (rápido/barato), NLP (médio) e Agentic (lento/caro).
  - Escolha do dataset: Web Scraping (grátis, penalidade de qualidade, dobra chance de auditoria/segurança) ou Licenciamento ($1.5k upfront, bônus de qualidade).
  - Interface do painel de projetos expandida de 300f para 440f de altura com botões dinâmicos de seleção e cálculo de custo estimado em tempo real.
- **NUnit Tests**: Implementados testes de unidade em `HiringTests.cs` cobrindo o fluxo de sucesso/falha do upgrade de GPU.
- **Compilação Automatizada**: Reconstruídas as cenas `MainMenu` e `GaragePrototype` via linha de comando em batchmode sem erros.

## Fase 3 - Marketing, Concorrentes e Expansão de Escritório (Dia 8)

- **Correção da Interface do TechPulse**: Resolvido o bug do feed de posts que impedia a visualização das publicações. Adicionado o componente `LayoutElement` ao `postPrefab` para evitar o colapso de altura (125f) sob o `VerticalLayoutGroup` e `ContentSizeFitter`.
- **Mecânica de Seguidores Iniciais**: O jogador inicia o jogo com exatamente **1 seguidor** e **1 seguindo** (sendo salvo/carregado no JSON).
- **Notícias Corporativas e Incidentes de Concorrentes**: O feed agora gera posts autônomos sobre demissões (layoffs), financiamentos milionários (funding) e incidentes de servidores/código de concorrentes rivais.
- **Opiniões Orgânicas e Menções ao Jogador**: Usuários comuns postam marcando a empresa do jogador (`@empresa`) avaliando e fazendo piadas sobre o modelo com base no seu nível de qualidade e reputação de mercado.
- **Cobrança de Inatividade**: Caso o jogador passe mais de 15 dias sem lançamentos, o sistema gera reclamações no feed citando a empresa, com penalidades na reputação e seguidores.
- **Integração de Logotipos Reais**: Implementado o carregamento dinâmico e exibição no avatar do post de logotipos reais a partir da pasta `MyAssets/Rivals/`:
  - `openai.png` $\rightarrow$ NeuraCorp
  - `anthropic.png` $\rightarrow$ AnthroTech
  - `grok.png` $\rightarrow$ Quantum Minds
  - Outros rivais usam a primeira letra do nome estilizada em sua cor de marca como fallback.

## Fase 4 - Expansão de Escritório e Laboratório Secreto (Dia 9)

- **Expansão Física do Escritório (Tiers 2 e 3)**:
  - Implementado o controlador visual `OfficeVisualController.cs` e a interface de upgrade `OfficeUpgradeController.cs` para gerenciar a expansão física do diorama 2x2.
  - Tier 2 (Corporate Suite, $30k) expande o diorama para a esquerda e desativa as paredes internas correspondentes (X: -5f).
  - Tier 3 (Secret R&D Lab, $75k) expande o diorama para trás e desativa as paredes traseiras (Z: 4f).
  - Estações de trabalho adicionais e personagens de pré-visualização são ativados conforme o escritório expande.
- **Novos Cargos e Funcionários Especializados**:
  - Habilitada a contratação de papéis avançados:
    - **Research Scientist** (Cientista de Pesquisa): Concede 50% de redução no tempo de pesquisa de novas tecnologias.
    - **Data Engineer** (Engenheiro de Dados): Concede +5 de qualidade base para todos os novos modelos iniciados.
    - **Safety Researcher** (Pesquisador de Segurança): Reduz em 40% a probabilidade de falhas de segurança e mitiga o impacto de eventos negativos.
  - Personagens criados com animações masculinas/femininas apropriadas configuradas via bootstrap.
- **Sistema de Painéis Glassmorphic e HUD Modernizado**:
  - Substituição da compra imediata nos botões do HUD dock por painéis glassmorphic sobrepostos (Overlay Panels):
    - **HiringPanel** (Contratação de funcionários especializados).
    - **UpgradesPanel** (Expansão de GPU e Escritório).
    - **ResearchPanel** (Pesquisa de tecnologias de R&D).
    - **AnalyticsPanel** (Visualização de métricas e gráficos).
    - **SystemPanel** (Salvar, Carregar e Sair do jogo).
- **Pesquisa Avançada (R&D)**:
  - Implementado o `ResearchController.cs` com duas pesquisas tecnológicas principais:
    - **Safety Alignment** (Alinhamento de Segurança): Reduz as chances de auditoria e vazamento de dados de datasets web scraped.
    - **Custom Silicon** (Silício Customizado): Oferece 30% de velocidade extra permanente no treinamento de modelos.
- **Salvar/Carregar Avançado (JSON)**:
  - Atualizado `SaveLoadManager.cs` para serializar o nível de tier do escritório, funcionários contratados, tecnologias pesquisadas e estados de GPU, mantendo total retrocompatibilidade.
- **Testes Unitários de Expansão**:
  - Adicionados testes a `HiringTests.cs` cobrindo o recálculo do burn rate por cargo contratado, bônus de P&D (Research) e desbloqueios do escritório.
- **Build Sem Erros**:
  - Todas as dependências e amarrações de serialização foram automatizadas no script `GaragePrototypeBootstrap.cs`. As cenas `MainMenu` e `GaragePrototype` foram compiladas com sucesso via linha de comando com código de retorno 0.

## Fase 5 - Gráficos da Empresa e Relatórios de Modelos Lançados (Dia 10)

- **Gráficos de Linha Dinâmicos e Premium**:
  - Implementada a renderização de gráficos de linha dinâmicos no `AnalyticsController.cs`. Os pontos são plotados proporcionalmente ao valor máximo (altura limite de 65f) e conectados por segmentos de imagem (`Image`) esticados e rotacionados no espaço local do container do Canvas.
  - Atualizados os três gráficos no bootstrap `GaragePrototypeBootstrap.cs`:
    - **Cash Over Time** (Faturamento e caixa acumulado mensalmente).
    - **Followers Over Time** (Crescimento de seguidores do jogador no TechPulse).
    - **Model Quality History** (Qualidade técnica dos últimos 8 modelos de IA lançados).
- **Relatório e Painel de Métricas do Analytics**:
  - Painel de estatísticas atualizado para exibir o total de modelos lançados, a qualidade média da empresa e o melhor modelo com seu percentual.
  - Log estruturado "Recent Model Launches" exibe a lista dos modelos recentes com qualidade e contribuição de receita mensal correspondente.
- **Persistência de Histórico (Save/Load)**:
  - O `SaveLoadManager.cs` foi expandido para serializar todas as listas de histórico e o log de lançamentos recentes em formato JSON, mantendo compatibilidade com saves antigos.
- **Testes Unitários**:
  - Criado o teste unitário `AnalyticsHistory_RecordsAndRestoresCorrectly` em `HiringTests.cs` para validar a gravação e restauração dos históricos.
  - Ajustado o teste `GameManager_LoadGameState_RestoresStateCorrectly` para refletir o cálculo dinâmico de burn rate da Fase 4. Todos os 11 testes unitários passam com sucesso.
- **Compilação Sem Erros**:
  - Cenas reconstruídas com sucesso através do editor bootstrap e compilação batchmode finalizada com código de saída 0.

## Fase 6 - Sistema de Contratos Avançados e Eventos Globais de IA (Dia 11)

- **Sistema de Contratos Comerciais Procedimentais**:
  - Implementado o `ContractController.cs` para gerar e gerenciar propostas de contratos comerciais de forma procedimental.
  - Os contratos exigem tipos específicos de modelos (Vision, NLP, Agentic) desbloqueados por tecnologia, metas de qualidade, prazos em dias, e impõem pagamentos adiantados (upfront), bônus de entrega (completion payout), e multas por falha/atraso (punição financeira e perda de reputação).
  - Limite estrito de no máximo 3 contratos ativos simultaneamente.
- **Painel de Contratos Premium (HUD Dock "C")**:
  - Adicionado um novo botão de dock vertical "C" posicionado no HUD dock para abrir o painel de contratos.
  - O painel exibe de forma dinâmica e elegante (com efeito glassmorphism) os contratos ativos com contagem regressiva e os contratos disponíveis com botões de aceitar.
- **Eventos Globais de IA e Modificadores Dinâmicos**:
  - Implementada a ocorrência regular de eventos de mercado global a cada 3-6 meses no `GameEventController.cs`:
    - **GPU Shortage** (Escassez de GPU): Aumenta o custo de upgrades de GPU em 50% por 3 meses, com escolhas de absorver custos ou fazer lobby (perda de reputação).
    - **EU Data Regulation** (Regulações de Dados): Período de auditoria de 3 meses que penaliza lançamentos com dados coletados via web scraping, a menos que um Safety Researcher esteja contratado.
    - **Generative AI Hype Wave** (Onda de Hype): Concede bônus de 2x reputação e 1.5x seguidores para lançamentos por 3 meses, ou uma concessão de pesquisa de $2.000.
  - As resoluções de eventos e ações de contratos (conclusão/quebra) publicam automaticamente atualizações e notas de imprensa no feed **TechPulse**.
- **Testes Unitários Robustos**:
  - Criado o arquivo `ContractTests.cs` (EditMode NUnit) contendo 6 novos testes que validam o fluxo completo de geração baseada em tecnologia, aceitação com pagamento adiantado, limite de slots, entrega com metas de qualidade, multas por atraso e modificadores de escassez de GPU.
  - Implementado reset estático e limpeza de singletons em `SetUp`/`TearDown` de testes e modificada a inicialização do `GameManager.cs` em EditMode (`Application.isPlaying` wrapper no `DontDestroyOnLoad`) para permitir execução confiável e livre de vazamento de estados. Todos os 17 testes de unidade do projeto passam sem falhas.
- **Compilação Sem Erros**:
  - As referências visuais e botões foram acoplados no script de bootstrap. O build em batchmode de todas as cenas completou com sucesso.

## Fase 7 - Infraestrutura Avançada e Datacenters Modulares (Dia 12)

- **Mecânica de Datacenters e Expansão Física (Tier 4)**:
  - Implementado o upgrade de escritório para o **Tier 4 (Modular Datacenter)** por $120.000 cash e burn rate mensal de $5.000.
  - A expansão amplia fisicamente o diorama para o lado direito (coordenadas X: 3f a 4f, Z: -2f a 3f), ativando novos pisos, paredes decoradas com LEDs, racks extras de servidores e desativando as paredes do canto direito (Tier 1) para integrar o ambiente.
- **Gestão Energética e Térmica (Energy & Cooling)**:
  - Implementados cálculos dinâmicos de consumo elétrico e dissipação térmica baseados na contagem física de GPUs ativas.
  - Cada GPU consome e gera 10kW de calor. Se a capacidade de carga da rede ou refrigeração for excedida, o sistema entra em **Superaquecimento Crítico (IsOverheating)**, reduzindo a velocidade de treinamento (dobra a duração de projetos de IA ativos).
  - Compras de upgrades da rede elétrica (+30kW, custo $8k, manutenção +$100/mês) e do sistema de resfriamento (+30kW, custo $6k, manutenção +$80/mês) no painel do NOC.
- **Painel de Controle do NOC (Network Operations Center)**:
  - Substituído o botão lateral redundante "L" (Load) pelo novo botão **"N" (NOC)** no HUD vertical lateral.
  - O painel exibe de forma reativa e glassmorphic o número de GPUs ativas, barras de status dinâmicas de energia e refrigeração, alertas em caso de sobrecarga térmica e ações para comprar upgrades elétricos/térmicos.
- **Novos Cargos e Funcionários Especializados**:
  - **Infrastructure Engineer** (Custo: $10k, salário: $2.8k/mês) - Reduz o consumo energético total de GPUs em 25%.
  - **GPU Technician** (Custo: $7k, salário: $1.9k/mês) - Reduz a dissipação térmica em 10% e acelera o treinamento em 10%.
  - **MLOps Engineer** (Custo: $9k, salário: $2.2k/mês) - Aumenta o faturamento líquido de modelos em +20% devido à otimização de custos de inferência/serviço.
  - **Backend Engineer** (Custo: $6k, salário: $1.5k/mês) - Expande permanentemente a capacidade de contratos comerciais ativos simultâneos de 3 para 4 slots.
  - Integrada a animação e movimentação física dos personagens correspondentes (que caminham até suas respectivas novas mesas de trabalho e digitam ao serem contratados).
- **Persistência de Dados (Save/Load)**:
  - Atualizado o `SaveLoadManager.cs` para serializar e deserializar todos os estados elétricos, contadores de upgrades e estados de contratação dos 4 novos engenheiros especialistas.
- **Testes Unitários de Integração**:
  - Adicionados testes em `HiringTests.cs` cobrindo o impacto financeiro dos salários e manutenções, o estado de superaquecimento e capacidade de rede/cooling, e a ampliação do slot de contratos para 4 no `ContractController`. Todos os 20 testes unitários passam com sucesso.
- **Build Sem Erros**:
  - Todas as dependências e amarrações visuais do diorama e do painel NOC foram conectadas e geradas com sucesso via build em batchmode de todas as cenas.

## Fase 8 - Investimentos, Conselho de Administração e Board Room (Dia 13)

- **Rodadas de Financiamento Venture Capital (Series A, B, C) e IPO**:
  - Implementado sistema de rodadas de financiamento VC no `InvestmentController.cs`. Permite captar investimentos massivos ($150k, $400k, $1.2M) em troca de diluição de equity (participação acionária), desbloqueados por marcos de reputação e seguidores.
  - Implementado processo de IPO (Oferta Pública Inicial) após a Série C, permitindo abrir o capital da empresa na bolsa, vendendo o equity restante e arrecadando recursos públicos gigantescos com base no valuation da empresa.
- **Conselho de Administração (Board Room) e Pressão dos Investidores**:
  - O conselho agora exige metas trimestrais procedimentais (receitas mínimas, crescimento de seguidores no TechPulse e controle estrito do burn rate mensal).
  - Implementada a métrica **Board Trust** (Confiança do Conselho, de 0 a 100). Falhar nas metas trimestrais ou tomar decisões contrárias aos investidores reduz a confiança. Se cair a zero, o jogador enfrenta penalidades severas (Multa por Falta de Confiança) ou demissão do cargo de CEO (Game Over imediato).
- **Fusões e Aquisições (M&A - Mergers & Acquisitions)**:
  - Possibilidade de adquirir concorrentes startups menores (como **Quantum Minds** e **AnthroTech**) diretamente do painel de investimentos por um custo de caixa substancial.
  - A aquisição absorve patentes de pesquisa e inteligência competitiva, concedendo bônus permanentes de +10% e +15% na velocidade de treinamento de modelos de IA.
- **5 Novos Funcionários Especializados**:
  - **Finance Lead** (Líder Financeiro): Melhora a eficiência fiscal, reduzindo custos operacionais gerais em 15% e facilitando captação de recursos com valuations 20% maiores.
  - **Recruiter** (Recrutador): Reduz os custos e tempos de contratação de novos funcionários em 30% e aumenta a habilidade básica de novos contratados.
  - **Product Manager** (Gerente de Produto): Reduz o tempo de desenvolvimento/treinamento de todos os projetos em 10% e adiciona +3 de qualidade base aos modelos.
  - **Sales Executive** (Executivo de Vendas): Aumenta os pagamentos de contratos comerciais enterprise em 20% e adiciona +1 slot de contrato ativo.
  - **Community Manager** (Gerente de Comunidade): Aumenta em 25% a conversão de seguidores no TechPulse e reduz a perda de reputação por inatividade.
  - Animações e movimentações físicas integradas no diorama: cada personagem se desloca até sua respectiva cadeira de trabalho e digita.
- **Diorama e Expansão Visual da Board Room**:
  - Implementação física do diorama correspondente na sala do conselho. Ativação de mesa retangular de reuniões de madeira escura, cadeiras executivas pretas, uma grande TV de tela plana para apresentações de slides na parede e estações de trabalho de apoio.
- **Painel da Board Room e Interface (HUD Dock "B")**:
  - Adicionado o botão de atalho **"B" (Board Room)** no HUD vertical lateral para exibir um painel elegante e glassmorphic.
  - O painel exibe o status de equity da empresa, o nível de confiança do conselho (Board Trust), as metas do trimestre atual com contagem regressiva em dias, as opções de rodadas de investimento VC/IPO e a lista de startups para aquisição.
- **Persistência de Dados Expandida (Save/Load)**:
  - Atualização do `SaveLoadManager.cs` para serializar e deserializar o status das rodadas de investimento (Séries A/B/C/IPO finalizadas), a porcentagem de equity retida, o nível de Board Trust, o progresso das metas trimestrais, as startups adquiridas e as novas contratações da equipe.
- **Testes Unitários**:
  - Adicionados testes de integração robustos em `HiringTests.cs` cobrindo o fluxo de captação de investimentos, validação e aplicação de metas trimestrais de receita/seguidores/burn rate, diluição de equity e bônus das startups adquiridas.
- **Compilação Sem Erros**:
  - Todas as referências visuais, painéis e layouts foram amarrados no editor bootstrap. Compilação bem-sucedida de todas as cenas via batchmode sem erros de compilação.

## Fase 9 - Era AGI e Laboratório de Superinteligência (Planejado)

- **Laboratório de R&D Secreto (Secret R&D Lab)**:
  - Desbloqueio do laboratório de segurança nacional e pesquisa avançada de fronteira para a busca da Inteligência Artificial Geral (AGI).
- **Crises de Alinhamento e Alertas de Segurança (Safety / Alignment)**:
  - Eventos globais de alto risco relacionados a modelos autônomos que saem de controle, alucinações severas em sistemas corporativos críticos e vazamento de modelos confidenciais.
  - Pressão do governo e de órgãos reguladores internacionais exigindo auditorias e o cumprimento de tratados de não-proliferação de IA.
- **Desbloqueio de Salas Avançadas**:
  - **War Room & Centro de Crise**: Salas dedicadas ao controle emergencial de segurança cibernética e de incidentes de reputação/vazamentos.
- **Parcerias de Defesa e Segurança**:
  - Escolhas de fechar contratos exclusivos com agências de segurança nacional ou abrir a tecnologia para consórcios globais abertos.
- **Novos Cargos e Funcionários Especializados**:
  - **AI Safety Researcher** (Pesquisador Avançado de Alinhamento): Mitiga em 60% os riscos de incidentes catastróficos de modelos de alta escala.
  - **Lawyer / Compliance** (Advogado Corporativo): Defende a empresa em processos legais, reduz multas de órgãos reguladores e facilita licenciamentos de dados de alto risco.

## Fase 10 - Steam Demo, Tutorial e Polimento Geral (Planejado)

- **Tutorial e Experiência de Integração**:
  - Criação de um tutorial narrativo integrado na garagem inicial, guiando o jogador passo a passo pela contratação do primeiro funcionário, treinamento de modelos e entrega de contratos.
- **Acessibilidade (Accessibility)**:
  - Suporte a filtros de daltonismo, redimensionamento de texto para legibilidade de UI (TechPulse, modais) e mapeamento dinâmico de controles.
- **Integração com a Steamworks**:
  - Implementação de conquistas (Achievements), salvamento em nuvem (Steam Cloud) e estatísticas globais do jogador.
- **Localização Multilíngue (Localization)**:
  - Preparação do jogo para múltiplos idiomas, com foco inicial em Português (PT-BR) e Inglês (EN-US).
- **Preparação de Lançamento (Marketing & Steam)**:
  - Produção e edição do trailer oficial do jogo, captura de screenshots profissionais do diorama e estruturação da página pública da loja na Steam para a Demo pública.
- **Balanceamento Econômico e Simulação de Mercado**:
  - Ajustes finais na progressão de dificuldade, margens de lucro dos contratos enterprise, custos de infraestrutura e a velocidade com que rivais evoluem nas diferentes eras do mercado de IA.

