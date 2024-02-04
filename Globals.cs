using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WinApp.Managers;
using WinApp.Sprites.Character;
using WinApp.Sprites.Skill;
using WinApp.States;

namespace WinApp;

public static class Globals {
    public static bool Clicked { get; private set; }
    public static readonly AudioManager AudioManager = new AudioManager();
    public static Game1 Game { get; set; }
    public static GameTime GameTime { get; private set; }
    public static SpriteFont Font { get; set; }
    public static SpriteFont BoldFont { get; set; }
    public static ContentManager Content { get; set; }
    public static GraphicsDevice GraphicsDevice { get; set; }
    public static MouseState MouseState { get; private set; }
    public static MouseState LastMouseState { get; set; }
    public static Rectangle MousePos { get; private set; }
    public static Rectangle PrevMousePos { get; private set; }
    public static Texture2D ObjectAtlas { get; set; }
    public static Texture2D HoveredAtlas { get; set; }
    public static Texture2D BattleBackground { get; set; }
    public static Texture2D GameBg1 { get; set; }
    public static Texture2D GameBg2 { get; set; }
    public static Texture2D GameBg3 { get; set; }
    public static Texture2D WalkBg1 { get; set; }
    public static Texture2D WalkBg2 { get; set; }
    public static Texture2D WalkBg3 { get; set; }
    public static Texture2D SkillAtlas { get; set; }
    public static Texture2D FireMonsterAtlas { get; set; }
    public static Texture2D WaterMonsterAtlas { get; set; }
    public static Texture2D NatureMonsterAtlas { get; set; }
    public static Texture2D WindMonsterAtlas { get; set; }
    public static Texture2D HudAtlas { get; set; }
    public static MenuState MenuState { get; set; }

    public static Dictionary<string, Dictionary<string, MonsterAttr>> MonsterDict; /* Element, MonsterName */
    public static Dictionary<string, Skill> SkillDict { get; set; }
    public static Dictionary<string, TexObject> HudDict { get; set; }
    public static Dictionary<string, TexObject> ObjectDict { get; set; }

    public static readonly List<Vector2> SkillPos = new List<Vector2> {
        new Vector2(561, 928),
        new Vector2(730, 928),
        new Vector2(899, 928),
        new Vector2(1068, 928)
    };
    public static readonly List<Vector2> IconPos = new List<Vector2> {
        new Vector2(30, 29),
        new Vector2(117, 29),
        new Vector2(204, 29),
        new Vector2(291, 29)
    };
    public static readonly List<int> XpList = new List<int> {
        0, 50, 125, 225, 500, 1250, 2500, 4000, 7000, 10000, 14000, 18000, 24000, 30000, 37000, 44000, 53000,
        63000, 73000, 95000, 107000, 125000, 143000, 163000, 183000, 203000, 225000, 250000, 275000, 300000
    };
    public async static Task DelayedAction(int milliSeconds, Action function) {
        await Task.Delay(milliSeconds);
        function();
    }
    public static void Update(GameTime gmTime) {
        GameTime = gmTime;
        LastMouseState = MouseState;
        MouseState = Mouse.GetState();

        Clicked = (MouseState.LeftButton == ButtonState.Pressed) && (LastMouseState.LeftButton == ButtonState.Released);
        MousePos = new Rectangle(MouseState.Position.X + 5, MouseState.Y - 15, 1, 1);
        PrevMousePos = new Rectangle(LastMouseState.Position.X + 5, LastMouseState.Y - 15, 1, 1);
    }
}