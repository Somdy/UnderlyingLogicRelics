using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Utils;

namespace UnderlyingLogicRelics.Frameworks.Patches.Content.Ancients
{
    public class DarvRelicPatch
    {
        
        protected internal static readonly List<ValidRelic> RelicsToDarv = [];
        
        [HarmonyPatch(typeof(Darv), "GenerateInitialOptions")]
        public static class AddSharedOldRelicsToDarvPool
        {
            static void Postfix(Darv __instance, ref IReadOnlyList<EventOption> __result)
            {
                if (RelicsToDarv.Count <= 0)
                {
                    return;
                }
                
                bool dustyTome = __result.Any(o => o.Relic is DustyTome);
                __result = __result.Where(o => o.Relic is not DustyTome).ToList();
                MethodInfo relicOptCreator = typeof(AncientEventModel).GetMethod("RelicOption", 
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    [ typeof(RelicModel), typeof(string), typeof(string) ]);
                List<EventOption> candidates = RelicsToDarv.Where(vr => vr.match(AncientPoolType.DARV, __instance.Owner))
                                                           .Select(vr => (EventOption) relicOptCreator?.Invoke(__instance, 
                                                               [ vr.relic.ToMutable(), "INITIAL", null ]))
                                                           .Union(__result)
                                                           .ToList()
                                                           .UnstableShuffle(__instance.Rng);
                if (candidates.Count == 0)
                {
                    return;
                }
                candidates.ForEach(o => ORLog.Info($"Possible relic: {o.Relic}"));
                if (dustyTome)
                {
                    __result = candidates.Take(2).ToList();
                    DustyTome tome = (DustyTome) ModelDb.Relic<DustyTome>().ToMutable();
                    if (__instance.Owner != null)
                    {
                        tome.SetupForPlayer(__instance.Owner);
                    }
                    __result = __result.AddItem((EventOption) relicOptCreator?.Invoke(__instance, [ tome, "INITIAL", null ])).ToList();
                }
                else
                {
                    __result = candidates.Take(3).ToList();
                }
            }
        }
    }

    public struct ValidRelic
    {
        public Func<AncientPoolType, Player, bool> match;

        public RelicModel relic;
    }
}