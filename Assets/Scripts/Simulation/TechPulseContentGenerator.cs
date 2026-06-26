using System;
using System.Collections.Generic;
using UnityEngine;

public static class TechPulseContentGenerator
{
    private static readonly string[] _productTypes = { "Chatbot", "Automação", "OCR", "Classificação de Imagem", "Recomendador", "Voz", "Pesquisa", "Atendimento", "Modelo de IA" };

    // 1. Official Player Announcements (Templates)
    private static readonly string[] _playerExcellentTemplates = {
        "Acabamos de lançar {produto}! Nosso novo {categoria} atingiu incríveis {qualidade}% de qualidade interna. SOTA absoluto no mercado! {empresa} liderando a revolução! 🚀",
        "Apresentamos {produto}, o {categoria} mais avançado do setor. Com {qualidade}% de acerto, superamos {concorrente} e redefinimos o que é possível. O futuro começou na {empresa}! 🌐",
        "Estamos sem palavras. {produto} ({categoria}) foi lançado com {qualidade}% de precisão. O hype é real! Agradecemos aos nossos desenvolvedores na {empresa}! 🔥"
    };

    private static readonly string[] _playerMediumTemplates = {
        "Lançamento oficial: {produto}! Um {categoria} robusto e eficiente desenvolvido pela equipe da {empresa}. Qualidade estável em {qualidade}%. Experimente hoje!",
        "Chegou o {produto}! Nosso mais recente {categoria} foca em usabilidade e performance, entregando {qualidade}% de precisão. Ideal para automações! ⚙️",
        "Mais um passo rumo à inovação: {produto} ({categoria}) está no ar. A {empresa} continua aprimorando suas tecnologias com {qualidade}% de qualidade provada."
    };

    private static readonly string[] _playerWeakTemplates = {
        "Lançamos {produto} ({categoria}). Sabemos que ainda há muito o que melhorar de {qualidade}%, mas queríamos entregar esse MVP para vocês. Feedbacks são bem-vindos na {empresa}.",
        "Colocamos {produto} no ar. Este {categoria} opera em {qualidade}% de qualidade. Um lançamento cauteloso para aprendermos com os dados em produção.",
        "A {empresa} anuncia o lançamento básico de {produto}, um {categoria} funcional de {qualidade}% de performance. Atualizações em breve."
    };

    // 2. Teasers & Marketing Campaigns
    private static readonly string[] _teaserTemplates = {
        "Estamos cozinhando algo gigante na {empresa}. Nosso próximo {categoria} vai mudar o jogo. Quem aí consegue adivinhar o nome? 👀 #AI #Hype",
        "Bastidores da {empresa}: Testando novos modelos de {categoria}. A precisão interna está insana! Fiquem de olho no que está por vir.",
        "Por que usar soluções lentas se você pode ter o melhor? Em breve, {empresa} trará a resposta definitiva para problemas de {categoria}."
    };

    private static readonly string[] _marketingTemplates = {
        "A {empresa} está investindo pesado em infraestrutura de ponta para entregar a melhor IA do Brasil. Nossa reputação de {reputacao}/100 fala por si só! 💎",
        "Com tecnologia proprietária de {categoria}, a {empresa} garante automação real e inteligência prática para o seu negócio. Junte-se aos nossos {seguidores} seguidores!",
        "Inteligência artificial não precisa ser complicada. Descubra como as soluções da {empresa} trazem eficiência de verdade. #TechPulse #AI"
    };

    // 3. User Comments / Reactions (Positive, Negative, Neutral)
    private static readonly string[] _positiveComments = {
        "Caramba, testei o {produto} e estou de cara! A {empresa} realmente se superou dessa vez. Muito melhor que o modelo da {concorrente}! 👏👏",
        "Finalmente um {categoria} que funciona de verdade! Esses {qualidade}% de qualidade não são brincadeira. Parabéns à {empresa}!",
        "Estava cético, mas o {produto} é incrível. O atendimento deles agora está em outro nível. Vale cada centavo! 🔥",
        "Sensacional! @{empresa} matando a pau como sempre. Esse {produto} resolveu minha dor em 5 minutos."
    };

