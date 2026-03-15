using MegaCrit.Sts2.Core.Entities.Players;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;

namespace UnderlyingLogicRelics.Frameworks.Core
{
    public interface IOldRelicModel
    {
         bool AncientAllow(AncientPoolType ancient, Player p) => true;
    }
}