using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace WinApp.Managers;

public class AudioManager {
    private readonly Dictionary<string, Song> _songDict = new Dictionary<string, Song>();
    private readonly Dictionary<string, SoundEffectInstance> _soundEffectDict = new Dictionary<string, SoundEffectInstance>();

    public void AddSong(string name, Song song) {
        _songDict.Add(name, song);
    }
    public void AddSoundEffect(string name, SoundEffect soundEffect) {
        _soundEffectDict.Add(name, soundEffect.CreateInstance());
    }

    static private float CalcVolume(string type) {
        Settings settings = Settings.Instance;
        return type.ToLower() != "general" ? settings.General * settings.GetVolume(type) : settings.General;
    }

    public void StartSong(string type, string name) {
        MediaPlayer.Volume = CalcVolume(type);
        MediaPlayer.Play(_songDict[name]);
        if (!Settings.Instance.IsNotMuted(type))
            PauseSong();
    }

    public void StartSong(string type, string name, TimeSpan timeSpan) {
        MediaPlayer.Volume = CalcVolume(type);
        MediaPlayer.Play(_songDict[name], timeSpan);
        if (!Settings.Instance.IsNotMuted(type))
            PauseSong();
    }

    public static void ResumeSong() {
        MediaPlayer.Resume();
    }
    public static void SetRepeating(bool condition) {
        MediaPlayer.IsRepeating = condition;
    }
    public static void PauseSong() {
        MediaPlayer.Pause();
    }
    public static void StopSong() {
        MediaPlayer.Stop();
    }
    public static void SetVolume(string type) {
        MediaPlayer.Volume = CalcVolume(type);
    }
    public void StartSoundEffect(string type, string name) {
        if (!Settings.Instance.IsNotMuted(type))
            return;
        _soundEffectDict[name].Volume = CalcVolume(type);
        _soundEffectDict[name].Play();
    }
}