    private static readonly string[] _negativeComments = {
        "Sério que a @{empresa} lançou isso? {produto} está cheio de bugs e a qualidade de {qualidade}% é piada perto da {concorrente}. Decepcionado.",
        "Não entendo o hype. Esse {categoria} ({produto}) é super lento e não entrega o que promete. Prefiro continuar usando soluções antigas.",
        "Dinheiro jogado fora. O {produto} falhou miseravelmente nos meus testes de carga. A {empresa} precisa polir muito isso ainda.",
        "Prometeram SOTA e entregaram um script básico. @{empresa}, melhore esses {qualidade}% aí, por favor!"
    };

    private static readonly string[] _neutralComments = {
        "Interessante o lançamento do {produto}. Parece uma alternativa viável ao que a {concorrente} oferece, embora nada revolucionário.",
        "Alguém já testou o {produto} da @{empresa}? Queria saber se o custo-benefício para {categoria} realmente vale a pena antes de migrar.",
        "Mais um {categoria} no mercado. A concorrência é boa para os preços, vamos ver como a {empresa} se posiciona a longo prazo.",
        "O {produto} funciona de forma ok. Nada extraordinário, mas atende ao básico para quem precisa de um {categoria} simples."
    };

    // 4. Competitor Comparisons
    private static readonly string[] _comparisonComments = {
        "O {produto} da @{empresa} é bom, mas o modelo da {concorrente} ainda é mais rápido para tarefas de {categoria}.",
        "Estava comparando o {produto} com a IA da {concorrente} e a {empresa} conseguiu ganhar em precisão! {qualidade}% vs {qualidadeConcorrente}%.",
        "Enquanto a {concorrente} cobra uma fortuna, a @{empresa} entregou um {categoria} excelente a preço justo. Grande dia!"
    };

    // 5. User Questions & Silly/Natural comments
    private static readonly string[] _userQuestions = {
        "@{empresa} quando vocês vão lançar suporte a novas linguagens no {produto}? Estou precisando muito para {categoria}!",
        "Isso funciona localmente ou precisa de conexão constante com o servidor? Parabéns pelo {produto}!",
        "Qual o limite de tokens desse {categoria}? Quero testar em produção na minha empresa."
    };

    private static readonly string[] _sillyComments = {
        "A IA vai dominar o mundo e eu ainda não sei usar o Git direito. Abraços para a equipe da {empresa}! 😂",
        "Se o {produto} programar por mim enquanto eu tomo café, já tem meu respeito absoluto. #DevLife",
        "Comprei o {produto} e agora meu gato acha que sou um gênio da tecnologia. Obrigado @{empresa}!",
        "Mais um dia, mais um framework de {categoria}. Mas esse da {empresa} parece simpático."
    };

    // 6. Rumors & Corporate News
    private static readonly string[] _rumors = {
        "Dizem por aí que a {empresa} está comprando novas placas de GPU para treinar um modelo secreto de {categoria}. Será? 🤫 #Vazamento",
        "Rumores fortes indicam que o próximo {produto} vai integrar NLP e visão de forma nativa. A {concorrente} que se cuide!",
        "Analistas apontam que a @{empresa} pode receber uma proposta milionária de aquisição após o sucesso do {produto}."
    };

    private static readonly string[] _corporateNews = {
        "MERCADO: {empresa} consolida-se como um player promissor no setor de {categoria}. A marca atingiu novos recordes de engajamento online.",
        "Análise de Tecnologia: Como a {empresa} conseguiu bater gigantes estabelecidos com recursos limitados de GPU? Leia mais em nosso blog.",
        "ARTIGO: O impacto do {produto} no mercado de trabalho tradicional. Como a automação de {categoria} está redefinindo tarefas corporativas."
    };

    // 7. Delay & Silence (Inactivity)
    private static readonly string[] _delayComments = {
        "Cadê as novidades, @{empresa}? Já faz {tempoSemLancamento} dias que não vemos um update de {categoria}. O mercado está cobrando!",
        "Sinto que a {empresa} sumiu um pouco do mapa. A {concorrente} está lançando novidades toda semana enquanto o {produto} continua sem patch.",
        "Alguém na @{empresa} ainda está acordado? Precisamos de novos lançamentos de {categoria} antes que o hype acabe de vez!"
    };

