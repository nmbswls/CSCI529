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

    public bool isReward = false;
}