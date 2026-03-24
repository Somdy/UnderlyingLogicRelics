using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;

namespace UnderlyingLogicRelics.Frameworks.Models.Relics
{
    [OldRelicAutoAdd(typeof(SharedRelicPool))]
    public class Sundial : OldRelicModelBase
    {
        private const int SHUFFLES = 3;
        
        public override RelicRarity Rarity => RelicRarity.Uncommon;
        
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(nameof(SHUFFLES), SHUFFLES),
            new EnergyVar(2)
        ];
        
        public override bool ShowCounter => true;

        public override int DisplayAmount
        {
            get
            {
                if (!IsActivating)
                {
                    return Shuffles % DynamicVars[nameof(SHUFFLES)].IntValue;
                }
                return DynamicVars[nameof(SHUFFLES)].IntValue;
            }
        }

        private bool _isActivating;
        
        private bool IsActivating
        {
            get => _isActivating;
            set
            {
                AssertMutable();
                _isActivating = value;
                UpdateDisplay();
            }
        }

        private int _shuffles;
        
        [SavedProperty]
        private int Shuffles
        {
            get => _shuffles;
            set
            {
                AssertMutable();
                _shuffles = value;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (IsActivating)
            {
                Status = RelicStatus.Normal;
            }
            else
            {
                int intValue = DynamicVars[nameof(SHUFFLES)].IntValue;
                Status = (Shuffles % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal;
            }
            InvokeDisplayAmountChanged();
        }

        public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
            {
                return;
            }
            
            Shuffles++;
            int intValue = DynamicVars[nameof(SHUFFLES)].IntValue;
            if (CombatManager.Instance.IsInProgress && Shuffles % intValue == 0)
            {
                TaskHelper.RunSafely(DoActivateVisuals());
                await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
            }
        }

        private async Task DoActivateVisuals()
        {
            IsActivating = true;
            Flash();
            await Task.Delay(1000);
            IsActivating = false;
        }
    }
}