using System.Collections.Generic;

public static class LocalizationManager
{
    public const string CurrentLanguage = "en";

    private static readonly Dictionary<string, string> English = new Dictionary<string, string>
    {
        { "startup.title", "Founder Operating System" },
        { "startup.subtitle", "Build a real AI company from zero: research, product, infrastructure, trust." },
        { "startup.stage", "Stage" },
        { "startup.cash", "Cash" },
        { "startup.burn", "Monthly Burn" },
        { "startup.revenue", "Monthly Revenue" },
        { "startup.research", "Research" },
        { "startup.model", "Model Capability" },
        { "startup.website", "Website" },
        { "startup.product", "Product UX" },
        { "startup.infra", "Infrastructure" },
        { "startup.trust", "Trust" },
        { "startup.team", "Team" },
        { "startup.study", "Study Papers" },
        { "startup.website_action", "Improve Website" },
        { "startup.prototype", "Build Prototype" },
        { "startup.hosting", "Secure Cloud Hosting" },
        { "startup.designer", "Hire Designer" },
        { "startup.developer", "Hire Developer" },
        { "startup.researcher", "Hire Scientist" },
        { "startup.cli", "Launch CLI Agent" },
        { "startup.builder", "Build Agent Studio" },
        { "startup.arena", "Submit to AI Arena" },
        { "startup.next", "Next goal" },
        { "startup.news_study", "Founder research session complete. The lab understands the field a little better." },
        { "startup.news_website", "Website updated: clearer positioning, better docs and stronger conversion." },
        { "startup.news_prototype", "Prototype shipped internally. It is not frontier-grade, but it is real progress." },
        { "startup.news_hosting", "Cloud hosting contract signed. Training and inference capacity are more reliable." },
        { "startup.news_designer", "Designer hired. Product clarity and launch quality improved." },
        { "startup.news_developer", "Developer hired. Product velocity and CLI work can finally scale." },
        { "startup.news_researcher", "Research scientist hired. Experiments now have stronger technical direction." },
        { "startup.news_cli", "CLI coding agent launched to early developers. Adoption depends on trust, docs and reliability." },
        { "startup.news_builder", "Agent Studio prototype launched. Enterprise buyers are interested, but risk is high." },
        { "startup.news_arena", "AI Arena submission complete. The public now has a benchmark signal for the company." },
        { "startup.fail_cash", "Not enough cash for this move." },
        { "startup.fail_require_dev", "A developer is required before launching developer tools." },
        { "startup.fail_require_model", "Model capability is too low for this launch." },
        { "startup.fail_require_infra", "Infrastructure is too weak for this launch." },
        { "startup.goal_0", "Study papers, improve the website, and build a prototype." },
        { "startup.goal_1", "Hire a developer or designer, then launch a credible product surface." },
        { "startup.goal_2", "Secure hosting, improve model capability, and submit to AI Arena." },
        { "startup.goal_3", "Build a product ecosystem: CLI, API, builder, support and trust." },
        { "startup.goal_4", "Scale toward a frontier lab without letting cost, trust or infrastructure collapse." },
        { "techpulse.bio", "Building AI products, agents and infrastructure in public." },
        { "techpulse.empty", "No signal yet. Ship something worth talking about." }
    };

    public static string T(string key)
    {
        return English.TryGetValue(key, out var value) ? value : key;
    }
}