    // 8. Competitor Announcements
    private static readonly string[] _competitorAnnouncements = {
        "Orgulho em anunciar o novo {produtoConcorrente}! Nosso {categoria} alcançou marcas incríveis em benchmarks privados. A {concorrente} lidera o caminho!",
        "Lançamos um patch de otimização para todos os nossos clientes de {categoria}. Redução de latência em 45%! #InovacaoConcorrente",
        "Fechamos uma parceria de exclusividade de dados com grande provedor de conteúdo. Nossos modelos de {categoria} serão imbatíveis. 🚀"
    };

    private static readonly string[] _competitorNeutral = {
        "Nossa equipe na {concorrente} acaba de expandir o cluster de GPUs. Aceleração total no treinamento de modelos generativos corporativos.",
        "Segurança em IA é nossa prioridade. Hoje publicamos nossas novas diretrizes de alinhamento ético para {categoria}.",
        "Anunciamos que o lançamento do nosso próximo modelo de {categoria} foi adiado em duas semanas para refinamento adicional. Qualidade em primeiro lugar."
    };

    // 9. Competitor News (Layoffs, Funding, Incidents)
    private static readonly string[] _competitorLayoffs = {
        "MERCADO: @{concorrente} anuncia reestruturação interna e redução de 10% do pessoal para otimizar custos de GPU.",
        "Fontes internas dizem que a @{concorrente} está realocando engenheiros de produto para o time de pesquisa de AGI.",
        "Após o último lançamento abaixo do esperado, a @{concorrente} enfrenta demissões em massa no time de vendas."
    };

    private static readonly string[] _competitorFunding = {
        "FINANÇAS: @{concorrente} fecha rodada de investimentos Série B de $450M liderada por consórcio internacional. Foco total em supercomputação!",
        "A @{concorrente} garante parceria estratégica e aporte financeiro massivo para expandir seu cluster local de H100s. 🚀",
        "Rumores confirmados: @{concorrente} recebeu injeção de capital de parceiros de nuvem em troca de exclusividade de API."
    };

    private static readonly string[] _competitorIncidents = {
        "ALERT: A API da @{concorrente} apresenta lentidão global devido a uma sobrecarga nos servidores de inferência. Engenheiros já estão atuando.",
        "Oops, o último update do chatbot da @{concorrente} causou um loop infinito que responde tudo em latim medieval. Rollback em progresso! 💀",
        "Segurança: @{concorrente} reporta incidente de segurança isolado em base de dados pública. Nenhum dado de cliente foi afetado."
    };

    // 10. Random User Mentions targeting Player
    private static readonly string[] _userGeneralMentions = {
        "Estive testando o último modelo da @{empresa} e estou bem surpreso. O custo-benefício está excelente! 💻",
        "Alguém mais notou alucinações estranhas nas respostas da @{empresa}? Ontem ela me recomendou formatar o PC para limpar cache...",
        "A @{empresa} sumiu um pouco do mapa. Já faz tempo que não vejo atualizações de {categoria} por aqui. @concorrente está tomando conta!",
        "Se o modelo da @{empresa} conseguir responder aos meus e-mails enquanto tomo um café, viro cliente vitalício. ☕ #DevLife",
        "Mais uma rodada de testes na @{empresa}. Os resultados parecem sólidos para {categoria}, mas o custo ainda é uma barreira.",
        "A qualidade de {qualidade}% da @{empresa} é um bom MVP, mas para colocar em produção ainda prefiro usar o modelo da @{concorrente}.",
        "Estou desenvolvendo um app com a API da @{empresa} e o suporte local é sensacional. Recomendo testarem!"
    };

