using Microsoft.Xna.Framework;

namespace WinApp.Sprites.Skill;

public class SkillTex : TexObject {
    public int Type { get; set; }
    public int Count { get; set; }
    public Vec2 NextItr { get; set; }

    public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);
}

public class SkillStats {
    public int Att { get; set; }
    public int AttUp { get; set; }
    public int AttDown { get; set; }
    public int DefUp { get; set; }
    public int DefDown { get; set; }
    public int EvsUp { get; set; }
    public int EvsDown { get; set; }
}

public class Skill {
    public string Name { get; set; }
    public string Element { get; set; }
    public SkillTex SkillTex { get; set; }

    public SkillStats SkillStats { get; set; }
}