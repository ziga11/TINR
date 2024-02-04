using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Managers;
using WinApp.Sprites;
using WinApp.Sprites.Character;
using WinApp.Sprites.UIElem;

namespace WinApp.States;

public class BattleState : State {
    private readonly Player _player;
    private readonly Texture2D[] _backgroundAtlases;

    private Monster _enemyMonster;

    private readonly Button _settings = new Button(new Vector2(1856, 1016), Globals.HudAtlas, Globals.HudDict["SettingsIcon"].Rect);
    private BattleManager _battleManager;
    private Sprite _background;

    public BattleState(Game1 game1, ContentManager content, Texture2D[] backgroundAtlases) :
        base(game1, content) {
        _backgroundAtlases = backgroundAtlases;
        GameplayState gameplayState = game1.LoadStates.GpState;
        _player = gameplayState.Player;
        Globals.AudioManager.StartSong("Battle", "Battle");
        AudioManager.SetRepeating(true);

        _settings.OnClick += MenuState;

        LoadContent();
    }

    public sealed override void LoadContent() {
        _background = new Sprite(_backgroundAtlases[0]) {
            Layer = 1.0f,
        };
        Init();
    }

    private void Init() {
        SetPossibleMobs();
        _battleManager = new BattleManager(_player, _enemyMonster);
    }

    private void SetPossibleMobs() {
        Vector2 firstBgPos = Game1.LoadStates.GpState.WalkBackgrounds[0].Position;
        switch (firstBgPos.X - _player.Position.X) {
            case > -1920:
                Sector1();
                break;
            case > -3840 and < -1920:
                Sector2();
                break;
            case >= -5760 and < -3840:
                Sector3();
                break;
        }
    }
    private void MenuState(object sender, EventArgs args) {
        Globals.MenuState.Backgrounds = new List<Sprite> {_background};
        Globals.MenuState.PrevState = this;
        Game1.SetState(Globals.MenuState);
    }

    private void Sector1() {
        List<Monster> possibleMonsters = new List<Monster> {
            new Monster("FirePuppy", "Fire", Globals.FireMonsterAtlas, new Random().Next(1, 6)),
            new Monster("AquaVisor", "Water", Globals.WaterMonsterAtlas, new Random().Next(1, 6)),
            new Monster("Tiger", "Nature", Globals.NatureMonsterAtlas, new Random().Next(1, 6)),
            new Monster("WindUnicorn", "Wind", Globals.WindMonsterAtlas, new Random().Next(1, 6))
        };
        Random random = new Random();
        int randomVal = random.Next(0, possibleMonsters.Count);
        _enemyMonster = possibleMonsters[randomVal];
    }

    private void Sector2() {
        List<Monster> possibleMonsters = new List<Monster> {
            new Monster("FirePuppy", "Fire", Globals.FireMonsterAtlas,new Random().Next(8, 15)),
            new Monster("FlameUnicorn", "Fire", Globals.FireMonsterAtlas,new Random().Next(8, 15)),
            new Monster("Blazer", "Fire", Globals.FireMonsterAtlas,new Random().Next(8, 15)),
            new Monster("AquaVisor", "Water", Globals.WaterMonsterAtlas,new Random().Next(8, 15)),
            new Monster("Alligator", "Water", Globals.WaterMonsterAtlas,new Random().Next(8, 15)),
            new Monster("AquaFluffy", "Water", Globals.WaterMonsterAtlas,new Random().Next(8, 15)),
            new Monster("Tiger", "Nature", Globals.NatureMonsterAtlas,new Random().Next(8, 15)),
            new Monster("GreenWolf", "Nature", Globals.NatureMonsterAtlas,new Random().Next(8, 15)),
            new Monster("GreenBunny", "Nature", Globals.NatureMonsterAtlas,new Random().Next(8, 15)),
            new Monster("WindUnicorn", "Wind", Globals.WindMonsterAtlas,new Random().Next(8, 15)),
            new Monster("Aetos", "Wind", Globals.WindMonsterAtlas,new Random().Next(8, 15)),
            new Monster("Bonobo", "Wind", Globals.WindMonsterAtlas,new Random().Next(8, 15))
        };
        Random random = new Random();
        int randomVal = random.Next(0, possibleMonsters.Count);
        _enemyMonster = possibleMonsters[randomVal];
    }

    private void Sector3() {
        List<Monster> possibleMonsters = new List<Monster> {
            new Monster("FirePuppy", "Fire", Globals.FireMonsterAtlas,new Random().Next(20, 31)),
            new Monster("FlameUnicorn", "Fire", Globals.FireMonsterAtlas,new Random().Next(20, 31)),
            new Monster("Blazer", "Fire", Globals.FireMonsterAtlas,new Random().Next(20, 31)),
            new Monster("AquaVisor", "Water", Globals.WaterMonsterAtlas,new Random().Next(20, 31)),
            new Monster("Alligator", "Water", Globals.WaterMonsterAtlas,new Random().Next(20, 31)),
            new Monster("AquaFluffy", "Water", Globals.WaterMonsterAtlas,new Random().Next(20, 31)),
            new Monster("Tiger", "Nature", Globals.NatureMonsterAtlas,new Random().Next(20, 31)),
            new Monster("GreenWolf", "Nature", Globals.NatureMonsterAtlas,new Random().Next(20, 31)),
            new Monster("GreenBunny", "Nature", Globals.NatureMonsterAtlas,new Random().Next(20, 31)),
            new Monster("WindUnicorn", "Wind", Globals.WindMonsterAtlas,new Random().Next(20, 31)),
            new Monster("Aetos", "Wind", Globals.WindMonsterAtlas,new Random().Next(20, 31)),
            new Monster("Bonobo", "Wind", Globals.WindMonsterAtlas,new Random().Next(20, 31))
        };
        Random random = new Random();
        int randomVal = random.Next(0, possibleMonsters.Count);
        _enemyMonster = possibleMonsters[randomVal];
    }

    public override void Update(GameTime gameTime) {
        _settings.Update(gameTime);
        _battleManager.Update(gameTime);
    }

    public override void PostUpdate(GameTime gameTime) { }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (_background == null || _player.BattleMonsters == null)
            return;
        spriteBatch.Begin(SpriteSortMode.BackToFront);
        _background.Draw(gameTime, spriteBatch);
        _battleManager.Draw(gameTime, spriteBatch);
        _settings.Draw(gameTime, spriteBatch);
        spriteBatch.End();
    }
}