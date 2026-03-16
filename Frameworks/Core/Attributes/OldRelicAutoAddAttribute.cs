using System;

namespace UnderlyingLogicRelics.Frameworks.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OldRelicAutoAddAttribute(Type poolType) : Attribute
    {
        public Type PoolType
        {
            get;
            private set;
        } = poolType;
    }
}