using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Runs;
using UnderlyingLogicRelics.Frameworks.Core;

// ReSharper disable InconsistentNaming

namespace UnderlyingLogicRelics.Frameworks.Patches
{
    public class HooksPatch
    {
        [HarmonyPatch(typeof(CardFactory), nameof(CardFactory.CreateForReward))]
        [HarmonyPatch([typeof(Player), typeof(int), typeof(CardCreationOptions)])]
        public static class CardFactoryCreateForRewardPatch
        {
            static void Prefix(Player player, ref int cardCount, CardCreationOptions options)
            {
                OldRelicHooks.BeforeCardRewardCreate(player.RunState, player, ref cardCount, options);
            }
        }
    }
}