using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace UnderlyingLogicRelics.Frameworks.Interfaces
{
    public interface IRewardCardCountModifier
    {
        int ModifyRewardCardCount(Player player, int cardCount, CardCreationOptions options);
    }
}