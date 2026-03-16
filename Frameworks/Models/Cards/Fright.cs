using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using UnderlyingLogicRelics.Frameworks.Core;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Cards
{
    [OldRelicAutoAdd(typeof(CurseCardPool))]
    public class Fright() : OldRelicCardModelBase(ENERGY_COST, TYPE, RARITY, TARGET)
    {
        public const int ENERGY_COST = -1;
        public const CardType TYPE = CardType.Curse;
        public const CardRarity RARITY = CardRarity.Curse;
        public const TargetType TARGET = TargetType.None;
        public const decimal COMBATS_TO_REMOVE = 2;

        public override int MaxUpgradeLevel => 0;
        public override bool CanBeGeneratedByModifiers => false;

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal, CardKeyword.Ethereal, CardKeyword.Unplayable];
        protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(COMBATS_TO_REMOVE), COMBATS_TO_REMOVE)];
        
        private int _combatsSeen;
        [SavedProperty]
        public int CombatsSeen
        {
            get => _combatsSeen;
            set
            {
                AssertMutable();
                _combatsSeen = value;
                DynamicVars[nameof(COMBATS_TO_REMOVE)].BaseValue = COMBATS_TO_REMOVE - _combatsSeen;
            }
        }

        public override async Task AfterCombatEnd(CombatRoom room)
        {
            if (Pile is not { Type: PileType.Deck })
            {
                return;
            }
            CombatsSeen++;
            if (CombatsSeen == COMBATS_TO_REMOVE && Pile?.Type == PileType.Deck)
            {
                await CardPileCmd.RemoveFromDeck(this);
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
            {
                return;
            }
            if (Pile is { Type: PileType.Hand })
            {
                IEnumerable<CardModel> cardsToDiscard = Pile.Type.GetPile(Owner).Cards
                                                                .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection);
                await CardCmd.Discard(choiceContext, cardsToDiscard);
            }
        }
    }
}