using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Animation;
using WinApp.Sprites;
using WinApp.Sprites.Character;
using WinApp.Sprites.Skill;
using WinApp.Sprites.UIElem;
using WinApp.States;

namespace WinApp.Managers;

public class BattleManager {
    private readonly Player _player;
    private readonly Monster _enemyMonster;
    private readonly AnimationManager _animationManager = new AnimationManager();
    private readonly Dictionary<Button, Skill> _btnSkillMap = new Dictionary<Button, Skill>();
    private readonly List<SkillLoadOut> _playerLoadOuts = new List<SkillLoadOut>();
    private readonly UiManager _hudUi = new UiManager();
    private readonly UiManager _skillUi = new UiManager();
    private readonly SkillAction _skillAction;
    private SkillLoadOut _enemyLoadOut;
    private int _pMonsterInd;
    private bool _turn = true;

    public BattleManager(Player player, Monster eMonster) {
        _player = player;
        _enemyMonster = eMonster;

        Monster monster = _player.BattleMonsters.FirstOrDefault(monster => monster is {Health: > 0});
        _pMonsterInd = Array.IndexOf(_player.BattleMonsters, monster);

        _skillAction = new SkillAction(player.BattleMonsters[_pMonsterInd], _enemyMonster);
        SetLoadOut();
        SetBattleHudUi();
    }

    private void SetLoadOut() {
        for (int i = 0; i < _player.BattleMonsters.Length && _player.BattleMonsters[i] != null; i++) {
            Monster monster = _player.BattleMonsters[i];
        
            List<Skill> skillList = monster.Skills.GetRange(0, monster.Level / 5 + 1);
            _playerLoadOuts.Add(new SkillLoadOut(skillList));
        }
        
        _enemyLoadOut = new SkillLoadOut(_enemyMonster.Skills.GetRange(0, _enemyMonster.Level / 5 + 1));
    }


    private void SetBattleHudUi() {
        SetBattleSprites();
        SetBattleText();
        SetBattleButtons();
        SetSkillHud();
    }

    /* TODO: Parse this out so skill hud is its own class, result is its own class, playerHUD is its own class and finally enemyMonsterHUD is its own class */
    /* TODO: Then use this Battle Manager to connect them */
    
    private void SetBattleSprites() {
        Vector2 skillHudPos = new Vector2(427, 888);
        _hudUi.AddSprite("SkillHud", Globals.HudAtlas, Globals.HudDict["SkillHUD"].Rect, skillHudPos, 1f);

        _hudUi.AddSprite("PHud", Globals.HudAtlas, Globals.HudDict["PMonsterHUD"].Rect, Vector2.One, 1f);

        Vector2 eMonsterHud = new Vector2(1663, 1);
        _hudUi.AddSprite("EHud", Globals.HudAtlas, Globals.HudDict["EMonsterHUD"].Rect, eMonsterHud, 1f);

        Vector2 eIconPos = new Vector2(1841, 33);
        _hudUi.AddSprite("EIcon", Globals.HudAtlas, Globals.HudDict[_enemyMonster.Name].Rect, eIconPos);

        Vector2 eHealthPos = new Vector2(1690, 45);
        Sprite eHealth = _hudUi.AddSprite("EHealth", Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, eHealthPos, 0.5f);
        eHealth.OnHover += (_, _) => _hudUi.TextElemDict["EHealth"].Visible = true;
        eHealth.OnLeave += (_, _) => _hudUi.TextElemDict["EHealth"].Visible = false;

        _enemyMonster.Position = new Vector2(1500, 250);

        for (int i = 0; i < _player.BattleMonsters.Length && _player.BattleMonsters[i] != null; i++) {
            int localI = i;
            Monster monster = _player.BattleMonsters[i];
            monster.Position = new Vector2(200, 250);

            /* Health */
            Vector2 hpPos = Vector2.Add(Globals.IconPos[i], new Vector2(1, 72));
            
            Sprite missingHp = _hudUi.AddSprite($"PMissingHealth{localI}", Globals.HudAtlas, Globals.HudDict["PHealth"].Rect, hpPos, 0.1f);
            missingHp.Color = Color.DarkSlateGray;
            missingHp.OnHover += (_, _) => _hudUi.TextElemDict[$"PHealth{localI}"].Visible = true;
            missingHp.OnLeave += (_, _) => _hudUi.TextElemDict[$"PHealth{localI}"].Visible = false;
            
            _hudUi.AddSprite($"PHealth{localI}", Globals.HudAtlas, HpBar(monster), hpPos);

            /* Element */
            Vector2 elemPos = Vector2.Add(Globals.IconPos[localI], new Vector2(41.6f, 41.6f));
            _hudUi.AddSprite($"Element{localI}", Globals.HudAtlas, Globals.HudDict[monster.Element].Rect, elemPos, new Vector2(0.35f));
        }
    }

