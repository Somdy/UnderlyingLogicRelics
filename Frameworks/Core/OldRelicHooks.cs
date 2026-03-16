using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using UnderlyingLogicRelics.Frameworks.Interfaces;

namespace UnderlyingLogicRelics.Frameworks.Core
{
    public class OldRelicHooks
    {
        public static void BeforeCardRewardCreate(IRunState runState, Player player, ref int cardCount, CardCreationOptions options)
        {
            foreach (AbstractModel item in runState.IterateHookListeners(null))
            {
                int modifiedCardCount = cardCount;
                if (item is IRewardCardCountModifier modifier)
                {
                    modifiedCardCount = modifier.ModifyRewardCardCount(player, modifiedCardCount, options);
                }
                cardCount = modifiedCardCount;
            }
        }
    }
}