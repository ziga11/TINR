using System;

namespace WinApp;

/* Pri TPO smo omenjali Singleton-a pa sem hotel testirat kaj je to in kako deluje,
   malo je scuffed zaradi tega, ker ne mores setirati lepo direkt iz JSON-a --> not good use case IK */
public sealed class Settings {
    public float General { get; set; }
    public float Battle { get; set; }
    public float Gameplay { get; set; }
    public float Skill { get; set; }
    public bool NotMutedAll { get; set; }
    public bool NotMutedBattle { get; set; }
    public bool NotMutedSkill { get; set; }
    
    static private readonly Lazy<Settings> _instance = new Lazy<Settings> (() => new Settings());
    public static Settings Instance => _instance.Value;
    public float GetVolume(string type) {
        return type.ToLower() switch {
            "general" => General,
            "battle" => Battle,
            "gameplay" => Gameplay,
            "skill" => Skill,
            _ => 0f
        };
    }

    public bool IsNotMuted(string type) {
        return type.ToLower() switch {
            "general" => NotMutedAll,
            "battle" => NotMutedBattle,
            "skill" => NotMutedSkill,
            _ => false
        };
    }

    private Settings() { }
}