    private void SetBattleText() {
        Vector2 eHealthPos = new Vector2(1730, 70);
        TextElem textElem = _hudUi.AddTextElem("EHealth", $"{_enemyMonster.Health}/{_enemyMonster.MaxHealth}", eHealthPos, Color.White);
        textElem.Visible = false;
        textElem.Scale = new Vector2(1.5f);

        Vector2 namePosition = Vector2.Subtract(new Vector2(1770, 24), Globals.Font.MeasureString(_enemyMonster.Name) / 2);
        _hudUi.AddTextElem("EName", $"{_enemyMonster.Name}", namePosition, Color.White);
        _hudUi.AddTextElem("ELvl", $"{_enemyMonster.Level}", new Vector2(1900, 33), Color.White).Font = Globals.BoldFont;
        for (int i = 0; i < _player.BattleMonsters.Length && _player.BattleMonsters[i] != null; i++) {
            Monster monster = _player.BattleMonsters[i];

            Vector2 monsterHealthPos = Vector2.Add(Globals.IconPos[i], new Vector2(10, 80));
            _hudUi.AddTextElem($"PHealth{i}", $"{monster.Health}/{monster.MaxHealth}", monsterHealthPos, Color.White).Visible = false;

            _hudUi.AddTextElem($"Lvl{i}", $"{monster.Level}", Globals.IconPos[i], Color.White).Font = Globals.BoldFont;

            Vector2 midXPos = Vector2.Add(Globals.IconPos[i], new Vector2((float) Globals.HudDict[monster.Name].Width / 2, -10));
            Vector2 namePos = Vector2.Subtract(midXPos, Globals.Font.MeasureString(monster.Name) * 0.75f / 2);
            TextElem name = _hudUi.AddTextElem($"PName{i}", $"{monster.Name}", namePos, Color.White);
            name.Scale = new Vector2(0.75f);
        }

        Vector2 capturePos = new Vector2(900, 200);
        string txt = $"Capture chance: {(1 - (float) _enemyMonster.Health / _enemyMonster.MaxHealth) * 100}%";
        _hudUi.AddTextElem("CaptureChance", txt, capturePos, Color.White).Visible = false;
    }

