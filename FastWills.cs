using System.Collections.Generic;
using System.Numerics;
using SML;
using UnityEngine;
using System;
using HarmonyLib;
using Server.Shared.State;
using Game.Interface;
using Services;
using ObjectsPatching;

namespace FastWills
{
    [HarmonyPatch(typeof(WhoDiedAndHowPanel), "HandleSubphaseRole")]
    class DetectDeadPerson{
        public static List<int> deathNumbers = new();
        public static void Postfix(WhoDiedAndHowPanel __instance){
            if(!deathNumbers.Contains(__instance.currentPlayerNumber))deathNumbers.Add(__instance.currentPlayerNumber);
        }
    }
    [HarmonyPatch(typeof(PlayerPopupController), "InitializeLastWillPanel")]
    class ShowWill{ // affects death note too since this goes before it.
        public static void Prefix(PlayerPopupController __instance){
            if(!Pepper.IsGamePhasePlay()) return;
            if(!__instance.m_discussionPlayerState.alive) return;
            if(ModSettings.GetBool("Show dead player's will fast", "JAN.movablewills") && DetectDeadPerson.deathNumbers.Contains(__instance.m_discussionPlayerState.position)) __instance.m_discussionPlayerState.alive = false;
        }
    }
    /*[HarmonyPatch(typeof(PlayerPopupController), "InitializeDeathNotePanel")]
    class ShowDeathNote{
        public static void Prefix(PlayerPopupController __instance){
            if(!Pepper.IsGamePhasePlay()) return;
            if(!__instance.m_discussionPlayerState.alive) return;
            if(ModSettings.GetBool("Show dead player's will fast", "JAN.movablewills") && DetectDeadPerson.deathNumbers.Contains(__instance.m_discussionPlayerState.position)) __instance.m_discussionPlayerState.alive = false;
        }
    }*/
}