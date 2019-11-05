using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate int GameEventDelegate(GameEvent e);

public class EventDelegateExecutor
{
    public struct GameEventTypeComparer : IEqualityComparer<GameEventType>
    {
        public bool Equals(GameEventType x, GameEventType y)
        {
            return x == y;
        }
        public int GetHashCode(GameEventType obj)
        {
            return (int)obj;
        }
    }
    //放入之后 才会捕获错误 否则直接跑。。？？？
    public HashSet<GameEventType> mTryCatchEventTypes = new HashSet<GameEventType>(new GameEventTypeComparer())
    {
        GameEventType.TestEvent
    };

    public int Execute(GameEventDelegate dlg, GameEvent e)
    {
        int ret = (int)EventRet.Continue;
        if (dlg != null && e != null)
        {
            if (!mTryCatchEventTypes.Contains(e.mEventType))
            {
                ret = dlg(e);
            }
            else
            {
                try
                {
                    ret = dlg(e);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        return ret;
    }
}




public class EventMgr : IEventMgr
{
    public class ModuleEventEntry
    {
        public GameEventDelegate callback;
        public IEventListener module;
    }
    private readonly Dictionary<int, List<ModuleEventEntry>> mEventKeyMap = new Dictionary<int, List<ModuleEventEntry>>();
    private EventDelegateExecutor mEventDlgExecutor = new EventDelegateExecutor();


    public void RegisterModuleEvent(IEventListener eventListener)
    {
        eventListener.RegisterEvent();
        GameEventMap eMap = eventListener.GetEventMap();
        if (eMap == null)
        {
            return;
        }
        foreach (int key in eMap.GetEventMap().Keys)
        {
            if (!mEventKeyMap.ContainsKey(key))
            {
                mEventKeyMap[key] = new List<ModuleEventEntry>();

            }
            ModuleEventEntry eventEntry = new ModuleEventEntry();
            eventEntry.callback = eMap.GetEventDlgByKey(key);
            eventEntry.module = eventListener;
            mEventKeyMap[key].Add(eventEntry);
        }
    }


    public void SendGlobalEvent(GameEvent e)
    {
        int key = e.GetEventKey();
        if (mEventKeyMap.ContainsKey(key))
        {
            List<ModuleEventEntry> eList = mEventKeyMap[key];
            if (eList == null)
            {
                return;
            }
            for (int i = 0; i < eList.Count; i++)
            {
                ModuleEventEntry eventEntry = eList[i];
                if (eventEntry == null || eventEntry.callback == null)
                {
                    continue;
                }
                int ret = mEventDlgExecutor.Execute(eventEntry.callback, e);
                if (ret == (int)EventRet.End)
                {
                    break;
                }
            }
        }
    }
}
