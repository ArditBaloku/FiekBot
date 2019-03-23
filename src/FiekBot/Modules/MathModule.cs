using System.Threading.Tasks;
using Discord.Commands;
using FiekBot.Expressions;
using FiekBot.Utils;

namespace FiekBot.Modules
{
    [Name("Llogaritjet")]
    [Summary("Llogaritje dhe veprime matematikore")]
    public class MathModule : Module
    {
        public MathModule(IExpressionEvaluator evaluator)
        {
            Evaluator = evaluator;
        }

        public IExpressionEvaluator Evaluator { get; }

        [Command("llogarit"), Alias("calc")]
        [Summary("Llogarit shprehjen matematikore.")]
        public async Task HelpAsync([Name("shprehja"), Remainder] string expression)
        {
            var result = Evaluator.Evaluate(expression);
            await ReplyAsync($"Rezultati: {result}");
        }

        [Command("dist"), Alias("distanca")]
        [Summary("Llogarit distancën mes teksteve.")]
        public async Task DistanceAsync(string a, string b)
        {
            var result = StringUtils.Distance(a, b);
            await ReplyAsync($"Distanca ndërmjet \"{a}\" dhe \"{b}\" është {result}");
        }
    }
}
