using System;
using BepInEx;
using BepInEx.Configuration;

namespace CustomMusic
{
    [BepInPlugin("com.dvize.CustomMusicPlayer", "dvize.CustomMusicPlayer", "1.1.0")]
    public class CustomMusicPlugin : BaseUnityPlugin
    {
        //add ConfigEntry<bool> enabled
        public static ConfigEntry<int> SongLoadAmount
        {
            get; set;
        }

        private void Awake()
        {
            SongLoadAmount = Config.Bind("Song Loading Configuration", "Song Load Amount", 5, new ConfigDescription("Sets the amount of songs to load on Game Start or Raid End. Yes it is randomized. More Songs = more time waiting",
                null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));

            new CustomMusicComponent().Enable();
            //new PlayerMethod3Patch().Enable();
            new EndScreenLoadPatch().Enable();
        }
    }
}
