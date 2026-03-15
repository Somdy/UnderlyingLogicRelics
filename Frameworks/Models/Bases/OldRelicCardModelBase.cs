using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using UnderlyingLogicRelics.Frameworks.Core;

namespace UnderlyingLogicRelics.Frameworks.Models.Bases
{
    public abstract class OldRelicCardModelBase : CardModel, IOldRelicModel
    {
        public override string PortraitPath => $"res://Content/Images/Cards/{_imageId}.png";
        
        private readonly string _imageId;

        protected OldRelicCardModelBase(int canonicalEnergyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true) : base(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
        {
            _imageId = GetType().Name;
        }
    }
}