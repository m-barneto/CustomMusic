using BepInEx;

namespace CustomMusic
{
    [BepInPlugin("com.dvize.CustomMusicPlayer", "dvize.CustomMusicPlayer", "1.0.0")]
    public class CustomMusicPlugin : BaseUnityPlugin
    {
        //add ConfigEntry<bool> enabled
        private void Awake()
        {
            new CustomMusicComponent().Enable();
            //new PlayerMethod3Patch().Enable();
            new EndScreenLoadPatch().Enable();
        }
    }
}
