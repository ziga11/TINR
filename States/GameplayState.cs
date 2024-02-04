using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Managers;
using WinApp.Sprites;
using WinApp.Sprites.Character;
using WinApp.Sprites.UI;
using WinApp.Sprites.UIElem;

namespace WinApp.States;

public sealed class GameplayState : State {
    public readonly Player Player;
    private readonly Game1 _game1;
    private readonly Button _settings = new Button(new Vector2(1856, 1015), Globals.HudAtlas, Globals.HudDict["SettingsIcon"].Rect);
    private readonly Button _mobSelection = new Button(new Vector2(1790, 1015), Globals.HudAtlas, Globals.HudDict["MobSelectIcon"].Rect);
    private List<SearchObject> _searchObjects;
    public readonly Sprite[] Backgrounds = {
            new Sprite(Globals.GameBg1),
            new Sprite(Globals.GameBg2) {Position = new Vector2(1920, 1080)},
            new Sprite(Globals.GameBg3) {Position = new Vector2(1920 * 2, 1080)}
    };

    public readonly Sprite[] WalkBackgrounds = {
            new Sprite(Globals.WalkBg1),
            new Sprite(Globals.WalkBg2) {Position = new Vector2(1920, 1080)},
            new Sprite(Globals.WalkBg3) {Position = new Vector2(1920 * 2, 1080)}
    };

    private int _leftBgInd;
    private int _rightBgInd = 1;
    private int _previousXPos = 1440;
    public bool RenderMobSelection;
    public TimeSpan Timespan;
    private MonsterSelection _monsterSelection;

    public GameplayState(Game1 game1, ContentManager content) : base(game1, content) {
        Player = new Player(game1);
        _game1 = game1;
        Globals.AudioManager.StartSong("General", "Gameplay");

        _settings.OnClick += MenuState;
        _mobSelection.OnClick += (_, _) => {
            _monsterSelection = new MonsterSelection(Player);
            RenderMobSelection = !RenderMobSelection;
        };

        LoadContent();
    }

    public override void LoadContent() {
        LoadObjects();
        foreach (Sprite bg in WalkBackgrounds)
            bg.SetTextureData();
    }

    /* TODO: SQL */

    private void LoadObjects() {

        _searchObjects = new List<SearchObject> {
                /* Sector 1*/
                new SearchObject("DarkRock1", new Vector2(200, 600)),
                new SearchObject("TreeBark", new Vector2(500, 500)),
                new SearchObject("BigLightRock1", new Vector2(1200, 800)),
                new SearchObject("YellowGrass", new Vector2(1500, 500)),
                /* Sector2 */
                new SearchObject("LilyPadRose", new Vector2(2028, 329)),
                new SearchObject("BrownOrangeGrass", new Vector2(2397, 608)),
                new SearchObject("LightBrownGrass", new Vector2(2255, 401)),
                new SearchObject("MiniBrownGrass", new Vector2(2496, 780)),
                new SearchObject("OrangeGrass", new Vector2(2615, 962)),
                new SearchObject("LilyPad", new Vector2(2182, 471)),
                new SearchObject("WeirdGrass", new Vector2(2957, 36)),
                new SearchObject("NoLeafTree", new Vector2(2557, 677)),
                new SearchObject("TreeBark", new Vector2(3033, 566)),
                new SearchObject("CampFire5", new Vector2(3555, 181)),
                new SearchObject("DarkRock6", new Vector2(3451, 181)),
                new SearchObject("LightBush", new Vector2(3140, 181)),
                new SearchObject("LightBush", new Vector2(2513, 500)),
                new SearchObject("CampFire9", new Vector2(2563, 549)),
                new SearchObject("DarkBush", new Vector2(3132, 476)),
                new SearchObject("MultipleDarkRocks1", new Vector2(3590, 469)),
                new SearchObject("TallerDarkRock1", new Vector2(3623, 633)),
                /* Sector 3*/
                new SearchObject("SpruceTree", new Vector2(5040, 800)),
                new SearchObject("BigFlower", new Vector2(5240, 450)),
                new SearchObject("TreeStump", new Vector2(5300, 250)),
                new SearchObject("DarkBush", new Vector2(5100, 250)),
                new SearchObject("LightBush", new Vector2(5100, 450)),
                new SearchObject("BigLightRock1", new Vector2(3890, 750)),
                new SearchObject("YellowGrass", new Vector2(4700, 50)),
                new SearchObject("MultipleDarkRocks2", new Vector2(4800, 200)),
                new SearchObject("TallerDarkRock1", new Vector2(4800, 400)),
                new SearchObject("FlatDarkRock", new Vector2(4500, 400)),
                new SearchObject("BrownGrass", new Vector2(4000, 400)),
                new SearchObject("OrangeGrass", new Vector2(4540, 100)),
                new SearchObject("MiniBrownGrass", new Vector2(4400, 150)),
                new SearchObject("CampFire3", new Vector2(3840, 600)),
                new SearchObject("SunFlower", new Vector2(4340, 700)),
                new SearchObject("TreeBark", new Vector2(4340, 500)),
                new SearchObject("DarkRock4", new Vector2(5440, 750))
        };
    }

