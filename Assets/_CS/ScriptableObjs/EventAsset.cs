using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName="new event",menuName="Ctm/event")]
[System.Serializable]
public class EventAsset : ScriptableObject
{

    public string EventId;
    public string TriggerCondString;
    public List<string> Actions = new List<string>();
    public bool OneTime;
}