    private void SetSkillHud() {
        _skillUi.BtnDict.Clear();
        _skillUi.SpriteDict.Clear();
        _skillUi.TextElemDict.Clear();
        List<Skill> skills = _playerLoadOuts[_pMonsterInd].Skills;
        List<Skill> shownSkills = _playerLoadOuts[_pMonsterInd].ShownSkills;

        Vector2 prevSkillPos = new Vector2(457, 952);
        Vector2 nextSkillPos = new Vector2(1430, 952);

        Button prev = _hudUi.AddButton("BackArrow", prevSkillPos, Globals.HudAtlas, Globals.HudDict["ArrowL"].Rect);
        Button next = _hudUi.AddButton("NextArrow", nextSkillPos, Globals.HudAtlas, Globals.HudDict["ArrowR"].Rect);

        prev.Visible = _playerLoadOuts[_pMonsterInd].Skills.Count > 3;
        prev.OnClick += (_, _) => {
            int firstIndex = _playerLoadOuts[_pMonsterInd].LeftArrow();
            _skillUi.BtnDict[$"Skill{firstIndex}"].Visible = true;
            for (int i = firstIndex, j = 0; i < firstIndex + 4; i++, j++) {
                _skillUi.BtnDict[$"Skill{i}"].Position = Globals.SkillPos[j];
                _skillUi.TextElemDict[$"Skill{i}"].Position = Vector2.Subtract(Globals.SkillPos[j], new Vector2(-5, 140));
                _skillUi.BtnDict[$"Skill{i}"].SetIntersectArea();
                _skillUi.BtnDict[$"Skill{i}"].OnHover += null;
                _skillUi.BtnDict[$"Skill{i}"].OnHover += SkillHover;
                _skillUi.BtnDict[$"Skill{i}"].OnLeave += null;
                _skillUi.BtnDict[$"Skill{i}"].OnLeave += SkillLeave;
            }
            _skillUi.BtnDict[$"Skill{firstIndex + 4}"].Visible = false;

            next.Visible = true;
            if (firstIndex == 0)
                prev.Visible = false;
        };

        next.OnClick += (_, _) => {
            int lastIndex = _playerLoadOuts[_pMonsterInd].RightArrow();
            shownSkills = _playerLoadOuts[_pMonsterInd].ShownSkills;
            _skillUi.BtnDict[$"Skill{lastIndex}"].Visible = true;
            for (int i = lastIndex - 3, j = 0; i <= lastIndex; i++, j++) {
                _skillUi.BtnDict[$"Skill{i}"].Position = Globals.SkillPos[j];
                _skillUi.TextElemDict[$"Skill{i}"].Position = Vector2.Subtract(Globals.SkillPos[j], new Vector2(-5, 140));
                _skillUi.BtnDict[$"Skill{i}"].SetIntersectArea();
                _skillUi.BtnDict[$"Skill{i}"].OnHover += null;
                _skillUi.BtnDict[$"Skill{i}"].OnHover += SkillHover;
                _skillUi.BtnDict[$"Skill{i}"].OnLeave += null;
                _skillUi.BtnDict[$"Skill{i}"].OnLeave += SkillLeave;
            }

            _skillUi.BtnDict[$"Skill{lastIndex - 4}"].Visible = false;

            prev.Visible = true;
            if (lastIndex == skills.Count - 1)
                next.Visible = false;
        };
        next.Visible = false;


        for (int i = 0; i < shownSkills.Count; i++) {
            Vector2 infoPos = Vector2.Subtract(Globals.SkillPos[i], new Vector2(0, 140));
            _skillUi.AddSprite($"InfoRect{i}", Globals.HudAtlas, Globals.HudDict["SkillInfoBox"].Rect, infoPos, 0.5f).Visible = false;
        }

        for (int i = 0; i < skills.Count; i++) {
            int shownIndex = Math.Min(Math.Max(shownSkills.IndexOf(skills[i]), 0), shownSkills.Count - 1);
            Vector2 infoTextPos = Vector2.Subtract(Globals.SkillPos[shownIndex], new Vector2(-5, 140));

            /* Add Att, AttUp, ...*/
            string text = $"{skills[i].Name}\n\n";
            _skillUi.AddTextElem($"Skill{i}", text, infoTextPos, Color.White).Visible = false;
        }

        for (int i = 0; i < skills.Count; i++) {
            int localI = i;
            int shownIndex = Math.Min(Math.Max(shownSkills.IndexOf(skills[localI]), 0), shownSkills.Count - 1);
            
            Button button = new Button(Globals.SkillPos[shownIndex], Globals.HudAtlas, Globals.HudDict[skills[i].Name].Rect);

            if (!shownSkills.Contains(skills[i]))
                button.Visible = false;

            List<Sprite> skillSprites = new List<Sprite> {new Sprite(Globals.SkillAtlas, skills[i].SkillTex.Rectangle, Vector2.Zero)};

            for (int j = 1; j < skills[i].SkillTex.Count; j++) {
                Rectangle prevSkillRect = skillSprites[j - 1].Rectangle;
                Rectangle newSkillRect = new Rectangle(prevSkillRect.X + 384, prevSkillRect.Y, prevSkillRect.Width, prevSkillRect.Height);
                skillSprites.Add(new Sprite(Globals.SkillAtlas, newSkillRect, Vector2.Zero));
            }

            _animationManager.AddAnimation($"P{skills[localI].Name}", skillSprites, GetSkillDir(skills[i], true), 1600f);
            button.OnClick += SkillClicked;
            button.OnHover += SkillHover;
            button.OnLeave += SkillLeave;

            _skillUi.AddButton($"Skill{localI}", button);
            _btnSkillMap[button] = skills[i];
        }
    }

