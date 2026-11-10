using System.ComponentModel;

namespace AgendaAgent;

/// <summary>
/// The schedule agent's tool. Sample data in memory; this is not the event's real schedule.
/// A C# method decorated with [Description] is enough for the model to know when to call it.
/// </summary>
public static class Tools
{
    private sealed record Session(string Time, string Title, string Speaker, string[] Topics);

    private static readonly Session[] Sessions =
    [
        new("18:00", "Your first agent with Microsoft Agent Framework", "Speaker A", ["agents", "agent framework", "c#", "beginner"]),
        new("18:40", "RAG in Foundry without losing your mind", "Speaker B", ["rag", "search", "data", "foundry"]),
        new("19:20", "From code to Foundry: agent infrastructure with Aspire", "Emiliano Montesdeoca", ["aspire", "infrastructure", "deployment", "agents", "foundry"]),
        new("20:00", "Evaluate agents before your users do", "Speaker C", ["evaluation", "quality", "observability"]),
        new("20:40", "Multi-agent systems: when to use them and when not to", "Speaker D", ["agents", "multi-agent", "architecture"]),
    ];

    [Description("Searches the sample schedule by topic. Returns the time, title, and speaker. If the topic is empty, returns the full schedule.")]
    public static string SearchSessions(
        [Description("The topic the attendee is interested in, for example: RAG, agents, or deployment")] string topic)
    {
        var results = string.IsNullOrWhiteSpace(topic)
            ? Sessions
            : Sessions.Where(session =>
                session.Topics.Any(item => item.Contains(topic, StringComparison.OrdinalIgnoreCase)) ||
                session.Title.Contains(topic, StringComparison.OrdinalIgnoreCase)).ToArray();

        return results.Length == 0
            ? $"No sessions found about \"{topic}\"."
            : string.Join("\n", results.Select(session => $"{session.Time} · {session.Title} · {session.Speaker}"));
    }
}
