using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SharedRelicPool))]
    public class ClockworkSouvenir : OldRelicModelBase
    {
        public const decimal ARTIFACT_AMT = 1m;
        
        public override RelicRarity Rarity => RelicRarity.Shop;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ArtifactPower>(nameof(ARTIFACT_AMT), ARTIFACT_AMT)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ArtifactPower>()];

        public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
        {
            if (side != Owner.Creature.Side || combatState.RoundNumber > 1)
            {
                return;
            }
            await PowerCmd.Apply<ArtifactPower>(Owner.Creature, DynamicVars[nameof(ARTIFACT_AMT)].BaseValue, null, null);
        }
    }
}