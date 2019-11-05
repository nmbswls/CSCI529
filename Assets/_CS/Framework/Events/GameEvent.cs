using UnityEngine;
using System.Collections;


public class GameEvent
{
    public int mEventId;
    public GameEventType mEventType;

    public object mObj1;


    public GameEvent(int id, GameEventType type)
    {
        this.mEventId = id;
        this.mEventType = type;
    }

    public int GetEventKey()
    {
        return mEventId;
    }


}
