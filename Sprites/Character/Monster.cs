using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.Sprites.Character;

public class MonsterAttr {
    public Dictionary<string, Vec2> Positions { get; set; }
    public Dictionary<string, string> Skills { get; set; }

    public List<Skill.Skill> GetSkills() {
        List<Skill.Skill> skillsWithIndex = Skills.Select(kv =>
                int.TryParse(kv.Key, out int index) ? new { Index = index, SkillName = kv.Value } : null)
                .Where(x => x != null && Globals.SkillDict.ContainsKey(x.SkillName))
                .Select(x => new { Skill = Globals.SkillDict[x.SkillName], x.Index })
                .OrderBy(x => x.Index)
                .Select(x => x.Skill)
                .ToList();

        return skillsWithIndex;
    }

    public Point Pos(string ind) => new Point(Positions[ind].X, Positions[ind].Y);

}

public class Monster : Sprite {
    public readonly string Name;
    public readonly string Element;
    public int Level;
    public int MaxHealth;
    public int Health;
    public int Attack;
    public int Defense;
    public int Evasion;
    public int Experience;

    public readonly List<Skill.Skill> Skills;
    private readonly Sprite[] _evolutions;

    public Monster(string name, string elem, Texture2D texture, int lvl) : base(texture) {
        Name = name;
        Element = elem;
        Texture = texture;
        Level = lvl;
        Experience = Globals.XpList[Level - 1];
        Attack = 15 * (int) Math.Pow(1.05, Level - 1);
        Defense = 10 * (int) Math.Pow(1.05, Level - 1);
        MaxHealth = 40 * (int) Math.Pow(1.05, Level - 1);
        Health = MaxHealth;
        
        MonsterAttr attr = Globals.MonsterDict[elem][name];
        Skills = attr.GetSkills();

        _evolutions = new[] {
            new Sprite(Texture, new Rectangle(attr.Pos("0"), new Point(384)), Vector2.Zero),
            new Sprite(Texture, new Rectangle(attr.Pos("1"), new Point(384)), Vector2.Zero),
            new Sprite(Texture, new Rectangle(attr.Pos("2"), new Point(384)), Vector2.Zero),
            new Sprite(Texture, new Rectangle(attr.Pos("3"), new Point(384)), Vector2.Zero)
        };
    }

    private void LevelUp() {
        Level++;
        Attack = (int) (1.05 * Attack);
        Defense = (int) (1.05 * Defense);
        MaxHealth = (int) (1.05 * MaxHealth);
        Health = MaxHealth;
    }

    public void XpGain(int xp) {
        while (xp > 0) {
            int neededXp = Globals.XpList[Level] - Experience;
            int xpGain = Math.Min(neededXp, xp);
            xp -= xpGain;
            Experience += xpGain;
            if (xpGain == neededXp)
                LevelUp();
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (_evolutions[Level / 10] == null)
            return;
        spriteBatch.Draw(Texture, Position, _evolutions[Level / 10].Rectangle, Color.White, 0f, Origin, 1f,
            SpriteEffects, Layer);
    }
}