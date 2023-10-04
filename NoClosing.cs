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
            return false;
        }
    }
}