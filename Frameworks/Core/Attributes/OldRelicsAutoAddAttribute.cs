using System;

namespace UnderlyingLogicRelics.Frameworks.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OldRelicsAutoAddAttribute(Type poolType) : Attribute
    {
        public Type PoolType
        {
            get;
            private set;
        } = poolType;
    }
}