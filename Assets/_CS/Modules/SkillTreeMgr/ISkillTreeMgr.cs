using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISkillTreeMgr : IModule
{
    SkillAsset GetSkillAsset(string skillId);

    SkillInfo GetOwnedSkill(string SkillId);

    void GainSkills(string skillId);

    List<string> GetSkillByType(eSkillType type);

    void GainExp(string sid);

    void PrintSkills();
}
