using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEventMap
{
    protected readonly Dictionary<int, GameEventDelegate> mMapEventDlg = new Dictionary<int, GameEventDelegate>();

    public Dictionary<int, GameEventDelegate> GetEventMap()
    {
        return mMapEventDlg;
    }

    public void RegisterEvent(GameEventType type, int id, GameEventDelegate handler)
    {
        int key = GetEventKey(type, id);
        if (!mMapEventDlg.ContainsKey(key))
        {
            mMapEventDlg[key] = handler;
        }
        else
        {
            Debug.Log("repeat register event");
        }
    }

    protected int GetEventKey(GameEventType type, int id)
    {
        return id;
    }

    public GameEventDelegate GetEventDlgByKey(int key)
    {
        return mMapEventDlg[key];
    }

}