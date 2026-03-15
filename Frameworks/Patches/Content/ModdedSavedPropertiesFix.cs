using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using UnderlyingLogicRelics.Frameworks.Utils;

namespace UnderlyingLogicRelics.Frameworks.Patches.Content
{
    public class ModdedSavedPropertiesFix
    {
        [HarmonyPatch(typeof(SavedPropertiesTypeCache), MethodType.StaticConstructor)]
        public static class AddRelicFix
        {
            static void Postfix()
            {
                var types = Assembly.GetExecutingAssembly().GetTypes()
                                       .Where(t => t.IsAssignableTo(typeof(AbstractModel)) && !t.IsAbstract)
                                       .Select(t => t)
                                       .ToList();
                types.Sort((t1, t2) => string.Compare(t1.Name, t2.Name, StringComparison.Ordinal));
                foreach (Type t in types)
                {
                    ORLog.Info($"Caching properties of type {t}");
                    AccessTools.Method(typeof(SavedPropertiesTypeCache), "CachePropertiesForType", [typeof(Type)])
                               .Invoke(null, [t]);
                }

                List<string> _netIdToPropertyNameMap = AccessTools.StaticFieldRefAccess<List<string>>
                    (typeof(SavedPropertiesTypeCache), "_netIdToPropertyNameMap");
                AccessTools.PropertySetter(typeof(SavedPropertiesTypeCache), "NetIdBitSize")
                           .Invoke(null, [ Mathf.CeilToInt(Math.Log2(_netIdToPropertyNameMap.Count)) ]);
            }
        }
    }
}