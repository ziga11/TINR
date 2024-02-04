using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Managers;
using WinApp.Sprites;
using WinApp.Sprites.UIElem;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace WinApp.States;

public class MenuState : State {
    private readonly Dictionary<string, UiManager> _uiManagers = new Dictionary<string, UiManager> {
            ["Start"] = new UiManager(),
            ["Options"] = new UiManager(),
            ["Sound"] = new UiManager(),
    };
    private readonly List<Vector2> _positions = new List<Vector2> {
            new Vector2(799, 317),
            new Vector2(799, 498),
            new Vector2(799, 682)
    };

    private readonly Stack<string> _sectors = new Stack<string>();
    public List<Sprite> Backgrounds = new List<Sprite>();
    public State PrevState;
    public MenuState(Game1 game1, ContentManager content) : base(game1, content) {
        _sectors.Push("Start");
        
        LoadButtons();
        LoadSliders();
        LoadRadio();
        
        foreach (string sector in _uiManagers.Keys) {
            SetText(sector);
            _uiManagers[sector].AddButton("XExit", new Vector2(1073, 235), Globals.HudAtlas, Globals.HudDict["XExit"].Rect)
                    .OnClick += (_, _) => Globals.DelayedAction(150, () => Game1.SetState(PrevState));
            if (sector == "Start")
                continue;
            
            _uiManagers[sector].AddButton("PrevSector", new Vector2(783, 235), Globals.HudAtlas, Globals.HudDict["BackArrow"].Rect)
                    .OnClick += (_, _) => _sectors.Pop();
        }
    }

    private void LoadButtons() {
        Button button = _uiManagers["Start"].AddButton("Resume", _positions[0], Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, new Vector2(2));
        button.OnClick += (_, _) => Globals.DelayedAction(150, () => Game1.SetState(PrevState));
        button.Layer = 0.2f;
        button = _uiManagers["Start"].AddButton("Options", _positions[1], Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, new Vector2(2));
        button.OnClick += (_, _) => _sectors.Push("Options");
        button.Layer = 0.2f;

        button = _uiManagers["Options"].AddButton("Sound", _positions[0], Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, new Vector2(2));
        button.OnClick += (_, _) => _sectors.Push("Sound");
        button.Layer = 0.2f;
    }
    private void LoadSliders() {
        Sprite sliderGen = new Sprite(Globals.HudAtlas, Globals.HudDict["Slider"].Rect, Vector2.Zero);
        Sprite boundsGen = new Sprite(Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, Vector2.Add(_positions[0], new Vector2(0, 40)), new Vector2(2, 0.5f));
        Slider volume = _uiManagers["Sound"].AddSlider("General Volume", sliderGen, boundsGen, true);
        volume.OnMove += (_, _) => {
            Rectangle intersectArea = volume.BoundsSprite.IntersectArea;
            Sprite slider = volume.SliderSprite;
            float newVolume = (volume.SliderSprite.Position.X - intersectArea.X + (float) slider.IntersectArea.Width / 2) / intersectArea.Width;
            Settings.Instance.General = newVolume;

            _uiManagers["Sound"].TextElemDict["General Volume"].Text = $"General Volume: {(int) (newVolume * 100)}";
            if (PrevState is GameplayState)
                AudioManager.SetVolume("General");
        };
        float xPos = (1f - Settings.Instance.General) * volume.BoundsSprite.IntersectArea.Width;
        volume.SliderSprite.Position = Vector2.Subtract(volume.SliderSprite.Position, new Vector2(xPos, 0));
        
        Sprite sliderBattle = new Sprite(Globals.HudAtlas, Globals.HudDict["Slider"].Rect, Vector2.Zero);
        Sprite boundsBattle = new Sprite(Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, _positions[1], new Vector2(2, 0.5f));
        Slider battleVolume = _uiManagers["Sound"].AddSlider("Battle Volume", sliderBattle, boundsBattle, true);
        battleVolume.OnMove += (_, _) => {
            Rectangle intersectArea = battleVolume.BoundsSprite.IntersectArea;
            Sprite slider = battleVolume.SliderSprite;
            float newVolume = (slider.Position.X - intersectArea.X + (float) slider.IntersectArea.Width / 2) / intersectArea.Width;
            Settings.Instance.Battle = Settings.Instance.General * newVolume;
            
            _uiManagers["Sound"].TextElemDict["Battle Volume"].Text = $"Battle Volume: {(int) (newVolume * 100)}";
            if (PrevState is BattleState)
                AudioManager.SetVolume("Battle");
        };
        xPos = (1f - Settings.Instance.Battle) * battleVolume.BoundsSprite.IntersectArea.Width;
        battleVolume.SliderSprite.Position = Vector2.Subtract(battleVolume.SliderSprite.Position, new Vector2(xPos, 0));
        
        Sprite sliderSkill = new Sprite(Globals.HudAtlas, Globals.HudDict["Slider"].Rect, Vector2.Subtract(_positions[2], new Vector2(0, 40)));
        Sprite boundsSkill = new Sprite(Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, Vector2.Subtract(_positions[2], new Vector2(0, 40)), new Vector2(2, 0.5f));
        Slider skillVolume = _uiManagers["Sound"].AddSlider("Skill Volume", sliderSkill, boundsSkill, true);
        skillVolume.OnMove += (_, _) => {
            Rectangle intersectArea = skillVolume.BoundsSprite.IntersectArea;
            Sprite slider = skillVolume.SliderSprite;
            float newVolume = (slider.Position.X - intersectArea.X + (float) slider.IntersectArea.Width / 2) / intersectArea.Width;
            Settings.Instance.Skill = Settings.Instance.General * newVolume;
            
            _uiManagers["Sound"].TextElemDict["Skill Volume"].Text = $"Skill Volume: {(int) (newVolume * 100)}";
        };
        xPos = (1f - Settings.Instance.Skill) * skillVolume.BoundsSprite.IntersectArea.Width;
        skillVolume.SliderSprite.Position = Vector2.Subtract(skillVolume.SliderSprite.Position, new Vector2(xPos, 0));
    }

