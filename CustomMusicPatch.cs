using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using EFT.UI;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;


namespace CustomMusic
{
    public class CustomMusicComponent : ModulePatch
    {
        internal static string CurrDir
        {
            get; set;
        }
        internal static List<AudioClip> musicClips = new List<AudioClip>();
        internal static string[] files;
        internal CustomMusicComponent()
        {
            CurrDir = BepInEx.Paths.PluginPath + "/CustomMusic";
            files = Directory.GetFiles(CurrDir + "/music");
        }
        internal static void LoadAudio(string url)
        {
            using (UnityWebRequest web = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                Logger.LogInfo("url: " + url);
                var operation = web.SendWebRequest();
                while (!operation.isDone)
                {
                }

                if (!web.isNetworkError && !web.isHttpError)
                {
                    var clip = DownloadHandlerAudioClip.GetContent(web);
                    string audioclipname = url.Replace("file:///" + (CurrDir + "/music/").Replace("\\", "/"), "");
                    clip.name = audioclipname;
                    Logger.LogInfo("AudioClip: " + clip.name + ", state: " + clip.loadState);

                    musicClips.Add(clip);
                }
                else
                {
                    var Error = web.error;
                    Debug.LogErrorFormat("Can't load audio at path:'{0}', error:{1}", new object[]
                    {
                        url,
                        Error
                    });
                }
            }
        }
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GUISounds), "method_1");
        }

        [PatchPostfix]
        private static void PatchPostfix(ref AudioClip[] ___audioClip_0, object __instance)
        {
            //clear musicClips
            //musicClips.Clear();

            //pick 10 or max unique random songs from files and then LoadAudio for each
            var random = new System.Random();
            var randomSongs = files.OrderBy(x => random.Next()).Take(5).ToArray();

            foreach (var song in randomSongs)
            {
                Logger.LogInfo("random song: " + song);
                string url = "file:///" + song.Replace("\\", "/");
                LoadAudio(url);
            }

            //assume replacing original
            ___audioClip_0 = musicClips.ToArray();
        }
    }

    public class EndScreenLoadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TarkovApplication), "method_46");
        }

        [PatchPostfix]
        private static void PatchPostfix()
        {
            //clear musicClips
            CustomMusicComponent.musicClips.Clear();

            //pick 5 unique random songs from files and then LoadAudio for each
            var random = new System.Random();
            var randomSongs = CustomMusicComponent.files.OrderBy(x => random.Next()).Take(5).ToArray();
            foreach (var song in randomSongs)
            {
                Logger.LogInfo("random song: " + song);
                string url = "file:///" + song.Replace("\\", "/");
                CustomMusicComponent.LoadAudio(url);
            }

            //assume replacing original
            var guisounds = Singleton<GUISounds>.Instance;

            var audioclip0 = AccessTools.Field(typeof(GUISounds), "audioClip_0").GetValue(guisounds);
            audioclip0 = CustomMusicComponent.musicClips.ToArray();

            //force reload by invoking method_1 of guisounds
            Logger.LogInfo("Force Reload of all item sounds");
            var method1 = AccessTools.Method(typeof(GUISounds), "method_1").Invoke(guisounds, null);

        }
    }


}