    // Helper to replace standard tokens
    public static string ReplaceTokens(string template, string company, string product, string category, string quality, string competitor, string idleTime, string reputation, string followers, string competitorProduct = "RivalBot", string competitorQuality = "75")
    {
        return template
            .Replace("{empresa}", company)
            .Replace("{produto}", product)
            .Replace("{categoria}", category)
            .Replace("{qualidade}", quality)
            .Replace("{concorrente}", competitor)
            .Replace("{tempoSemLancamento}", idleTime)
            .Replace("{reputacao}", reputation)
            .Replace("{seguidores}", followers)
            .Replace("{produtoConcorrente}", competitorProduct)
            .Replace("{qualidadeConcorrente}", competitorQuality);
    }

    // Main API to generate player's product launch post
    public static TechPulsePost GeneratePlayerLaunchPost(string company, string product, float quality, string category, string competitor, int followers, float reputation, int postIndex)
    {
        string template;
        if (quality >= 75f)
            template = _playerExcellentTemplates[UnityEngine.Random.Range(0, _playerExcellentTemplates.Length)];
        else if (quality >= 45f)
            template = _playerMediumTemplates[UnityEngine.Random.Range(0, _playerMediumTemplates.Length)];
        else
            template = _playerWeakTemplates[UnityEngine.Random.Range(0, _playerWeakTemplates.Length)];

        string content = ReplaceTokens(template, company, product, category, quality.ToString("F0"), competitor, "0", reputation.ToString("F0"), followers.ToString());

        var post = new TechPulsePost
        {
            Id = $"player_launch_{postIndex}",
            AuthorName = company,
            AuthorHandle = "@" + company.Replace(" ", "").ToLower(),
            AuthorColor = GameDesignConstants.BrandPrimary,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Hoje",
            Category = TechPulsePost.PostCategory.ModelLaunch,
            RelatedCompany = company,
            RelatedProduct = product,
            PerceivedQuality = quality,
            Priority = 10,
            Likes = Mathf.RoundToInt(followers * UnityEngine.Random.Range(0.08f, 0.25f) + UnityEngine.Random.Range(5, 20)),
            Reposts = Mathf.RoundToInt(followers * UnityEngine.Random.Range(0.01f, 0.08f) + UnityEngine.Random.Range(1, 5)),
            Replies = UnityEngine.Random.Range(3, 10)
        };

        // Generate Comments/Replies for this launch post
        GenerateLaunchComments(post, company, product, quality, category, competitor, followers);

        return post;
    }

    private static void GenerateLaunchComments(TechPulsePost post, string company, string product, float quality, string category, string competitor, int followers)
    {
        int numComments = UnityEngine.Random.Range(3, 8);
        var handlesUsed = new HashSet<string>();

        // Names list for commentators
        string[] userNames = { "Lucas Silva", "Beatriz Costa", "GamerDev", "AI_Enthusiast", "TechCruncher", "Clara Souza", "DevPedro", "Mariana_ML", "Gabriel_X", "SandroTech" };
        string[] userHandles = { "@lucas_silva", "@beatriz_c", "@gamer_dev", "@ai_enthusiast", "@tech_cruncher", "@clara_s", "@dev_pedro", "@mari_ml", "@gabriel_x", "@sandro_t" };

        for (int i = 0; i < numComments; i++)
        {
            int commenterIndex = UnityEngine.Random.Range(0, userNames.Length);
            string uName = userNames[commenterIndex];
            string uHandle = userHandles[commenterIndex];

            if (handlesUsed.Contains(uHandle)) continue;
            handlesUsed.Add(uHandle);

            // Choose comment type based on quality
            string template;
            float dice = UnityEngine.Random.value;

            if (quality >= 75f) // High quality
            {
                if (dice < 0.65f)
                    template = _positiveComments[UnityEngine.Random.Range(0, _positiveComments.Length)];
                else if (dice < 0.85f)
                    template = _comparisonComments[UnityEngine.Random.Range(0, _comparisonComments.Length)];
                else
                    template = _sillyComments[UnityEngine.Random.Range(0, _sillyComments.Length)];
            }
            else if (quality >= 45f) // Medium quality
            {
                if (dice < 0.40f)
                    template = _positiveComments[UnityEngine.Random.Range(0, _positiveComments.Length)];
                else if (dice < 0.80f)
                    template = _neutralComments[UnityEngine.Random.Range(0, _neutralComments.Length)];
                else
                    template = _userQuestions[UnityEngine.Random.Range(0, _userQuestions.Length)];
            }
            else // Low quality
            {
                if (dice < 0.65f)
                    template = _negativeComments[UnityEngine.Random.Range(0, _negativeComments.Length)];
                else if (dice < 0.85f)
                    template = _neutralComments[UnityEngine.Random.Range(0, _neutralComments.Length)];
                else
                    template = _userQuestions[UnityEngine.Random.Range(0, _userQuestions.Length)];
            }

            string commentText = ReplaceTokens(template, company, product, category, quality.ToString("F0"), competitor, "0", "0", "0", "RivalGPT", "72");
            var comment = new TechPulseComment(
                $"c_{post.Id}_{i}",
                uName,
                uHandle,
                commentText,
                TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Hoje"
            );
            post.Comments.Add(comment);
        }
    }

