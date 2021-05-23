using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace ShaderAnalysis
{
    [BepInPlugin(ID, title, version)]
    public class ShaderAnalysisMod : BaseUnityPlugin
    {
        public const string ID = "mixone.valheim.shaderanalysis";
        public const string version = "0.0.0.1";
        public const string title = "Shader Analysis";

        public Harmony harmony;

        public static BepInEx.Logging.ManualLogSource harmonyLog;
        public static GameObject intellion;

        public void Awake()
        {
            harmony = new Harmony(ID);
            harmony.PatchAll();
            harmonyLog = Logger;

            harmonyLog.LogDebug("Shader Analysis loaded.");
        }
    }

    #region Utils
    public static class Utils
    {
        public static void OutputStructure(Transform trans, int level=0)
        {
            if(level == 0)
            {
                ShaderAnalysisMod.harmonyLog.LogDebug($"\nParent : {trans.name}");
            }
            ShaderAnalysisMod.harmonyLog.LogDebug($"\n{String.Concat(Enumerable.Repeat("-", level + 1))}> {trans.name}");
            if (trans.GetComponents<Component>().Length > 1)
            {
                ShaderAnalysisMod.harmonyLog.LogDebug($"\n{String.Concat(Enumerable.Repeat("-", level + 1))}> {trans.name} Components :");
                foreach (Component comp in trans.GetComponents<Component>())
                {                    
                    ShaderAnalysisMod.harmonyLog.LogDebug($"\n{String.Concat(Enumerable.Repeat(" ", level + 2))} {comp.GetType()}");
                }
            } else
            {
                ShaderAnalysisMod.harmonyLog.LogDebug($"\n{String.Concat(Enumerable.Repeat("-", level + 1))}> {trans.name} has no components.");
            }
            foreach (Transform child in trans)
            {
                OutputStructure(child, level+1);
            }
        }
    }

    #endregion

    #region Patches 

    public static class FejdStartup_Patches
    {
        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Awake))]
        public static class FejdStartup_Awake_Patch
        {
            public static void Postfix(ref FejdStartup __instance)
            {
                ItemDrop[] allItems = Resources.FindObjectsOfTypeAll<ItemDrop>();
                List<Shader> foundShaders = new List<Shader>();
                ShaderAnalysisMod.harmonyLog.LogDebug("\n########## Item Data ##########");
                foreach (string itemType in Enum.GetNames(typeof(ItemDrop.ItemData.ItemType)))
                {
                    ShaderAnalysisMod.harmonyLog.LogDebug($"\n##### {itemType} Items #####");
                    foreach (ItemDrop item in allItems.Where<ItemDrop>(itm => itm.m_itemData.m_shared.m_itemType == (ItemDrop.ItemData.ItemType)Enum.Parse(typeof(ItemDrop.ItemData.ItemType), itemType)))
                    {
                        ShaderAnalysisMod.harmonyLog.LogDebug($"\n### { Localization.instance.Localize(item.m_itemData.m_shared.m_name)} ###");
                        ShaderAnalysisMod.harmonyLog.LogDebug($"\nItem keyword : {item.m_itemData.m_shared.m_name}");
                        if (item.m_itemData.m_shared.m_variants > 0)
                        {
                            ShaderAnalysisMod.harmonyLog.LogDebug("\nVariants: { item.m_itemData.m_shared.m_variants}");
                        } else
                        {
                            ShaderAnalysisMod.harmonyLog.LogDebug("\nItem has no variants");
                        }
                        if (item.gameObject.GetComponentInChildren<Renderer>(true) != null)
                        {
                            foreach (Renderer rendy in item.gameObject.GetComponentsInChildren<Renderer>(true)) {
                                ShaderAnalysisMod.harmonyLog.LogDebug($"Shader : {rendy.material.shader.name}");
                                if (!foundShaders.Contains(rendy.material.shader))
                                    foundShaders.Add(rendy.material.shader);
                            }
                        } else
                        {
                            ShaderAnalysisMod.harmonyLog.LogDebug($"Item has no Renderer");
                        }
                        ShaderAnalysisMod.harmonyLog.LogDebug($"\n# GameObject Structure #");
                        Utils.OutputStructure(item.gameObject.transform);
                    }
                }
                ShaderAnalysisMod.harmonyLog.LogDebug("\n########## Shader Data ##########");
                foreach (Shader shade in foundShaders)
                {
                    ShaderAnalysisMod.harmonyLog.LogDebug($"\n{shade.name} Properties :");
                    for (int i = 0; i < shade.GetPropertyCount(); i++)
                    {
                        ShaderAnalysisMod.harmonyLog.LogDebug($"-> {shade.GetPropertyName(i)}");
                        ShaderAnalysisMod.harmonyLog.LogDebug($"-> {shade.GetPropertyDescription(i)}");
                        if (shade.GetPropertyAttributes(i).Length > 0)
                        {
                            ShaderAnalysisMod.harmonyLog.LogDebug($"-> {shade.GetPropertyName(i)} Property Attributes:");
                            foreach (string attr in shade.GetPropertyAttributes(i))
                            {
                                ShaderAnalysisMod.harmonyLog.LogDebug($"--> {attr}");
                            }
                        } else
                        {
                            ShaderAnalysisMod.harmonyLog.LogDebug($"-> {shade.GetPropertyName(i)} property has no attributes.");
                        }
                    }
                }
            }
        }

    }

    #endregion
}
