using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Relics;
using UnderlyingLogicRelics.Frameworks.Utils;

namespace UnderlyingLogicRelics.Frameworks.Core
{
    [ModInitializer("Initialize")]
    public class ModEntry
    {

        public const string MOD_ID = "Charone-OldRelics";
        public const string MOD_LOGGING = $"[{MOD_ID}]::";

        private static Harmony s_harmony;
        
        public static void Initialize()
        {
            autoRegisterModels();
            ModHelper.AddModelToPool<SharedRelicPool, RunicDome>();
            s_harmony = new Harmony("sts2.mod.charone.oldrelics");
            s_harmony.PatchAll();
        }

        private static void autoRegisterModels()
        {
            Dictionary<Type, Type> typeMap = new();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsAssignableTo(typeof(AbstractModel))
                         && !t.IsAbstract
                         && t.GetInterface(nameof(IOldRelicModel)) != null))
            {
                Type poolType = null;
                TypeInfo info = t.GetTypeInfo();
                if (info.GetCustomAttribute<OldRelicsAutoAddAttribute>(false) is { } poolAttr)
                {
                    poolType = poolAttr.PoolType;
                }
                if (poolType == null)
                {
                    continue;
                }
                typeMap[t] = poolType;
            }

            if (typeMap.Count == 0)
            {
                return;
            }
            
            foreach (KeyValuePair<Type, Type> e in typeMap)
            {
                ModHelper.AddModelToPool(e.Value, e.Key);
                ORLog.Info($"Added model {e.Key.Name} to pool {e.Value.Name}");
            }
        }
    }
}