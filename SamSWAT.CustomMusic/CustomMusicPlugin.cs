using BepInEx;

namespace SamSWAT.CustomMusic
{
    [BepInPlugin("com.samswat.custommusic", "SamSWAT.CustomMusic", "1.1.0")]
    public class CustomMusicPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new CustomMusicPatch().Enable();
        }
    }
}
