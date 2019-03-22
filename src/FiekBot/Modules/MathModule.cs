using System.Threading.Tasks;
using Discord.Commands;
using FiekBot.Math;

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
        public async Task HelpAsync([Name("shprehja"), Remainder] string expression)
        {
            var result = Evaluator.Evaluate(expression);
            await ReplyAsync($"Rezultati: {result}");
        }
    }
}
