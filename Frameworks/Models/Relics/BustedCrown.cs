using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Interfaces;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SharedRelicPool))]
    [OldRelicAncientPool(AncientPoolType.DARV)]
    public class BustedCrown : OldRelicModelBase, IRewardCardCountModifier
    {
        public const int REDUCTION = 2;
        
        public override RelicRarity Rarity => RelicRarity.Ancient;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new DynamicVar(nameof(REDUCTION), REDUCTION)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player == Owner)
            {
                amount += DynamicVars.Energy.IntValue;
            }
            return base.ModifyMaxEnergy(player, amount);
        }

        public int ModifyRewardCardCount(Player player, int cardCount, CardCreationOptions options)
        {
            if (player != Owner || options.Source != CardCreationSource.Encounter)
            {
                return cardCount;
            }
            
            Flash();
            cardCount -= DynamicVars[nameof(REDUCTION)].IntValue;
            return cardCount;
        }
    }
}