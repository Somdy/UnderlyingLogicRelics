using MegaCrit.Sts2.Core.Logging;
using UnderlyingLogicRelics.Frameworks.Core;

namespace UnderlyingLogicRelics.Frameworks.Utils
{
    public sealed class ORLog
    {
        public static void Info(string msg)
        {
            Log.Info($"{ModEntry.MOD_LOGGING}{msg}");
        }
    }
}