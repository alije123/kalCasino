using System.Collections.ObjectModel;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace kalCasino.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ReqireCategoriesAttribute : CheckBaseAttribute
{
    public IReadOnlyList<string> CategoryNames { get; }
    public ChannelCheckMode CheckMode { get; }

    public ReqireCategoriesAttribute(ChannelCheckMode checkMode, params string[] channelIds)
    {
        CheckMode = checkMode;
        CategoryNames = new ReadOnlyCollection<string>(channelIds);
    }
    
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        if (ctx.Guild == null || ctx.Member == null)
        {
            return Task.FromResult(false);
        }

        bool contains = CategoryNames.Contains(ctx.Channel.Parent.Id.ToString(), StringComparer.OrdinalIgnoreCase);

        return CheckMode switch
        {
            ChannelCheckMode.Any => Task.FromResult(contains),

            ChannelCheckMode.None => Task.FromResult(!contains),

            _ => Task.FromResult(false)
        };
    }
}