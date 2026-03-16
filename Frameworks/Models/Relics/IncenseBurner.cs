using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SharedRelicPool))]
    [OldRelicAncientPool(AncientPoolType.DARV)]
    public class IncenseBurner : OldRelicModelBase
    {
        public const int COOLDOWN = 6;
        public const int POWER_AMT = 1;
        
        public override RelicRarity Rarity => RelicRarity.Ancient;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new(nameof(COOLDOWN), COOLDOWN), 
            new(nameof(POWER_AMT), POWER_AMT)
        ];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];
        
        public override bool ShowCounter => true;

        public override int DisplayAmount => TurnCounter % DynamicVars[nameof(COOLDOWN)].IntValue;

        private int _turnCounter;
        [SavedProperty]
        public int TurnCounter
        {
            get => _turnCounter;
            set
            {
                AssertMutable();
                _turnCounter = value;
                InvokeDisplayAmountChanged();
            }
        }

        public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
            {
                return;
            }

            TurnCounter++;
            int targetCounter = DynamicVars[nameof(COOLDOWN)].IntValue;
            if (!CombatManager.Instance.IsInProgress || TurnCounter % targetCounter != 0)
            {
                return;
            }

            Flash();
            await PowerCmd.Apply<IntangiblePower>(Owner.Creature, DynamicVars[nameof(POWER_AMT)].BaseValue, Owner.Creature, null);
        }
    }
}