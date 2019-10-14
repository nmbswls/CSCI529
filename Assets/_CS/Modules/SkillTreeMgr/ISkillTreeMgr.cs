using UnityEngine;
using System.Collections;

public interface ISkillTreeMgr : IModule
{
    SkillAsset GetSkillAsset(string skillId);

    SkillInfo GetOwnedSkill(string SkillId);

    void GainSkills(string skillId);
}
