using BepInEx;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Text.RegularExpressions;
using GorillaNetworking;
using Photon.Pun;

namespace TooMuchInfo
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        void Start()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }


        static string CheckMods(VRRig rig)
        {
            string specialMods = "";
            NetPlayer creator = rig.Creator;

            Dictionary<string, string[]> specialModsList = new Dictionary<string, string[]> { 
                { "genesis", new string[] { "GENESIS", "07019C" } },
                { "HP_Left", new string[] { "HOLDABLEPAD", "332316" } },
                { "GrateVersion", new string[] { "GRATE", "707070" } },
                { "void", new string[] { "VOID", "FFFFFF" } }, 
                { "BANANAOS", new string[] { "BANANAOS", "FFFF00" } }, 
                { "GC", new string[] { "GORILLACRAFT", "43B581" } }, 
                { "CarName", new string[] { "GORILLAVEHICLES", "43B581" } }, 
                { "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", new string[] { "CCMV2", "BF00FC" } }, 
                { "cronos", new string[] { "CRONOS", "0000FF" } }, 
                { "ORBIT", new string[] { "ORBIT", "FFFFFF" } }, 
                { "Violet On Top", new string[] { "VIOLET", "DF6BFF" } }, 
                { "MP25", new string[] { "MONKEPHONE", "707070" } }, 
                { "GorillaWatch", new string[] { "GORILLAWATCH", "707070" } }, 
                { "InfoWatch", new string[] { "GORILLAINFOWATCH", "707070" } }, 
                { "BananaPhone", new string[] { "BANANAPHONE", "FFFC45" } }, 
                { "Vivid", new string[] { "VIVID", "F000BC" } }, 
                { "RGBA", new string[] { "CUSTOMCOSMETICS", "FF0000" } },
                { "cheese is gouda", new string[] { "WHOSICHEATING", "707070" } },
                { "I like cheese", new string[] { "RECROOMRIG", "FE8232" } } };

            foreach (KeyValuePair<string, string[]> specialMod in specialModsList)
            {
                if (creator.GetPlayerRef().CustomProperties.ContainsKey(specialMod.Key))
                    specialMods += (specialMods == "" ? "" : ", ") + "<color=#" + specialMod.Value[1] + ">" + specialMod.Value[0] + "</color>";
            }

            CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
            foreach (CosmeticsController.CosmeticItem cosmetic in cosmeticSet.items)
            {
                if (!cosmetic.isNullItem && !rig.concatStringOfCosmeticsAllowed.Contains(cosmetic.itemName))
                {
                    specialMods += (specialMods == "" ? "" : ", ") + "<color=green>COSMETX</color>";
                    break;
                }
            }

            return specialMods == "" ? null : specialMods;
        }



        static string GetPlatform(VRRig rig)
        {
            string concatStringOfCosmeticsAllowed = rig.concatStringOfCosmeticsAllowed;

            if (concatStringOfCosmeticsAllowed.Contains("S. FIRST LOGIN"))
                return "STEAM";
            else if (concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN") || rig.Creator.GetPlayerRef().CustomProperties.Count >= 2)
                return "PC";

            return "STANDALONE";
        }


        public static void UpdateName(VRRig rig)
        {
            try
            {
                string targetText = "Name";
                NetPlayer creator = rig.Creator;

                if (creator != null)
                {
                    List<string> lines = new List<string>
                    {
                        "",
                        "",
                        "",
                        creator.NickName,
                        "ID " + creator.UserId
                    };



                    string platform = GetPlatform(rig);
                    if (platform != null) lines.Add(platform);

                    string mods = CheckMods(rig);
                    if (mods != null) lines.Add("MODS " + mods);

                    targetText = string.Join("\n", lines);
                }

                Regex noRichText = new Regex("<.*?>");
                rig.playerText1.text = targetText;
                rig.playerText2.text = noRichText.Replace(targetText, "");
            } catch { }
        }
    }
}
