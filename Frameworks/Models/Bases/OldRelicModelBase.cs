using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using UnderlyingLogicRelics.Frameworks.Core;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Patches.Content.Ancients;
using UnderlyingLogicRelics.Frameworks.Utils;

namespace UnderlyingLogicRelics.Frameworks.Models.Bases
{
    public abstract class OldRelicModelBase : RelicModel, IOldRelicModel
    {
        public override string PackedIconPath => $"res://Content/Images/Relics/{_imageId}.png";
        protected override string PackedIconOutlinePath => $"res://Content/Images/Relics/{_imageId}.png";
        protected override string BigIconPath => $"res://Content/Images/Relics/{_imageId}.png";

        public override RelicRarity Rarity => RelicRarity.Event;

        private readonly string _imageId;

        protected OldRelicModelBase()
        {
            _imageId = GetType().Name;
            ORLog.Info($"Creating model instance {GetType()}");
        }

        public virtual bool AncientAllow(AncientPoolType ancient, Player p) => true;
    }
}