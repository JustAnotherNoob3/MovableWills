using System.Numerics;
using SML;
using UnityEngine;
using System;
using HarmonyLib;
using Utils;
using Game.Interface;
using Services;
using BMG.UI;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using UnityEngine.UI;
using Home.Common.Dialog;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.EventSystems;
using Home.Common;
using Home.Services;
using System.Linq;
using System.Collections;

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
            if (__instance.transform.GetChild(0).gameObject.GetComponent<DragnDrop>() != null) return;
            if(__instance.gameObject.name == "asd") return;
            GameObject notepad = __instance.transform.GetChild(0).gameObject;
            DragnDrop a = notepad.AddComponent<DragnDrop>();
            a.isVisible = () => Service.Game.Interface.IsNotesOpen.Data;
            a.xLength = 0.14f;
            a.marginY = 0.3f;
            a.yLength = 0.033f;
            a.yOffset = 0.267f;
            a.field = notepad.transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<BMG_InputField>();
            if(ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills")){
                PlayerNotesController.open = false;
                notepad.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
                GameObject me = UnityEngine.Object.Instantiate(__instance.gameObject,__instance.transform.parent);
                me.name = "asd";
                DragnDrop drop = me.transform.GetChild(0).GetComponent<DragnDrop>();
                drop.isVisible = () => PlayerNotesController.open == true;
                drop.xLength = 0.12f;
                drop.marginY = 0.255f;
                drop.yLength = 0.03f;
                drop.yOffset = 0.207f;
                me.transform.SetAsFirstSibling();
                me.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                me.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                if(ModSettings.GetBool("Auto-hide dead players", "JAN.movablewills"))
                __instance.StartCoroutine(stupidCoroutineGameMakesMeDo(me.GetComponent<NotepadPanel>()));
            } else if(ModSettings.GetBool("Auto-hide dead players", "JAN.movablewills")){
                __instance.StartCoroutine(stupidCoroutineGameMakesMeDo(__instance));
            }
        }
        private static IEnumerator stupidCoroutineGameMakesMeDo(NotepadPanel panel)
        {
            yield return null;
            yield return null;
            yield return null;
            panel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetComponent<ToggleCustomClick>().isOn = true;
            panel.OnHideDeadPlayersEnabledChanged(true);
        }
    }
    [HarmonyPatch(typeof(NotepadPanel), "ValidateVisible")]
    class h{
        public static bool Prefix(NotepadPanel __instance){
            if(__instance.gameObject.name != "asd") return true;
            if(PlayerNotesController.open){
				__instance.canvasGroup.EnableRenderingAndInteraction();
				__instance.AddMask();
				return false;
			}
			__instance.canvasGroup.DisableRenderingAndInteraction();
			__instance.RemoveMask();
            return false;
        }
    }
    [HarmonyPatch(typeof(LastWillCommonElementsPanel), "Start")]
    class MovableWillPatch
    {
        public static void Postfix(LastWillCommonElementsPanel __instance)
        {
            FastWills.DetectDeadPerson.deathNumbers = new();
            if (__instance.gameObject.GetComponent<DragnDrop>() == null)
            {
                Func<bool> w;
                LastWillPanel z = null;
                GameObject lastWill = __instance.gameObject;
                switch(lastWill.transform.parent.name)
                {
                    case "MainPanel":
                    w = () => Service.Game.Interface.IsLastWillOpen.Data;
                    z = lastWill.transform.parent.GetComponent<LastWillPanel>();
                    break;
                    case "PlayerPopupElementsUI(Clone)":
                    w = () => __instance.canvasGroup.alpha != 0;
                    break;
                    default:
                    return;
                }
                DragnDrop a = lastWill.AddComponent<DragnDrop>();
                a.a = z;
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
            if(ModSettings.GetBool("Auto-save will", "JAN.movablewills")){
            __instance.SaveWill();
            }
        }

    }
    [HarmonyPatch(typeof(NotepadPlayerListPlayerItem), "Update")]
    class KEK{
        public static void Postfix(NotepadPlayerListPlayerItem __instance){
            if(Pepper.CheckIfThisIsMe(__instance._characterPosition)){if(!ModSettings.GetBool("Hide self from Player Notes","JAN.movablewills")) return;
            __instance.gameObject.SetActive(false);}
            else if(__instance._characterPosition < 0) {__instance.gameObject.SetActive(false);return;}
        }
    }
    [HarmonyPatch(typeof(NotepadPanel), "HandleClickTogglePlayerNotes")]
    class PlayerNotesController{
        public static bool open = false;
        public static bool Prefix(NotepadPanel __instance){
            if(!ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills"))return true;
            if(__instance.gameObject.name != "asd") return true;
            open = !open;
            __instance.ValidateVisible();
            return false;
        }
        public static void OpenNClose(){
            GameObject notes = GameObject.Find("Hud/NotepadElementsUI(Clone)/asd");
            NotepadPanel notepadPanel=notes.GetComponent<NotepadPanel>();
            notepadPanel.HandleClickTogglePlayerNotes();
            notepadPanel.PlaySound("Audio/UI/ClickSound", false);
            if(ModSettings.GetBool("Movable Wills", "JAN.movablewills")) return;
            Service.Game.Interface.IsLastWillOpen.Set(false);
			Service.Game.Interface.IsDeathNoteOpen.Set(false);
			Service.Game.Interface.IsNotesOpen.Set(false);
        }
        public static void ForceClose(){
            GameObject notes = GameObject.Find("Hud/NotepadElementsUI(Clone)/asd");
            NotepadPanel notepadPanel=notes.GetComponent<NotepadPanel>();
            open = true;
            notepadPanel.HandleClickTogglePlayerNotes();
        }
    }
    [HarmonyPatch(typeof(HudDockPanel), "LoadAndOrderDockItems")] //stolen straight from Curtis
public class CasualModeDockItem
{
    [HarmonyPostfix]
    public static void Postfix(HudDockPanel __instance)
    {
        if(!ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills"))return;
        if(!Pepper.IsGamePhasePlay()) return;
        HudDockItem dockItem = UnityEngine.Object.Instantiate(__instance.dockItems.Find(x => x.gameObject.name == "NotesSlot")).GetComponent<HudDockItem>();

        dockItem.transform.SetParent(__instance.transform);
        dockItem.transform.localScale = new(1,1,1);
        dockItem.transform.SetSiblingIndex(__instance.dockItems.IndexOf(__instance.dockItems.Find(x => x.gameObject.name == "NotesSlot"))+1);
        dockItem.transform.GetChild(0).GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        dockItem.canMove = false;
        dockItem.dockItemType = HudDockItem.DockItemType.GAME_ANY;
        dockItem.name = "PlayerNotesSlot";
        dockItem.localizationKey = "GUI_DOCK_PLAYER_NOTES";
        dockItem.label.text = "Player Notes";
        dockItem.dockFunctionType = DockFunctionType.NONE;
        dockItem.hotkeyType = InputService.HotKey.HotKeyType.NONE;
        dockItem.eventTriggers.triggers.Clear();
        dockItem.GetComponentInChildren<Image>().color = new Color(0.40f,0.17f,0);
        var eventTrigger = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        eventTrigger.callback.AddListener((s) => PlayerNotesController.OpenNClose());

        dockItem.eventTriggers.triggers.Add(eventTrigger);
    }
}
}
