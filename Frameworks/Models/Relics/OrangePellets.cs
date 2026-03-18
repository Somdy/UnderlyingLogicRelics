using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SharedRelicPool))]
    public class OrangePellets : OldRelicModelBase
    {
        public override RelicRarity Rarity => RelicRarity.Shop;
        
        private HashSet<CardType> _uniqueTypes = [];
        
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner || !f_validType(cardPlay.Card.Type))
            {
                return;
            }

            if (!_uniqueTypes.Add(cardPlay.Card.Type))
            {
                return;
            }

            if (_uniqueTypes.Count >= 3)
            {
                Flash();
                _uniqueTypes.Clear();
                var debuffs = Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff)
                                   .Select(p => p)
                                   .ToList();
                foreach (PowerModel power in debuffs)
                {
                    await PowerCmd.Remove(power);
                }
            }

            return;

            bool f_validType(CardType t) => t is CardType.Attack or CardType.Skill or CardType.Power;
        }

        public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            if (player == Owner)
            {
                _uniqueTypes.Clear();
            }
            return base.AfterPlayerTurnStartEarly(choiceContext, player);
        }
    }
}