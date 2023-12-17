using SML;
using UnityEngine;
using HarmonyLib;
using Home.Services;

namespace Main
{
    [Mod.SalemMod]
    public class Main
    {
        public static void Start()
            {
            Debug.Log("Working?"); 
            }
        
    }
    [HarmonyPatch(typeof(HomeLocalizationService), "RebuildStringTables")]
	public class StringTable
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002988 File Offset: 0x00000B88
		[HarmonyPostfix]
		public static void Postfix(HomeLocalizationService __instance)
		{
			__instance.stringTable_.Add("GUI_DOCK_PLAYER_NOTES", "Player Notes");
		}
	}
    [SML.DynamicSettings] 
    public class Settings{
        public ModSettings.CheckboxSetting MovableWills
        {
        get
        {
            ModSettings.CheckboxSetting MovableWills = new()
            {
                Name = "Movable Wills",
                Description = "Makes the wills, notepad and death note all movable if you grab them from the top part",
                DefaultValue = true,
                AvailableInGame = false,
                Available = true
            };
            return MovableWills;
            
        }
        }
        public ModSettings.CheckboxSetting OldControls
        {
        get
        {
            ModSettings.CheckboxSetting OldControls = new()
            {
                Name = "Use old controls",
                Description = "Instead of clicking on the top of the objects to move them, right click in any part of them to do it. If you are having problems moving your things, use this setting.",
                DefaultValue = false,
                AvailableInGame = false,
                Available = ModSettings.GetBool("Movable Wills","JAN.movablewills")
            };
            return OldControls;
            
        }
        }
        public ModSettings.CheckboxSetting RoughEstimates
        {
        get
        {
            ModSettings.CheckboxSetting RoughEstimates = new()
            {
                Name = "Use rough estimates",
                Description = "If you are having problems with the limits for moving your things, use this setting. This will only work with the \"Use old controls\" setting.",
                DefaultValue = false,
                AvailableInGame = false,
                Available = ModSettings.GetBool("Movable Wills","JAN.movablewills")
            };
            return RoughEstimates;
            
        }
        }
        
        
        
    }
}
