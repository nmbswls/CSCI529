using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "SkillAsset2Collection", menuName = "Ctm/SkillAsset2Collection")]
[System.Serializable]
public class SkillAsset2Collection : ScriptableObject
{
    public List<SkillAsset2List> SkillCollection;
}