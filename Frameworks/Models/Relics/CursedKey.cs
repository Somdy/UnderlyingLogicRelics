using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;
using UnderlyingLogicRelics.Frameworks.Models.Cards;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SharedRelicPool))]
    [OldRelicAncientPool(AncientPoolType.DARV)]
    public class CursedKey : OldRelicModelBase
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

        protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Fright>()
            .Union([HoverTipFactory.ForEnergy(this)]);

        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player == Owner)
            {
                amount += DynamicVars.Energy.IntValue;
            }
            return base.ModifyMaxEnergy(player, amount);
        }

        public override async Task AfterRoomEntered(AbstractRoom room)
        {
            if (room is TreasureRoom)
            {
                Flash();
                await CardPileCmd.AddCurseToDeck<Fright>(Owner);
            }
        }

        public override bool AncientAllow(AncientPoolType ancient, Player p) => p.RunState.CurrentActIndex <= 1;
    }
}