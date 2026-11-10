using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    /// <summary>Music and sound effects, loaded from Resources/Audio (made by tools/audio/make_audio.py).</summary>
    public static class Audio
    {
        static AudioSource music, sfx;
        static readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        static string current;
        public static bool Muted { get; private set; }

        public static void Init(GameObject host)
        {
            if (music != null) return;
            music = host.AddComponent<AudioSource>();
            music.playOnAwake = false; music.loop = true; music.volume = 0.5f;
            sfx = host.AddComponent<AudioSource>();
            sfx.playOnAwake = false; sfx.volume = 0.9f;
        }

        static AudioClip Clip(string path)
        {
            AudioClip c;
            if (clips.TryGetValue(path, out c)) return c;
            c = Resources.Load<AudioClip>("Audio/" + path);
            if (c == null) Debug.LogWarning("Tapestry: missing sound Resources/Audio/" + path);
            clips[path] = c;
            return c;
        }

        /// <summary>Start a tune (does nothing if it is already playing).</summary>
        public static void Music(string name, bool loop = true)
        {
            if (music == null) return;
            if (current == name && music.isPlaying) return;
            current = name;
            music.clip = Clip("music/" + name);
            music.loop = loop;
            if (music.clip != null) music.Play();
        }

        public static void MusicPitch(float p) { if (music != null) music.pitch = p; }
        public static void PauseMusic() { if (music != null) music.Pause(); }
        public static void ResumeMusic() { if (music != null) music.UnPause(); }

        public static void Play(string name, float volume = 1f)
        {
            if (sfx == null) return;
            var c = Clip("sfx/" + name);
            if (c != null) sfx.PlayOneShot(c, volume);
        }

        public static void ToggleMute()
        {
            Muted = !Muted;
            AudioListener.volume = Muted ? 0f : 1f;
        }
    }
}
