using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.RelicPools;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicsAutoAdd(typeof(DefectRelicPool))]
    [OldRelicsAncientPool(AncientPoolType.DARV)]
    public class NuclearBattery : OldRelicModelBase
    {
        private const string PLASMA = "Plasma";
        
        public override RelicRarity Rarity => RelicRarity.Ancient;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [new(PLASMA, 1m)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.Static(StaticHoverTip.Channeling), 
            HoverTipFactory.FromOrb<PlasmaOrb>()
        ];

        public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
        {
            if (side != Owner.Creature.Side || combatState.RoundNumber > 1)
            {
                return;
            }
            for (decimal i = 0; i < DynamicVars[PLASMA].BaseValue; i++)
            {
                await OrbCmd.Channel<PlasmaOrb>(new BlockingPlayerChoiceContext(), Owner);
            }
        }

        public override bool AncientAllow(AncientPoolType ancient, Player p) => p.Character is Defect;
    }
}