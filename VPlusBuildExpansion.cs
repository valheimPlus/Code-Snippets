using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ValheimPlus.UI
{
    public class VPlusBuildExpansion
    {
        public static int newGridHeight = 10;
        public static int newGridWidth = 10;

        [HarmonyPatch(typeof(Hud), "Awake")]
        public static class Hud_Awake_Patch
        {
            public static void Prefix(ref Hud __instance)
            {
                DefaultControls.Resources uiRes = new DefaultControls.Resources();
                uiRes.standard = __instance.m_pieceCategoryRoot.transform.parent.GetChild(0).gameObject.GetComponent<Image>().sprite;
                Scrollbar myScroll = DefaultControls.CreateScrollbar(uiRes).GetComponent<Scrollbar>();
                myScroll.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 0f);
                myScroll.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                myScroll.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                myScroll.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                myScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 0);
                myScroll.direction = Scrollbar.Direction.BottomToTop;
                myScroll.gameObject.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
                myScroll.gameObject.transform.SetParent(__instance.m_pieceListRoot.transform.parent, false);
                ScrollRect testScroll = __instance.m_pieceListRoot.transform.parent.gameObject.AddComponent<ScrollRect>();
                testScroll.content = __instance.m_pieceListRoot;
                testScroll.viewport = __instance.m_pieceListRoot.transform.parent.gameObject.GetComponent<RectTransform>();
                testScroll.verticalScrollbar = myScroll;
                testScroll.movementType = ScrollRect.MovementType.Clamped;
                testScroll.inertia = false;
                testScroll.scrollSensitivity = __instance.m_pieceIconSpacing;
                testScroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                __instance.m_pieceListRoot.sizeDelta = new Vector2((int)(__instance.m_pieceIconSpacing * newGridWidth), (int)(__instance.m_pieceIconSpacing * newGridHeight));
            }
        }

        [HarmonyPatch(typeof(Hud), nameof(Hud.GetSelectedGrid))]
        public static class Hud_GetSelectedGrid_Transpiler
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                codes[0].operand = newGridWidth;
                codes[2].opcode = OpCodes.Ldc_I4_S;
                codes[2].operand = newGridHeight;
                return codes;
            }
        }

        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdatePieceList))]
        public static class Hud_UpdatePieceList_Transpiler
        {
            public static bool haveReanchored = false;
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                codes[3].operand = newGridWidth;
                codes[5].opcode = OpCodes.Ldc_I4_S;
                codes[5].operand = newGridHeight;
                return codes;
            }

            public static void Postfix(ref Hud __instance)
            {
                if (!haveReanchored)
                {
                    foreach (Transform pieceTrans in __instance.m_pieceListRoot.transform)
                    {
                        (pieceTrans as RectTransform).anchoredPosition = (pieceTrans as RectTransform).anchoredPosition +
                            new Vector2(
                                (-1 * (int)((__instance.m_pieceIconSpacing * newGridWidth) / 2)),
                                ((int)((__instance.m_pieceIconSpacing * newGridWidth) / 2)));
                    }
                    haveReanchored = true;
                }
            }
        }

        [HarmonyPatch(typeof(PieceTable), nameof(PieceTable.UpdateAvailable))]
        public static class PieceTable_UpdateAvailable
        {
            public static void Prefix(ref PieceTable __instance)
            {

            }
        }

        [HarmonyPatch(typeof(PieceTable), nameof(PieceTable.PrevCategory))]
        public static class PieceTable_PrevCategory
        {
            public static bool Prefix(ref PieceTable __instance)
            {
                if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PieceTable), nameof(PieceTable.NextCategory))]
        public static class PieceTable_NextCategory
        {
            public static bool Prefix(ref PieceTable __instance)
            {
                if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
