using System.Numerics;
using SML;
using UnityEngine;
using System;
using HarmonyLib;
using Server.Shared.State;
using Game.Interface;
using Services;

namespace NoClosing
{

    [HarmonyPatch(typeof(HudDockPanel), "ToggleLastWillClick")]
    class DontCloseWill{
        public static bool Prefix(HudDockPanel __instance){
            __instance.PlaySound("Audio/UI/ClickSound", false);
			if (Pepper.GetCurrentGamePhase() == GamePhase.PLAY)
			{
				Service.Game.Interface.IsLastWillOpen.Toggle();
			}
            GameObject dnd = GameObject.Find("Hud/LastWillElementsUI(Clone)/MainPanel/LastWillCommonElements");
            DragnDrop.lastTouched = dnd;
            return false;
        }
    }
    [HarmonyPatch(typeof(HudDockPanel), "ToggleDeathNoteClick")]
    class DontCloseDeathNote{
        public static bool Prefix(HudDockPanel __instance){
            __instance.PlaySound("Audio/UI/ClickSound", false);
			if (Pepper.GetCurrentGamePhase() == GamePhase.PLAY)
			{
				Service.Game.Interface.IsDeathNoteOpen.Toggle();
			}
            GameObject dnd = GameObject.Find("Hud/DeathNoteElementsUI(Clone)/MainPanel/DeathNoteCommonElements");
            DragnDrop.lastTouched = dnd;
            
            return false;
        }
    }
    [HarmonyPatch(typeof(HudDockPanel), "ToggleNotesClick")]
    class DontCloseNotes{
        public static bool Prefix(HudDockPanel __instance){
            __instance.PlaySound("Audio/UI/ClickSound", false);
			if (Pepper.GetCurrentGamePhase() == GamePhase.PLAY)
			{
				Service.Game.Interface.IsNotesOpen.Toggle();
			}
            GameObject dnd = GameObject.Find("Hud/NotepadElementsUI(Clone)/MainPanel/NotepadCommonElements");
            DragnDrop.lastTouched = dnd;
            return false;
        }
    }
}