    private List<Vector2> GetSkillDir(Skill skill, bool pAttacker) {
        SkillStats skillStats = skill.SkillStats;
        if (skillStats.Att > 0)
            return pAttacker ? new List<Vector2> {new Vector2(200, 250), new Vector2(1500, 250)} : new List<Vector2> {new Vector2(1500, 250), new Vector2(200, 250)};
        if (skillStats.AttDown > 0 || skillStats.DefDown > 0)
            return pAttacker ? new List<Vector2> {new Vector2(1500, 150), new Vector2(1500, 600)} : new List<Vector2> {new Vector2(200, 150), new Vector2(200, 600)};
         
        return pAttacker ? new List<Vector2> {new Vector2(1500, 600), new Vector2(1500, 150)} : new List<Vector2> {new Vector2(200, 600), new Vector2(200, 150)};
    }

    private void SetBattleButtons() {
        foreach (Skill skill in _enemyLoadOut.Skills) {
            List<Sprite> a = new List<Sprite> {new Sprite(Globals.SkillAtlas, Globals.SkillDict[skill.Name].SkillTex.Rectangle, Vector2.Zero)};

            for (int j = 0; j < Globals.SkillDict[skill.Name].SkillTex.Count - 1; j++)
                a.Add(new Sprite(Globals.SkillAtlas, new Rectangle(a[j].Rectangle.X + 384, a[j].Rectangle.Y, a[j].Rectangle.Width, a[j].Rectangle.Height), Vector2.Zero));

            _animationManager.AddAnimation($"E{skill.Name}", a, GetSkillDir(skill, false), 1600f);
        }


        Monster[] monsters = _player.BattleMonsters;
        for (int i = 0; i < monsters.Length && monsters[i] != null; i++) {
            int localI = i;
            Button icon = new Button(Globals.IconPos[localI], Globals.HudAtlas, Globals.HudDict[monsters[localI].Name].Rect)
                                                        {Layer = 0.5f, SpriteEffects = SpriteEffects.FlipHorizontally};
            if (monsters[i].Health == 0)
                icon.Shade = Color.DarkSlateGray;

            icon.OnHover += (_, _) => {
                if (monsters[localI].Health == 0)
                    return;
                icon.Shade = Color.DarkSlateGray;
            };
            icon.OnLeave += (_, _) => {
                if (monsters[localI].Health == 0)
                    return;
                icon.Shade = Color.White;
            };
            icon.OnClick += (_, _) => {
                if (monsters[localI].Health == 0 || localI == _pMonsterInd)
                    return;
                Globals.DelayedAction(500, () => {
                    _pMonsterInd = localI;
                    _turn = false;
                    _skillAction.PMonster = monsters[_pMonsterInd];
                    SetSkillHud();
                    EnemyAttack();
                });
            };

            _hudUi.AddButton($"{monsters[i].Name}Icon", icon);
        }

        _hudUi.AddButton("Exit", new Vector2(1238, 928), Globals.HudAtlas, Globals.HudDict["Exit"].Rect).OnClick += Exit;

        Button button = _hudUi.AddButton("Capture", new Vector2(864, 0), Globals.HudAtlas, Globals.HudDict["Backpack"].Rect);
        button.Layer = 0.5f;
        button.OnClick += Capture;
        button.OnHover += (_, _) => _hudUi.TextElemDict["CaptureChance"].Visible = true;
        button.OnLeave += (_, _) => _hudUi.TextElemDict["CaptureChance"].Visible = false;
    }

    private void SkillHover(object sender, EventArgs args) {
        if (sender is not Button btn)
            return;
        btn.Shade = Color.DarkSlateGray;
        Skill skill = _btnSkillMap[btn];
        _skillUi.TextElemDict[$"Skill{_playerLoadOuts[_pMonsterInd].Skills.IndexOf(skill)}"].Visible = true;
        _skillUi.SpriteDict[$"InfoRect{_playerLoadOuts[_pMonsterInd].ShownSkills.IndexOf(skill)}"].Visible = true;
    }

    private void SkillLeave(object sender, EventArgs args) {
        if (sender is not Button btn)
            return;
        btn.Shade = Color.White;
        Skill skill = _btnSkillMap[btn];
        _skillUi.TextElemDict[$"Skill{_playerLoadOuts[_pMonsterInd].Skills.IndexOf(skill)}"].Visible = false;
        _skillUi.SpriteDict[$"InfoRect{_playerLoadOuts[_pMonsterInd].ShownSkills.IndexOf(skill)}"].Visible = false;
    }