    // Generate other organic posts
    public static TechPulsePost GenerateOrganicPost(string company, int followers, float reputation, string category, int postIndex, bool isDelay, int daysInactivity)
    {
        string template;
        TechPulsePost.PostCategory postCategory = TechPulsePost.PostCategory.Trending;

        if (isDelay)
        {
            template = _delayComments[UnityEngine.Random.Range(0, _delayComments.Length)];
            postCategory = TechPulsePost.PostCategory.Incident;
        }
        else
        {
            float r = UnityEngine.Random.value;
            if (r < 0.35f)
            {
                template = _teaserTemplates[UnityEngine.Random.Range(0, _teaserTemplates.Length)];
                postCategory = TechPulsePost.PostCategory.Trending;
            }
            else if (r < 0.70f)
            {
                template = _marketingTemplates[UnityEngine.Random.Range(0, _marketingTemplates.Length)];
                postCategory = TechPulsePost.PostCategory.Trending;
            }
            else
            {
                template = _rumors[UnityEngine.Random.Range(0, _rumors.Length)];
                postCategory = TechPulsePost.PostCategory.Trending;
            }
        }

        string randomProduct = "ModelX_" + UnityEngine.Random.Range(1, 9);
        string competitor = "NeuraCorp";
        if (CompetitorManager.Instance != null)
        {
            var rComp = CompetitorManager.Instance.GetRandomCompany();
            if (rComp != null) competitor = rComp.Name;
        }

        string content = ReplaceTokens(template, company, randomProduct, category, "70", competitor, daysInactivity.ToString(), reputation.ToString("F0"), followers.ToString());

        // For delay/inactivity, the author is a random complaining user
        string authorName = company;
        string authorHandle = "@" + company.Replace(" ", "").ToLower();
        Color authorColor = GameDesignConstants.BrandSecondary;

        if (isDelay)
        {
            string[] userNames = { "Carlos Dev", "Aline Santos", "MachineLover", "TechGuru", "GigaCoder", "Julia_AI", "Pedro_Tech", "Sofia_ML" };
            string[] userHandles = { "@carlos_dev", "@aline_s", "@machine_lover", "@tech_guru", "@giga_coder", "@julia_ai", "@pedro_tech", "@sofia_ml" };
            int userIdx = UnityEngine.Random.Range(0, userNames.Length);
            authorName = userNames[userIdx];
            authorHandle = userHandles[userIdx];
            authorColor = new Color(0.6f, 0.6f, 0.7f);
        }

        var post = new TechPulsePost
        {
            Id = $"player_organic_{postIndex}",
            AuthorName = authorName,
            AuthorHandle = authorHandle,
            AuthorColor = authorColor,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Hoje",
            Category = postCategory,
            RelatedCompany = company,
            Priority = 5,
            Likes = Mathf.RoundToInt(followers * UnityEngine.Random.Range(0.02f, 0.12f) + UnityEngine.Random.Range(1, 10)),
            Reposts = Mathf.RoundToInt(followers * UnityEngine.Random.Range(0.005f, 0.03f) + UnityEngine.Random.Range(0, 3)),
            Replies = UnityEngine.Random.Range(1, 4)
        };

        return post;
    }

