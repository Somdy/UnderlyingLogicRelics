using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using UnderlyingLogicRelics.Frameworks.Core;
using UnderlyingLogicRelics.Frameworks.Core.Attributes;
using UnderlyingLogicRelics.Frameworks.Models.Bases;
using UnderlyingLogicRelics.Frameworks.Patches.Content.Ancients;
using UnderlyingLogicRelics.Frameworks.Utils;
// ReSharper disable InconsistentNaming

namespace UnderlyingLogicRelics.Frameworks.Patches.Content
{
    public class RelicIdFix
    {
        private static bool s_netIdAssigned = false;
        
        [HarmonyPatch(typeof(SharedRelicPool), "GenerateAllRelics")]
        public static class ManualAssignId
        {
            static void Postfix(SharedRelicPool __instance)
            {
                if (!s_netIdAssigned)
                {
                    s_netIdAssigned = true;
                    foreach (Type t in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsAssignableTo(typeof(AbstractModel))
                                 && !t.IsAbstract
                                 && t.GetInterface(nameof(IOldRelicModel)) != null))
                    {
                        assignNetId(t);
                        assignAncientPool(t);
                    }
                }
            }
            
            private static void assignNetId(Type type)
            {
                if (!type.IsAssignableTo(typeof(OldRelicModelBase)))
                {
                    return;
                }
                ORLog.Info($"Assigning model {type.Name} netId");
                Dictionary<ModelId, AbstractModel> _contentById = AccessTools.StaticFieldRefAccess<Dictionary<ModelId, AbstractModel>>
                    (typeof(ModelDb), "_contentById");
                _contentById[ModelDb.GetId(type)] = ModelDb.AllRelics.First(r => r.GetType() == type);
                Dictionary<string, int> _catNameToNetIdMap = AccessTools.StaticFieldRefAccess<Dictionary<string, int>>
                    (typeof(ModelIdSerializationCache), "_categoryNameToNetIdMap");
                _catNameToNetIdMap[ModelDb.GetId(type).Category] = _catNameToNetIdMap.Count;
            
                Dictionary<string, int> _entryNameToNetIdMap = AccessTools.StaticFieldRefAccess<Dictionary<string, int>>
                    (typeof(ModelIdSerializationCache), "_entryNameToNetIdMap");
                _entryNameToNetIdMap[ModelDb.GetId(type).Entry] = _entryNameToNetIdMap.Count;
            
                List<string> _netIdToCatNameMap = AccessTools.StaticFieldRefAccess<List<string>>
                    (typeof(ModelIdSerializationCache), "_netIdToCategoryNameMap");
                _netIdToCatNameMap.Add(ModelDb.GetId(type).Category);
            
                List<string> _netIdToEntryNameMap = AccessTools.StaticFieldRefAccess<List<string>>
                    (typeof(ModelIdSerializationCache), "_netIdToEntryNameMap");
                _netIdToEntryNameMap.Add(ModelDb.GetId(type).Entry);
            }

            private static void assignAncientPool(Type type)
            {
                if (type.GetCustomAttribute<OldRelicAncientPoolAttribute>(false) is not { } ancientAttr)
                {
                    return;
                }

                RelicModel r = ModelDb.GetById<RelicModel>(ModelDb.GetId(type));
                AncientPoolType ancientType = ancientAttr.AncientType;
                if (ancientType.HasFlag(AncientPoolType.DARV))
                {
                    DarvRelicPatch.RelicsToDarv.Add(new ValidRelic
                    {
                        match = (pool, player) => r is IOldRelicModel or && or.AncientAllow(pool, player),
                        relic = r
                    });
                }
            }
        }
    }
}