    private void LoadRadio() {
        RadioBtn general = _uiManagers["Sound"].AddRadio("Sound", Globals.HudAtlas, Globals.HudDict["Radio"].Rect, Vector2.Add(_positions[2], new Vector2(0, 60)));
        general.OnClick += (_, _) => {
            Settings settings = Settings.Instance;
            settings.NotMutedAll = !settings.NotMutedAll;
            if (settings.NotMutedAll)
                AudioManager.ResumeSong();
            else
                AudioManager.PauseSong();
        };
        general.Checked = Settings.Instance.NotMutedAll;
        RadioBtn battle = _uiManagers["Sound"].AddRadio("Battle Sound", Globals.HudAtlas, Globals.HudDict["Radio"].Rect, Vector2.Add(_positions[2], new Vector2(120, 60)));
        battle.OnClick += (_, _) => {
            Settings settings = Settings.Instance;
            settings.NotMutedBattle = !settings.NotMutedBattle;
            if (PrevState is not BattleState)
                return;
            if (settings.NotMutedBattle)
                AudioManager.ResumeSong();
            else
                AudioManager.PauseSong();
        };
        battle.Checked = Settings.Instance.NotMutedBattle;
        RadioBtn skill = _uiManagers["Sound"].AddRadio("Skill Sound", Globals.HudAtlas, Globals.HudDict["Radio"].Rect, Vector2.Add(_positions[2], new Vector2(240, 60)));
        skill.OnClick += (_, _) => {
            Settings settings = Settings.Instance;
            settings.NotMutedSkill = !settings.NotMutedSkill;
        };
        skill.Checked = Settings.Instance.NotMutedSkill;
    }

    private void SetText(string sector) {
        foreach ((string key, Button btn) in _uiManagers[sector].BtnDict) {
            Vector2 midPoint = Vector2.Add(btn.Position, new Vector2((float) btn.IntersectArea.Width / 2, (float) btn.IntersectArea.Height / 2));
            Vector2 textStart = Vector2.Subtract(midPoint, Globals.BoldFont.MeasureString(key));

            TextElem textElem = _uiManagers[sector].AddTextElem(key, key, textStart, Color.White);
            textElem.Scale = new Vector2(2);
            textElem.Font = Globals.BoldFont;
        }

        foreach ((string key, Slider slider) in _uiManagers[sector].SliderDict) {
            Vector2 midPoint = Vector2.Add(slider.BoundsSprite.Position, new Vector2((float) slider.BoundsSprite.IntersectArea.Width / 2, (float) slider.BoundsSprite.IntersectArea.Height / 2 - 50));
            Vector2 textStart = Vector2.Subtract(midPoint, Globals.Font.MeasureString(key) * 1.5f / 2);

            float volume = Settings.Instance.GetVolume(key.Split(" ")[0]);

            TextElem textElem = _uiManagers[sector].AddTextElem(key, $"{key}: {(int) (volume * 100)}", textStart, Color.White);
            textElem.Scale = new Vector2(1.5f);
        }

        foreach ((string key, RadioBtn radioBtn) in _uiManagers[sector].RadioDict) {
            Vector2 midPoint = Vector2.Add(radioBtn.Position, new Vector2((float) radioBtn.Rectangle.Width / 2 * radioBtn.Scale.X, (float) radioBtn.Rectangle.Height / 2 - 50));
            Vector2 textStart = Vector2.Subtract(midPoint, Globals.BoldFont.MeasureString(key) / 2);

            TextElem textElem = _uiManagers[sector].AddTextElem(key, key, textStart, Color.White);
            textElem.Font = Globals.BoldFont;
        }
    }

    public override void LoadContent() { }
    public override void Update(GameTime gameTime) {
        _uiManagers[_sectors.Peek()].Update(gameTime);
    }

    public override void PostUpdate(GameTime gameTime) { }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Backgrounds == null || PrevState == null)
            return;

        spriteBatch.Begin();
        foreach (Sprite bg in Backgrounds)
            bg.Draw(gameTime, spriteBatch);
        
        spriteBatch.Draw(Globals.HudAtlas, new Vector2(768, 220), Globals.HudDict["SettingsHUD"].Rect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.75f);
        _uiManagers[_sectors.Peek()].Draw(gameTime, spriteBatch);

        spriteBatch.End();
    }
}