    // Generate competitor post
    public static TechPulsePost GenerateCompetitorPost(CompetitorCompany comp, int postIndex)
    {
        string template;
        TechPulsePost.PostCategory cat = TechPulsePost.PostCategory.Benchmark;

        float dice = UnityEngine.Random.value;
        if (dice < 0.30f) // Launches (ModelLaunch)
        {
            template = _competitorAnnouncements[UnityEngine.Random.Range(0, _competitorAnnouncements.Length)];
            cat = TechPulsePost.PostCategory.ModelLaunch;
        }
        else if (dice < 0.45f) // Layoffs
        {
            template = _competitorLayoffs[UnityEngine.Random.Range(0, _competitorLayoffs.Length)];
            cat = TechPulsePost.PostCategory.Incident;
        }
        else if (dice < 0.60f) // Funding
        {
            template = _competitorFunding[UnityEngine.Random.Range(0, _competitorFunding.Length)];
            cat = TechPulsePost.PostCategory.Funding;
        }
        else if (dice < 0.75f) // Incidents
        {
            template = _competitorIncidents[UnityEngine.Random.Range(0, _competitorIncidents.Length)];
            cat = TechPulsePost.PostCategory.Incident;
        }
        else // Neutral Benchmarks
        {
            template = _competitorNeutral[UnityEngine.Random.Range(0, _competitorNeutral.Length)];
            cat = TechPulsePost.PostCategory.Benchmark;
        }

        string randomProd = comp.Name + "-" + _productTypes[UnityEngine.Random.Range(0, _productTypes.Length)];
        string randomCat = _productTypes[UnityEngine.Random.Range(0, _productTypes.Length)];

        string content = ReplaceTokens(template, "Model Foundry", "AI_X", randomCat, "80", comp.Name, "0", "50", "1000", randomProd, "82");

        var post = new TechPulsePost
        {
            Id = $"comp_{comp.Name}_{postIndex}",
            AuthorName = comp.Name,
            AuthorHandle = comp.Handle,
            AuthorColor = comp.BrandColor,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Hoje",
            Category = cat,
            RelatedCompany = comp.Name,
            PerceivedQuality = UnityEngine.Random.Range(60f, 95f),
            Priority = 6,
            Likes = UnityEngine.Random.Range(100, 5000) * comp.StrengthTier,
            Reposts = UnityEngine.Random.Range(10, 500) * comp.StrengthTier,
            Replies = UnityEngine.Random.Range(5, 120) * comp.StrengthTier
        };

        return post;
    }

    // 11. Generate random user mention targeting player
    public static TechPulsePost GenerateUserMentionPost(string company, int followers, float reputation, string category, string competitor, int postIndex, float qualityVal)
    {
        string template = _userGeneralMentions[UnityEngine.Random.Range(0, _userGeneralMentions.Length)];
        string content = ReplaceTokens(template, company, "Modelo", category, qualityVal.ToString("F0"), competitor, "0", reputation.ToString("F0"), followers.ToString());

        string[] userNames = { "Carlos Dev", "Aline Santos", "MachineLover", "TechGuru", "GigaCoder", "Julia_AI", "Pedro_Tech", "Sofia_ML" };
        string[] userHandles = { "@carlos_dev", "@aline_s", "@machine_lover", "@tech_guru", "@giga_coder", "@julia_ai", "@pedro_tech", "@sofia_ml" };

        int userIdx = UnityEngine.Random.Range(0, userNames.Length);
        string uName = userNames[userIdx];
        string uHandle = userHandles[userIdx];

        return new TechPulsePost
        {
            Id = $"user_mention_{postIndex}",
            AuthorName = uName,
            AuthorHandle = uHandle,
            AuthorColor = new Color(0.6f, 0.6f, 0.7f),
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Hoje",
            Category = TechPulsePost.PostCategory.Trending,
            RelatedCompany = company,
            Priority = 4,
            Likes = UnityEngine.Random.Range(5, 50),
            Reposts = UnityEngine.Random.Range(0, 10),
            Replies = UnityEngine.Random.Range(0, 5)
        };
    }
}
