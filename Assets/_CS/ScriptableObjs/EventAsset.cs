using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName="new event",menuName="Ctm/event")]
[System.Serializable]
public class EventAsset : ScriptableObject
{

    public string EventId;
    public string TriggerCondString;
    public string ActionString;
    public bool OneTime;
}



