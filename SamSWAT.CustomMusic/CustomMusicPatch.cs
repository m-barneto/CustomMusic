using Aki.Reflection.Patching;
using EFT.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using BepInEx;

namespace SamSWAT.CustomMusic
{
    public class CustomMusicPatch : ModulePatch
    {
        private static Dictionary<string, bool> ReplaceOriginal;
        private static string CurrDir { get; set; }

        private static List<AudioClip> musicClips = new List<AudioClip>();

        public CustomMusicPatch()
        {
            CurrDir = BepInEx.Paths.PluginPath + "/SamSWAT.CustomMusic";
            string json = new StreamReader(Path.Combine(CurrDir, "config.json")).ReadToEnd();
            ReplaceOriginal = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);

            string[] files = Directory.GetFiles(CurrDir + "/music");

            foreach (var song in files)
            {
                Logger.LogInfo(song);
                string url = "file:///" + song.Replace("\\", "/");
                if (song.Contains(".mp3"))
                    LoadAudio(url, AudioType.MPEG);
                else if (song.Contains(".wav"))
                    LoadAudio(url, AudioType.WAV);
                else if (song.Contains(".ogg"))
                    LoadAudio(url, AudioType.OGGVORBIS);
                else
                    LoadAudio(url, AudioType.UNKNOWN);
            }
        }

        private async void LoadAudio(string url, AudioType audioType)
        {
            using (UnityWebRequest web = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                Logger.LogInfo("url: " + url);
                var operation = web.SendWebRequest();
                while (!operation.isDone)
                {
                    Logger.LogInfo("operation pending..");
                    await Task.Yield();
                }
                    
                if (!web.isNetworkError && !web.isHttpError)
                {
                    var clip = DownloadHandlerAudioClip.GetContent(web);
                    Logger.LogInfo("AudioClip: " + clip.loadState + " " + clip.frequency);
                    string audioclipname = url.Replace("file:///"+ (CurrDir + "/music/").Replace("\\","/"), "");
                    clip.name = audioclipname;
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
            return typeof(GUISounds).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(ref AudioClip[] ___audioClip_0, object __instance)
        {
            // Shuffle the musicClips list
            Shuffle(musicClips);

            if (ReplaceOriginal["ReplaceOriginalMusic"])
                ___audioClip_0 = musicClips.ToArray();
            else
            {
                ___audioClip_0 = ___audioClip_0.Concat(musicClips).ToArray();
            }

            foreach (var clip in musicClips)
            {
                Logger.LogInfo("clip in array: " + clip.name);
            }

            foreach (var clip2 in ___audioClip_0)
            {
                Logger.LogInfo("\n \n \n" + "original clips: " + clip2.name);
            }
        }

        private static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
