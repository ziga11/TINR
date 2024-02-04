using System;
using WinApp.Sprites.Character;

namespace WinApp.Sprites.Skill;

public class SkillAction {
    public Monster PMonster;
    private readonly Monster _eMonster;

    public SkillAction(Monster pMonster, Monster eMonster) {
        PMonster = pMonster;
        _eMonster = eMonster;
    }

    public void Action(Skill skill, bool turn) {
        Monster attacker = turn ? PMonster : _eMonster;
        Monster victim = attacker == PMonster ? _eMonster : PMonster;

        victim.Health = Math.Max(victim.Health - CalculateDamage(attacker.Attack, victim.Defense, skill.SkillStats.Att), 0);

        attacker.Attack += skill.SkillStats.AttUp;
        attacker.Defense += skill.SkillStats.DefUp;
        attacker.Evasion += skill.SkillStats.EvsUp;

        victim.Attack = Math.Max(victim.Attack - (int) (0.3 * skill.SkillStats.AttDown), 0);
        victim.Defense = Math.Max(victim.Defense - (int) (0.3 * skill.SkillStats.DefDown), 0);
        victim.Evasion = Math.Max(victim.Evasion - (int) (0.3 * skill.SkillStats.EvsDown), 0);
    }

    private int CalculateDamage(int attackerAttack, int defenderDefense, int skillAttack) {
        if (skillAttack == 0)
            return 0;

        int baseDamage = attackerAttack + skillAttack;

        int damageAfterDefense = (int) ((baseDamage - defenderDefense) * StrengthRatio());

        return Math.Max(damageAfterDefense, 0);
    }

    private float StrengthRatio() {
        string vElem = _eMonster.Element;
        string aElem = PMonster.Element;

        switch (vElem) {
            case "Fire" when aElem == "Water":
            case "Nature" when aElem == "Fire":
            case "Water" when aElem == "Nature":
                return 1.25f;
            case "Water" when aElem == "Fire":
            case "Fire" when aElem == "Nature":
            case "Nature" when aElem == "Water":
                return 0.75f;
            default:
                return 1f;
        }
    }
}