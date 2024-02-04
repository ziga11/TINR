using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Managers;
using WinApp.Sprites.Character;
using WinApp.Sprites.UIElem;

namespace WinApp.Sprites.UI;

public class MonsterSelection {
    private readonly Player _player;
    private readonly Dictionary<Button, Monster> _btnMobDict = new Dictionary<Button, Monster>();
    private readonly UiManager _uiManager = new UiManager();

    public MonsterSelection(Player player) {
        _player = player;
        LoadInventory();
        LoadSelection();
    }

    private void LoadInventory() {
        Sprite bg = _uiManager.AddSprite("Background", Globals.HudAtlas, Globals.HudDict["ResultHUD"].Rect, new Vector2(608, 220), 1f);
        Button exit = _uiManager.AddButton("Exit", new Vector2(bg.Position.X + bg.IntersectArea.Width - 64, 220), Globals.HudAtlas, Globals.HudDict["XExit"].Rect);
        exit.OnClick += (_, _) => Globals.Game.LoadStates.GpState.RenderMobSelection = false;
        for (int i = 0; i < _player.Inventory.Count; i++) {
            Monster monster = _player.Inventory[i];
            // ReSharper disable once PossibleLossOfFraction
            Vector2 first = new Vector2(660, 320 + i / 6 * 120);
            Vector2 pos = Vector2.Add(first, new Vector2(i % 6 * 100, 0));
            Sprite circle = _uiManager.AddSprite($"Circle{monster.Name}", Globals.HudAtlas, Globals.HudDict["Circle"].Rect, pos, 0.2f);
            Button icon = _uiManager.AddButton($"{monster.Name}", pos, Globals.HudAtlas, Globals.HudDict[monster.Name].Rect);
            icon.Layer = 0.3f;
            _btnMobDict[icon] = monster;
            icon.OnHover += (_, _) => {
                if (!_player.BattleMonsters.Contains(monster))
                    icon.Shade = Color.Magenta;
            };
            icon.OnLeave += (_, _) => {
                if (!_player.BattleMonsters.Contains(monster))
                    icon.Shade = Color.White;
            };
            icon.OnClick += AddMonster;
            if (monster.Health == 0)
                icon.Shade = Color.DarkSlateGray;
            if (_player.BattleMonsters.Contains(monster))
                icon.Shade = Color.DarkSlateBlue;

            Vector2 elemPos = Vector2.Add(pos, new Vector2(0, 41.6f));
            _uiManager.AddSprite($"Elem{monster.Name}", Globals.HudAtlas, Globals.HudDict[monster.Element].Rect, elemPos, 0.1f, new Vector2(0.35f));

            Vector2 midTextPoint = Vector2.Subtract(pos, new Vector2((float) -circle.IntersectArea.Width / 2, 20));
            Vector2 textPos = Vector2.Subtract(midTextPoint, Globals.Font.MeasureString($"{monster.Name}") / 2);
            _uiManager.AddTextElem($"{monster.Name}", $"{monster.Name}", textPos, Color.White);
            _uiManager.AddTextElem($"Lvl{monster.Name}", $"{monster.Level}", pos, Color.White);
        }
    }

    private void AddMonster(object sender, EventArgs args) {
        if (_player.BattleMonsters[3] != null)
            return;
        if (sender is not Button btn)
            return;
        Monster monster = _btnMobDict[btn];
        if (_player.BattleMonsters.Contains(monster))
            return;

        int index = 0;
        for (; index < _player.BattleMonsters.Length && _player.BattleMonsters[index] != null; index++) {}
        _player.BattleMonsters[index] = monster;
        btn.Shade = Color.DarkRed;
        Sprite pHud = _uiManager.SpriteDict["PHud"];
        Vector2 iconPos = Vector2.Add(pHud.Position, Globals.IconPos[index]);
        AddToSelection(monster, iconPos);
    }

    private void RemoveMonster(object sender, EventArgs args) {
        if (sender is not Button btn)
            return;
        Monster monster = _btnMobDict[btn];
        RemoveFromSelection(monster);
    }

    private void LoadSelection() {
        Sprite bg = _uiManager.SpriteDict["Background"];
        Vector2 pHudPos = new Vector2(bg.Position.X + (float) bg.IntersectArea.Width / 2 - (float) Globals.HudDict["PMonsterHUD"].Rect.Width / 2, bg.IntersectArea.Height + 50);
        Sprite pHud = _uiManager.AddSprite("PHud", Globals.HudAtlas, Globals.HudDict["PMonsterHUD"].Rect, pHudPos, 0.5f);
        for (int i = 0; i < _player.BattleMonsters.Length && _player.BattleMonsters[i] != null; i++) {
            Vector2 iconPos = Vector2.Add(pHud.Position, Globals.IconPos[i]);
            Monster monster = _player.BattleMonsters[i];
            AddToSelection(monster, iconPos);
        }
    }
    private void AddToSelection(Monster monster, Vector2 pos) {
        int index = Array.IndexOf(_player.BattleMonsters, monster);
        Button icon = _uiManager.AddButton($"Selected{index}", pos, Globals.HudAtlas, Globals.HudDict[monster.Name].Rect);
        icon.OnHover += (_, _) => icon.Shade = Color.DarkRed;
        icon.OnLeave += (_, _) => icon.Shade = Color.White;
        icon.OnClick += RemoveMonster;
        _btnMobDict[icon] = monster;
        
        Vector2 midTextPoint = Vector2.Subtract(pos, new Vector2((float) -icon.IntersectArea.Width / 2, 10));
        Vector2 textPos = Vector2.Subtract(midTextPoint, Globals.Font.MeasureString(monster.Name) * (float) 0.7 / 2);
        TextElem textElem = _uiManager.AddTextElem($"Selected{index}", monster.Name, textPos, Color.White);
        textElem.Position = textPos;
        textElem.Scale = new Vector2(0.7f);
    }
    private void RemoveFromSelection(Monster monster) {
        _uiManager.BtnDict[monster.Name].Shade = Color.White;
        int index = Array.IndexOf(_player.BattleMonsters, monster);
        int i = index + 1;
        for (; i < _player.BattleMonsters.Length && _player.BattleMonsters[i] != null; i++) {
            _player.BattleMonsters[i - 1] = _player.BattleMonsters[i];
            Vector2 pos = Vector2.Add(_uiManager.SpriteDict["PHud"].Position, Globals.IconPos[i - 1]);
            Button icon = _uiManager.BtnDict[$"Selected{i}"];
            icon.Position = pos;
            icon.SetIntersectArea();
            _uiManager.BtnDict[$"Selected{i - 1}"] = icon;

            Vector2 midTextPoint = Vector2.Subtract(pos, new Vector2((float) -icon.IntersectArea.Width / 2, 10));
            Vector2 textPos = Vector2.Subtract(midTextPoint, Globals.Font.MeasureString(monster.Name) * (float) 0.7 / 2);
            TextElem textElem = _uiManager.TextElemDict[$"Selected{i}"];
            textElem.Position = textPos;
            textElem.Scale = new Vector2(0.7f);
            _uiManager.TextElemDict[$"Selected{i - 1}"] = textElem;
        }
        _player.BattleMonsters[i - 1] = null;
        _uiManager.RemoveTextElem($"Selected{i - 1}");
        _uiManager.RemoveButton($"Selected{i - 1}");
    }

    public void Update(GameTime gameTime) {
        _uiManager.Update(gameTime);
    }
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        _uiManager.Draw(gameTime, spriteBatch);
    }
}