using System.Numerics;
using SML;
using UnityEngine;
using System;
using HarmonyLib;
using Utils;
using Game.Interface;
using Services;
using BMG.UI;

namespace ObjectsPatching
{
    [HarmonyPatch(typeof(DeathNoteCommonElementsPanel), "Start")]
    class MovableDeathNotePatch
    {
        public static void Postfix(DeathNoteCommonElementsPanel __instance)
        {
            if (__instance.gameObject.GetComponent<DragnDrop>() == null)
            {
                Func<bool> w;
                GameObject deathNote = __instance.gameObject;
                switch(deathNote.transform.parent.name)
                {
                    case "MainPanel":
                    w = () => Service.Game.Interface.IsDeathNoteOpen.Data;
                    break;
                    case "PlayerPopupElementsUI(Clone)":
                    w = () => __instance.canvasGroup.alpha != 0;
                    break;
                    default:
                    return;
                }
                DragnDrop a = deathNote.AddComponent<DragnDrop>();
                a.isVisible = w;
                a.xLength = 0.142f;
                a.marginY = 0.3f;
                a.yLength = 0.0331f;
                a.yOffset = 0.2668f;
                a.field = deathNote.GetComponentInChildren<BMG_InputField>();
            }
        }

    }
    [HarmonyPatch(typeof(NotepadPanel), "Start")]
    class MovableNotesPatch
    {
        public static void Postfix(NotepadPanel __instance)
        {
            if (__instance.gameObject.GetComponent<DragnDrop>() == null)
            {
                GameObject notepad = __instance.transform.GetChild(0).gameObject;
                DragnDrop a = notepad.AddComponent<DragnDrop>();
                a.isVisible = () => Service.Game.Interface.IsNotesOpen.Data;
                a.xLength = 0.14f;
                a.marginY = 0.3f;
                a.yLength = 0.033f;
                a.yOffset = 0.267f;
                a.field = notepad.transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<BMG_InputField>();
            }
        }

    }
    [HarmonyPatch(typeof(LastWillCommonElementsPanel), "Start")]
    class MovableWillPatch
    {
        public static void Postfix(LastWillCommonElementsPanel __instance)
        {
            if (__instance.gameObject.GetComponent<DragnDrop>() == null)
            {
                Func<bool> w;
                GameObject lastWill = __instance.gameObject;
                switch(lastWill.transform.parent.name)
                {
                    case "MainPanel":
                    w = () => Service.Game.Interface.IsLastWillOpen.Data;
                    break;
                    case "PlayerPopupElementsUI(Clone)":
                    w = () => __instance.canvasGroup.alpha != 0;
                    break;
                    default:
                    return;
                }
                DragnDrop a = lastWill.AddComponent<DragnDrop>();
                a.isVisible = w;
                a.xLength = 0.125f;
                a.marginY = 0.345f;
                a.yLength = 0.04f;
                a.yOffset = 0.315f;
                a.field = lastWill.GetComponentInChildren<BMG_InputField>();
            }
        }

    }
    [HarmonyPatch(typeof(LastWillPanel), "HandleOnDeselectInput")]
    class AutoSaveWillPatch
    {
        public static void Prefix(LastWillPanel __instance)
        {
            if(ModSettings.GetBool("Auto-save will")){
            __instance.SaveWill();
            }
        }

    }
        
}
