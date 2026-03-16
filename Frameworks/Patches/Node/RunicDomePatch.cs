using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using UnderlyingLogicRelics.Frameworks.Models.Relics;
using UnderlyingLogicRelics.Frameworks.Utils;
using Label = System.Reflection.Emit.Label;
// ReSharper disable InconsistentNaming

namespace UnderlyingLogicRelics.Frameworks.Patches.Node
{
    public class RunicDomePatch
    {
        [HarmonyPatch(typeof(NIntent), nameof(NIntent._Process))]
        public static class NIntentVisiblePatch
        {
            static void Prefix(NIntent __instance, Creature ____owner, AbstractIntent ____intent)
            {
                Player whoAmI = LocalContext.GetMe(____owner.CombatState);
                bool wasDomed = whoAmI != null && whoAmI.Relics.Any(r => r is RunicDome);
                __instance.Visible = !wasDomed;
            }
        }
        
        [HarmonyPatch(typeof(Creature), MethodType.Getter)]
        [HarmonyPatch(nameof(Creature.HoverTips))]
        public static class HideIntentTipsPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
            {
                MethodInfo getterHasIntentTipInfo = AccessTools.PropertyGetter(typeof(AbstractIntent), nameof(AbstractIntent.HasIntentTip));
                MethodInfo getterHoverTipsInfo = AccessTools.PropertyGetter(typeof(PowerModel), nameof(PowerModel.HoverTips));
                MethodInfo moveNextInfo = AccessTools.Method(typeof(IEnumerator), nameof(IEnumerator.MoveNext));
                MethodInfo canShowIntentTipsInfo = AccessTools.Method(typeof(RunicDomePatch), nameof(CanShowIntentTips));
                FieldInfo fieldPowersInfo = AccessTools.Field(typeof(Creature), "_powers");
                var matcher = new CodeMatcher(instructions, generator);
                matcher.DeclareLocal(typeof(bool), out LocalBuilder locBool)
                       .MatchStartForward(
                           // Matches the following sequences and move the cursor to the start (ldloc.1)
                           new CodeMatch(OpCodes.Ldloc_1),
                           new CodeMatch(OpCodes.Callvirt, moveNextInfo),
                           new CodeMatch(OpCodes.Brtrue_S)
                       )
                       .ThrowIfNotMatch($"Could not find any entry for failed label")
                       .DefineLabel(out Label failedSkipDest)
                       .AddLabels([failedSkipDest])
                       .Start()
                       .MatchStartForward(
                           new CodeMatch(OpCodes.Ldloc_2), // intent
                           new CodeMatch(OpCodes.Callvirt, getterHasIntentTipInfo),
                           new CodeMatch(OpCodes.Brfalse_S)
                       )
                       .ThrowIfNotMatch($"Could not find any entry for insert location")
                       .Advance()
                       .Insert(
                           new CodeInstruction(OpCodes.Ldarg_0),
                           new CodeInstruction(OpCodes.Call, canShowIntentTipsInfo),
                           new CodeInstruction(OpCodes.Stloc, locBool),
                           new CodeInstruction(OpCodes.Ldloc, locBool),
                           new CodeInstruction(OpCodes.Brfalse_S, failedSkipDest),
                           new CodeInstruction(OpCodes.Ldloc_2)
                       );
                return matcher.InstructionEnumeration();
            }
        }
        
        [HarmonyPatch(typeof(NIntent), "OnHovered")]
        public static class NIntentTipsPatch
        {
            static bool Prefix(NIntent __instance, Creature ____owner)
            {
                Player whoAmI = LocalContext.GetMe(____owner.CombatState);
                bool wasDomed = whoAmI != null && whoAmI.Relics.Any(r => r is RunicDome);
                bool skip = !wasDomed;
                return skip;
            }
        }

        public static bool CanShowIntentTips(AbstractIntent intent, Creature owner)
        {
            Player whoAmI = LocalContext.GetMe(owner.CombatState);
            bool wasDomed = whoAmI != null && whoAmI.Relics.Any(r => r is RunicDome);
            return !wasDomed;
        }
    }
}