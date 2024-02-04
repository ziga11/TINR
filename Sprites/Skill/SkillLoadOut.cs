using System;
using System.Collections.Generic;

namespace WinApp.Sprites.Skill;

public class SkillLoadOut {
    public readonly List<Skill> Skills;
    public readonly List<Skill> ShownSkills = new List<Skill>();

    public SkillLoadOut(List<Skill> skills) {
        Skills = skills;
        Setup();
    }

    private void Setup() {
        int firstInd = Math.Max(Skills.Count - 4, 0);
        for (int i = firstInd; i < Skills.Count; i++)
            ShownSkills.Add(Skills[i]);
    }
    public int LeftArrow() {
        int newSkillInd = Skills.IndexOf(ShownSkills[0]) - 1;
        ShownSkills.RemoveAt(3);
        ShownSkills.Insert(0, Skills[newSkillInd]);
        return newSkillInd;
    }
    public int RightArrow() {
        int newSkillInd = Skills.IndexOf(ShownSkills[3]) + 1;
        ShownSkills.RemoveAt(0);
        ShownSkills.Add(Skills[newSkillInd]);
        return newSkillInd;
    }

    public Skill Random() {
        int randomSkill = new Random().Next(0, Skills.Count);

        return Skills[randomSkill];
    }
}