    private void MenuState(object sender, EventArgs args) {
        Globals.MenuState.Backgrounds = Backgrounds.ToList();
        Globals.MenuState.PrevState = this;
        _game1.SetState(Globals.MenuState);
    }

    private void UpdateBackgrounds() {
        Vector2 currPos = Player.Position;
        int xDiff = (int) currPos.X - _previousXPos;

        /* Same Position as the previous one */
        if (_previousXPos == (int) currPos.X)
            return;

        /* At first BG, which is fully loaded and moving left */
        if (_leftBgInd == 0 && (Backgrounds[_leftBgInd].Position.X - xDiff) >= 0)
            return;

        /* At last BG, which is fully loaded and moving right */
        if (_rightBgInd == Backgrounds.Length - 1 && Backgrounds[_rightBgInd].Position.X - xDiff <= 0)
            return;

        /* BGs only move if the char is in the 600px of the either side of the BG */
        if ((currPos.X > 600 && xDiff < 0) || currPos.X < 1920 - 600 && xDiff > 0)
            return;

        if (Backgrounds[_leftBgInd].Position.X - xDiff < -1920) {
            _rightBgInd = Math.Min(_rightBgInd + 1, Backgrounds.Length - 1);
            _leftBgInd = _rightBgInd - 1;
        }
        else if (Backgrounds[_rightBgInd].Position.X - xDiff > 1920) {
            _leftBgInd = Math.Max(_leftBgInd - 1, 0);
            _rightBgInd = _leftBgInd + 1;
        }

        for (int i = 0; i < Backgrounds.Length; i++) {
            Backgrounds[i].Position = new Vector2(Backgrounds[i].Position.X - xDiff, 0);
            WalkBackgrounds[i].Position = new Vector2(WalkBackgrounds[i].Position.X - xDiff, 0);
        }

        foreach (SearchObject searchObject in _searchObjects)
            searchObject.UpdatePos((int) searchObject.Position.X - xDiff, (int) searchObject.Position.Y);

        Player.Position = new Vector2(Player.Position.X - xDiff, Player.Position.Y);
    }


    public override void Update(GameTime gameTime) {
        if (Player == null)
            return;
        if (RenderMobSelection) {
            _monsterSelection.Update(gameTime);
            return;
        }

        UpdateBackgrounds();
        _previousXPos = (int) Player.Position.X;

        Player.Update(gameTime);

        Player.SearchObject = null;

        foreach (SearchObject searchObject in _searchObjects.Where(searchObject => searchObject.Position.X is >= 0 and <= 1920)) {
            searchObject.Update(gameTime);
            if (searchObject.Texture2D == Globals.HoveredAtlas)
                Player.SearchObject = searchObject;
        }
        _settings.Update(gameTime);
        _mobSelection.Update(gameTime);
    }

    public override void PostUpdate(GameTime gameTime) { }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Backgrounds == null || Player == null)
            return;

        spriteBatch.Begin();

        Backgrounds[_leftBgInd].Draw(gameTime, spriteBatch);
        Backgrounds[_rightBgInd].Draw(gameTime, spriteBatch);

        if (RenderMobSelection) {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront);
            _monsterSelection.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            return;
        }
        foreach (SearchObject searchObject in _searchObjects.Where(searchObject => searchObject.Position.X is >= 0 and <= 1920))
            searchObject.Draw(gameTime, spriteBatch);

        Player.Draw(gameTime, spriteBatch);
        _settings.Draw(gameTime, spriteBatch);
        _mobSelection.Draw(gameTime, spriteBatch);
        spriteBatch.End();
    }
}