using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace CustomMusic
{
    [BepInPlugin("com.dvize.CustomMusicPlayer", "dvize.CustomMusicPlayer", "1.3.0")]
    public class CustomMusicPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<int> SongLoadAmount
        {
            get; set;
        }

        private void Awake()
        {
            SongLoadAmount = Config.Bind(
                "Song Loading Configuration", 
                "Song Load Amount", 
                5, 
                new ConfigDescription("Sets the amount of songs to load on Game Start or Raid End. Yes it is randomized. More Songs = more time waiting",
                null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));

            new CustomMusicComponent().Enable();
            new EndScreenLoadPatch().Enable();
            new HideoutScreenLoadPatch().Enable();

        }
    }
}