    private void SkillClicked(object sender, EventArgs args) {
        if (!_turn)
            return;
        if (sender is not Button btn)
            return;
        Skill skill = _btnSkillMap[btn];
        _turn = false;
        Animation.Animation anim = _animationManager.Animate($"P{skill.Name}");
        Globals.AudioManager.StartSoundEffect("Skill", skill.Name);
        anim.AnimationEnd = null;
        anim.AnimationEnd = (_, _) => {
            _skillAction.Action(skill, true);
            _hudUi.SpriteDict["EHealth"].Rectangle = HpBar(_enemyMonster);
            _hudUi.TextElemDict["EHealth"].Text = $"{_enemyMonster.Health}/{_enemyMonster.MaxHealth}";
            _hudUi.TextElemDict["CaptureChance"].Text = $"Capture Chance: {(1 - (float) _enemyMonster.Health / _enemyMonster.MaxHealth) * 100}%";
            foreach (Button button in _skillUi.BtnDict.Values)
                button.Shade = Color.DarkSlateGray;

            if (_enemyMonster.Health == 0)
                SetResultHudUi();
            else
                EnemyAttack();
        };
    }

    private void SetResultHudUi() {
        _skillUi.BtnDict.Clear();
        _skillUi.SpriteDict.Clear();
        _skillUi.TextElemDict.Clear();
        _hudUi.SpriteDict.Clear();
        _hudUi.BtnDict.Clear();
        _hudUi.TextElemDict.Clear();

        CalcXp();
        SetResultText();
        SetResultButtons();
        SetResultSprites();
    }

    private void SetResultSprites() {
        _hudUi.AddSprite("Background", Globals.HudAtlas, Globals.HudDict["ResultHUD"].Rect, new Vector2(608, 220), 1f);
        Monster[] monsters = _player.BattleMonsters;
        for (int i = 0; i < monsters.Length && monsters[i] != null; i++) {
            Monster monster = monsters[i];
            Vector2 iconPos = Vector2.Subtract(new Vector2(752, 290 + i * 147), new Vector2(100, 19));
            int localI = i;

            Sprite icon = _hudUi.AddSprite($"Icon{i}", Globals.HudAtlas, Globals.HudDict[monster.Name].Rect, iconPos, 0.2f, new Vector2(1.5f));
            if (monster.Health == 0)
                icon.Color = Color.DarkSlateGray;

            Vector2 healthPos = Vector2.Add(iconPos, new Vector2(100, 19));
            Vector2 xpPos = Vector2.Add(iconPos, new Vector2(100, 57));

            Sprite missingXp = _hudUi.AddSprite($"MissingXp{i}", Globals.HudAtlas, Globals.HudDict["XPBar"].Rect, xpPos, 0.5f);
            missingXp.Color = Color.DarkSlateGray;
            missingXp.OnHover += (_, _) => _hudUi.TextElemDict[$"PXp{localI}"].Visible = true;
            missingXp.OnLeave += (_, _) => _hudUi.TextElemDict[$"PXp{localI}"].Visible = false;

            Sprite missingHp = _hudUi.AddSprite($"MissingHealth{i}", Globals.HudAtlas, Globals.HudDict["PHealth"].Rect, healthPos, 0.5f, new Vector2(3, 2));
            missingHp.Color = Color.DarkSlateGray;
            missingHp.OnHover += (_, _) => _hudUi.TextElemDict[$"PHealth{localI}"].Visible = true;
            missingHp.OnLeave += (_, _) => _hudUi.TextElemDict[$"PHealth{localI}"].Visible = false;

            _hudUi.AddSprite($"Xp{i}", Globals.HudAtlas, XpBar(monster), xpPos, 0f);
            _hudUi.AddSprite($"Health{i}", Globals.HudAtlas, HpBar(_player.BattleMonsters[i]), healthPos, new Vector2(3, 2));

            Vector2 elemPos = Vector2.Add(iconPos, new Vector2(62.4f));
            _hudUi.AddSprite($"monsterElem{i}", Globals.HudAtlas, Globals.HudDict[monster.Element].Rect, elemPos, new Vector2(0.525f));
        }

        Vector2 eIconPos = new Vector2(1140, 464);
        _hudUi.AddSprite("eIcon", Globals.HudAtlas, Globals.HudDict[_enemyMonster.Name].Rect, eIconPos, 0.5f, new Vector2(2));

        Vector2 eElementPos = Vector2.Add(eIconPos, new Vector2(94.4f, 94.4f));
        _hudUi.AddSprite("eElement", Globals.HudAtlas, Globals.HudDict[_enemyMonster.Element].Rect, eElementPos, new Vector2(0.525f));

        Vector2 eHealthPos = Vector2.Add(eIconPos, new Vector2(0, 140));
        Sprite eMissingHealth = _hudUi.AddSprite("eMissingHealth", Globals.HudAtlas, Globals.HudDict["EHealth"].Rect, eHealthPos, 0.5f, new Vector2(0.8f));
        eMissingHealth.Color = Color.DarkSlateGray;
        eMissingHealth.OnHover += (_, _) => _hudUi.TextElemDict["EHealth"].Visible = true;
        eMissingHealth.OnLeave += (_, _) => _hudUi.TextElemDict["EHealth"].Visible = false;

        _hudUi.AddSprite("eHealth", Globals.HudAtlas, HpBar(_enemyMonster), eHealthPos, 0.2f, new Vector2(0.8f));
    }

