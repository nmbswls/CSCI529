using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "SkillAsset2List", menuName = "Ctm/SkillAsset2List")]
[System.Serializable]
public class SkillAsset2List : ScriptableObject
{
    public List<SkillAsset2> SkillBranch;
}
