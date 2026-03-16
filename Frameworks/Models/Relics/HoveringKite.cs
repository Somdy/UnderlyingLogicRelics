using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SilentRelicPool))]
    [OldRelicAncientPool(AncientPoolType.DARV)]
    public class HoveringKite : OldRelicModelBase
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
        
        private bool _triggered;

        public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
        {
            if (!_triggered)
            {
                _triggered = true;
                Flash();
                await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            }
        }

        public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            _triggered = false;
            return base.AfterPlayerTurnStartEarly(choiceContext, player);
        }

        public override Task BeforeCombatStart()
        {
            _triggered = false;
            return base.BeforeCombatStart();
        }

        public override bool AncientAllow(AncientPoolType ancient, Player p) => p.Character is Silent;
    }
}