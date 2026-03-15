using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicsAutoAdd(typeof(SharedRelicPool))]
    [OldRelicsAncientPool(AncientPoolType.DARV)]
    public class RunicDome : OldRelicModelBase
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player == Owner)
            {
                amount += DynamicVars.Energy.IntValue;
            }
            return base.ModifyMaxEnergy(player, amount);
        }
    }
}