using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;
using UnderlyingLogicRelics.Frameworks.Utils;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SharedRelicPool))]
    [OldRelicAncientPool(AncientPoolType.DARV)]
    public class FusionHammer : OldRelicModelBase
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

        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != Owner)
            {
                return false;
            }
            options.RemoveAll(o => o is SmithRestSiteOption);
            return true;
        }
    }
}