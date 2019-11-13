using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EmergencyChoice
{
    public string Content;
    public string NextEmId;
    public string Ret;
}

[CreateAssetMenu(fileName = "emergency", menuName = "Ctm/emergency")]
[System.Serializable]
public class EmergencyAsset : ScriptableObject
{
    public string EmId;

    public string EmName;

    [TextArea(3, 10)]
    public string EmDesp;


    public List<EmergencyChoice> Choices = new List<EmergencyChoice>();
}