    private void SetResultButtons() {
        Button a = _hudUi.AddButton("Continue", new Vector2(1124, 788), Globals.HudAtlas, Globals.HudDict["EHealth"].Rect);
        a.OnHover += (_, _) => a.Shade = Color.DarkSlateGray;
        a.OnLeave += (_, _) => a.Shade = Color.White;
        a.OnClick += Exit;
        a.Layer = 0.2f;
    }

    private void SetResultText() {
        Vector2 conPos = Vector2.Subtract(new Vector2(1204, 809), Globals.Font.MeasureString("Continue"));
        _hudUi.AddTextElem("Continue", "Continue", conPos, Color.White).Scale = new Vector2(2);

        /* Player monster, Icon, Lvl, Xp, Health, Name */
        Monster[] monsters = _player.BattleMonsters;
        for (int i = 0; i < monsters.Length && monsters[i] != null; i++) {
            Monster monster = monsters[i];

            Vector2 iconPos = Vector2.Subtract(new Vector2(752, 290 + i * 147), new Vector2(100, 19));

            Vector2 healthPos = Vector2.Add(iconPos, new Vector2(180, 40));
            _hudUi.AddTextElem($"PHealth{i}", $"{monster.Health}/{monster.MaxHealth}", healthPos, Color.White).Visible = false;

            string xp = $"{monster.Experience} / {Globals.XpList[monster.Level]}";
            Vector2 xpPos = Vector2.Add(iconPos, new Vector2(180, 80));
            _hudUi.AddTextElem($"PXp{i}", xp, xpPos, Color.White).Visible = false;

            _hudUi.AddTextElem($"PLvl{i}", $"{monster.Level}", iconPos, Color.White).Font = Globals.BoldFont;

            Vector2 namePos = Vector2.Subtract(new Vector2(iconPos.X + 48, iconPos.Y - 20), Globals.Font.MeasureString(monster.Name) / 2);
            _hudUi.AddTextElem($"PName{i}", $"{monster.Name}", namePos, Color.White);
        }

        Vector2 eIconPos = new Vector2(1140, 464);
        Vector2 eNamePos = Vector2.Subtract(eIconPos, new Vector2(-20, 20));
        Vector2 eHealthPos = Vector2.Add(eIconPos, new Vector2(50, 170));

        _hudUi.AddTextElem("EName", _enemyMonster.Name, eNamePos, Color.White);
        _hudUi.AddTextElem("ELevel", $"{_enemyMonster.Level}", eIconPos, Color.White).Font = Globals.BoldFont;
        _hudUi.AddTextElem("EHealth", $"{_enemyMonster.Health}/{_enemyMonster.MaxHealth}", eHealthPos, Color.White).Visible = false;
    }
    static private void Exit(object sender, EventArgs args) {
        AudioManager.StopSong();
        GameplayState gameplayState = Globals.Game.LoadStates.GpState;
        Globals.AudioManager.StartSong("General", "Gameplay", gameplayState.Timespan);
        Globals.DelayedAction(250, () => Globals.Game.SetState(gameplayState));
    }

    private Rectangle HpBar(Monster monster) {
        Rectangle atlasArea = monster == _enemyMonster ? Globals.HudDict["EHealth"].Rect : Globals.HudDict["PHealth"].Rect;
        Point size = new Point((int) ((float) monster.Health / monster.MaxHealth * atlasArea.Width), atlasArea.Height);

        return new Rectangle(new Point(atlasArea.X, atlasArea.Y), size);
    }

