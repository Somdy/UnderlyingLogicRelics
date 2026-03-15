using System;
using MegaCrit.Sts2.Core.Entities.Players;

namespace UnderlyingLogicRelics.Frameworks.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OldRelicsAncientPoolAttribute(AncientPoolType ancientType) : Attribute
    {
        public AncientPoolType AncientType
        {
            get;
            private set;
        } = ancientType;
    }

    [Flags]
    public enum AncientPoolType
    {
        DARV,
    }
}