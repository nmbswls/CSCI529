using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "SkillAsset2", menuName = "Ctm/SkillAsset2")]
[System.Serializable]
public class SkillAsset2 : ScriptableObject
{
    public string SkillId;

    public string SkillName;

    [TextArea(3, 10)]
    public string SkingDesp;

    [TextArea(3, 10)]
    public string SkingEffectDesp;

    public int Branch;

    public int Level;

    public SkillReq Requirements;

    public SkillReward Rewards;
}

[CreateAssetMenu(fileName = "SkillAsset2List", menuName = "Ctm/SkillAsset2List")]
[System.Serializable]
public class SkillAsset2List : ScriptableObject
{
    public List<SkillAsset2> SkillBranch;
}

[CreateAssetMenu(fileName = "SkillAsset2Collection", menuName = "Ctm/SkillAsset2Collection")]
[System.Serializable]
public class SkillAsset2Collection : ScriptableObject
{
    public List<SkillAsset2List> SkillCollection;
}