    static private Rectangle XpBar(Monster monster) {
        if (monster.Level == 30)
            return new Rectangle(0, 0, 0, 0);

        Rectangle atlasArea = Globals.HudDict["XPBar"].Rect;

        int currXp = monster.Experience - Globals.XpList[monster.Level - 1];
        int lvlXp = Globals.XpList[monster.Level] - Globals.XpList[monster.Level - 1];
        Point xpSize = new Point((int) ((float) currXp / lvlXp * atlasArea.Width), atlasArea.Height);
        
        return new Rectangle(new Point(atlasArea.X, atlasArea.Y), xpSize);
    }

    private void CalcXp() {
        Monster[] battleMonsters = _player.BattleMonsters;

        const int maxLevel = 30;
        const int baseXpGain = 1000;
        float generalXp = baseXpGain * ((float) _enemyMonster.Level / maxLevel);
        float bonus = 1.5f - ((battleMonsters.Length - 1) * 0.25f);
        int maxLvl = battleMonsters[0].Level;
        for (int i = 1; i < battleMonsters.Length && battleMonsters[i] != null; i++)
            maxLvl = Math.Max(maxLvl, battleMonsters[i].Level);

        for (int i = 0; i < battleMonsters.Length && battleMonsters[i] != null; i++) {
            float lvlRatio = 1 - (float) (maxLvl - _enemyMonster.Level) / maxLevel;
            float monsterCountRatio = generalXp / battleMonsters.Length;
            float xpGain = (monsterCountRatio * lvlRatio);
            
            battleMonsters[i].XpGain((int) (xpGain * bonus));
        }
    }

    private void Capture(object sender, EventArgs args) {
        if (!_turn)
            return;

        int rnd = new Random().Next(0, 100);
        float chance = (float) (_enemyMonster.MaxHealth - _enemyMonster.Health) / _enemyMonster.MaxHealth * 100;

        if (rnd <= chance) {
            _player.Inventory.Add(new Monster(_enemyMonster.Name, _enemyMonster.Element, _enemyMonster.Texture, 1)
                                                                        {SpriteEffects = SpriteEffects.FlipHorizontally});
            CalcXp();
            Globals.DelayedAction(500, SetResultHudUi);
        }
        else {
            _turn = false;
            Globals.DelayedAction(1000, EnemyAttack);
        }
    }
    private void EnemyAttack() {
        Skill skill = _enemyLoadOut.Random();
        Animation.Animation anim = _animationManager.Animate($"E{skill.Name}");
        Globals.AudioManager.StartSoundEffect("Skill", skill.Name);
        anim.AnimationEnd = null;
        _turn = true;
        anim.AnimationEnd = (_, _) => {
            _skillAction.Action(skill, false);
            Monster pMonster = _player.BattleMonsters[_pMonsterInd];
            _hudUi.SpriteDict[$"PHealth{_pMonsterInd}"].Rectangle = HpBar(pMonster);
            _hudUi.TextElemDict[$"PHealth{_pMonsterInd}"].Text = $"{pMonster.Health}/{pMonster.MaxHealth}";
            Monster newMonster = null;
            for (int i = 0; i < _player.BattleMonsters.Length && _player.BattleMonsters[i] != null; i++) {
                if (_player.BattleMonsters[i].Health <= 0)
                    continue;
                newMonster = _player.BattleMonsters[i];
                break;
            }
            if (newMonster == null)
                SetResultHudUi();
            else {
                foreach (Button button in _skillUi.BtnDict.Values)
                    button.Shade = Color.White;
                if (pMonster.Health != 0)
                    return;
                _skillAction.PMonster = newMonster;
                _pMonsterInd = Array.IndexOf(_player.BattleMonsters, newMonster);
                SetBattleButtons();
                SetSkillHud();
                _hudUi.RemoveButton("BackArrow");
                _hudUi.RemoveButton("NextArrow");
            }
        };
    }

    public void Update(GameTime gameTime) {
        _hudUi.Update(gameTime);
        _skillUi.Update(gameTime);
        _animationManager.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        _player.BattleMonsters[_pMonsterInd].Draw(gameTime, spriteBatch);
        _enemyMonster.Draw(gameTime, spriteBatch);
        _animationManager.Draw(gameTime, spriteBatch);
        _hudUi.Draw(gameTime, spriteBatch);
        _skillUi.Draw(gameTime, spriteBatch);
    }
}