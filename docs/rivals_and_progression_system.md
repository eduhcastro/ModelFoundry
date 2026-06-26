# Planejamento Técnico: Concorrentes e Progressão Temporal

Este documento detalha o sistema de empresas adversárias de *Model Foundry*, o mapeamento dos logotipos personalizados inseridos em `MyAssets/Rivals/`, o nível de riqueza e a progressão temporal de cada rival ao longo da simulação de mercado.

---

## 1. Mapeamento dos Logotipos e Concorrentes "Premium"

Na pasta `MyAssets/Rivals/`, temos três arquivos de imagem:
* `openai.png`
* `anthropic.png`
* `grok.png`

Eles correspondem diretamente aos três principais concorrentes históricos da indústria, representados no jogo pelas seguintes empresas configuradas no **`CompetitorManager`**:

| Logotipo | Concorrente no Jogo | Personalidade | Nível de Força (1-5) | Foco Principal |
| :--- | :--- | :--- | :--- | :--- |
| **`openai.png`** | **NeuraCorp** | *Aggressive* | Tier 5 (Líder) | Lançamentos rápidos, frontier models (GPT-like), capital massivo de VCs. |
| **`anthropic.png`** | **AnthroTech** | *Cautious* | Tier 4 (Crescendo para 5) | Segurança (Alignment), modelos focados em empresas, posts éticos. |
| **`grok.png`** | **Quantum Minds** | *Bold* | Tier 4 (Finanças ilimitadas) | Open Weights (inicialmente), humor irônico nas postagens, integrados a montadoras. |

---

## 2. Quem é a Empresa Mais Rica? (Hierarquia Financeira)

1. **TitanCloud** (Infraestrutura): É o provedor de computação em nuvem do jogo. Embora não dispute diretamente os modelos com o jogador, é a empresa mais valiosa do ecossistema, pois aluga GPUs para todas as outras.
2. **NeuraCorp** (`openai.png`): A mais rica entre as criadoras de modelos. Começou com forte apoio de VCs em 2015/2016 e lidera o market share global.
3. **Quantum Minds** (`grok.png`): Possui o maior potencial de fôlego financeiro tardio, pois é financiada por um conglomerado de satélites e automóveis elétricos.
4. **AnthroTech** (`anthropic.png`): Cresce substancialmente captando fundos de grandes corporações de e-commerce e nuvem que desejam alternativas à NeuraCorp.

---

## 3. Linha do Tempo e Linha de Progressão (Timeline)

O jogo progride de **2017 a 2026** (ano atual da simulação). Os concorrentes surgem, ganham tração ou entram em declínio seguindo eras tecnológicas:

### Era 1: Startup de Garagem (2017 - 2018)
O jogador compete com concorrentes locais de tecnologia tradicional:
* **HorizonLabs** (Tier 2 - Consumer): Foca em aplicativos móveis simples de classificação e automações básicas. É o concorrente mais fraco e serve para o jogador testar seus primeiros lançamentos.
* **AtlasAI** (Tier 2 - Creative): Pequeno estúdio que tenta vender ferramentas básicas de tradução e design assistido.

### Era 2: Deep Learning e Consolidação (2019 - 2021)
O mercado começa a exigir modelos maiores (Transformers iniciais):
* **PrimordialAI** (Tier 3 - Efficient): Especialistas em compactação de modelos. Tentam lançar produtos de baixo custo e latência ultra-baixa.
* **SakuraNet** (Tier 3 - Innovative): Laboratório focado em automações para robótica industrial e visão computacional avançada.
* **CortexAI** (Tier 3 - Pragmatic): Foca em ferramentas empresariais corporativas (B2B) tradicionais.

### Era 3: Corrida de APIs e Modelos de Fundação (2022 - 2024)
A inteligência artificial generativa explode no TechPulse:
* **AnthroTech** (`anthropic.png` - Tier 4): Lança seus primeiros modelos de contexto gigante baseados em segurança (Constitutional AI), atraindo grandes corporações financeiras.
* **MetaLogic AI** (Tier 4 - OpenSource): Lança modelos abertos concorrentes, diminuindo a barreira de entrada e desafiando a margem de lucro das outras empresas.
* **Quantum Minds** (`grok.png` - Tier 4): Entra forte no mercado integrando dados em tempo real de redes sociais. Seus posts no TechPulse são cínicos e humorísticos.

### Era 4: Fronteira e Corrida para AGI (2025 - 2026)
Modelos de raciocínio lógico (Reasoning) e Agentes Autônomos dominam o topo do mercado:
* **NeuraCorp** (`openai.png` - Tier 5): Consome volumes gigantescos de energia com superclusters de computação, tentando manter o monopólio da inteligência de fronteira.
* **DeepForge Labs** (Tier 5 - Scientific): Publica papers revolucionários que cortam o custo de treinamento pela metade, desafiando a NeuraCorp.
* **Nexus Systems** (Tier 5 - Enterprise): Especialistas em integrações governamentais de segurança nacional.

---

## 4. Integração Visual dos Logotipos no Jogo

Para que as logos em `MyAssets/Rivals` apareçam no jogo, propomos a seguinte integração limpa na arquitetura:

1. **Classe `CompetitorCompany`**:
   - Adicionar o campo `string LogoPath` ou `Sprite Logo` para representar a imagem de perfil da empresa no TechPulse.
2. **Carregamento Dinâmico**:
   - Ao iniciar o `CompetitorManager`, o script tentará carregar as sprites da pasta correspondente:
     - NeuraCorp carrega `openai.png`.
     - AnthroTech carrega `anthropic.png`.
     - Quantum Minds carrega `grok.png`.
     - Outros concorrentes utilizam as cores e símbolos gerados dinamicamente no `GaragePrototypeBootstrap`.
3. **Exibição no Prefab do Post**:
   - Em `TechPulseUI.CreatePostUI`, em vez de gerar um caractere genérico (`▲`) no avatar, o sistema verifica se a empresa tem uma imagem correspondente definida no `CompetitorManager` e a aplica ao componente `Image` do `PostAvatar`.

---

## 5. Como as Empresas Concorrentes Agem Autonomamente

As ações das empresas rivais influenciam diretamente a jogabilidade do jogador através de:
* **Média de Qualidade do Mercado**: A qualidade dos lançamentos rivais define o "padrão ouro" do mercado. Se o jogador lança um modelo abaixo da média concorrente, sua reputação cai.
* **Ataques de Hype no TechPulse**: Empresas agressivas (NeuraCorp) postam provocações se o jogador demorar muito para lançar algo novo.
* **Flutuação de Preço e Clientes**: Quando a MetaLogic lança um grande modelo de código aberto gratuito, o jogador pode perder receita mensal se não se diferenciar por qualidade superior ou contratos personalizados.
