using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialEvent
{

    public string EventId;
    public LogicNode TriggerCond;
    public List<string> actions = new List<string>();
    public bool OneTime;


    public SpecialEvent(string EventId, LogicNode TriggerCond, List<string> actions)
    {
        this.EventId = EventId;
        this.TriggerCond = TriggerCond;
        this.actions = actions;
    }
}

public class EventGroup
{

}


public class SpeEventMgr : ModuleBase, ISpeEventMgr
{

	public Dictionary<string, SpecialEvent> EventDict = new Dictionary<string, SpecialEvent>();
    public HashSet<string> ListenEvents = new HashSet<string>();

    private ILogicTree pLogidTree;

	public override void Setup(){

        pLogidTree = GameMain.GetInstance().GetModule<LogicTree>();
        LoadEventTable("s");
    }

    public void RemoveListener(string eventId)
    {
        if (!ListenEvents.Contains(eventId))
        {
            return;
        }
        ListenEvents.Remove(eventId);
    }

    public void AddListener(string eventId)
    {
        if (ListenEvents.Contains(eventId))
        {
            return;
        }
        ListenEvents.Add(eventId);
    }


    public List<SpecialEvent> CheckEvent()
    {
        List<SpecialEvent> ret = new List<SpecialEvent>();

        foreach (var k in ListenEvents)
        {
            string eid = k;
            SpecialEvent se = GetEvent(eid);
            bool trigger = se.TriggerCond.Check();
            if (trigger)
            {
                ret.Add(se);
            }
        }
        return ret;
    }


    private SpecialEvent GetEvent(string eventId)
    {
        SpecialEvent ret = null;
        if(!EventDict.TryGetValue(eventId,out ret))
        {
            EventAsset eventAsset = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<EventAsset>("events/" + eventId);
            string eid = eventAsset.EventId;
            LogicNode node = pLogidTree.ConstructFromString(eventAsset.TriggerCondString);
            List<string> actions = new List<string>(eventAsset.Actions);
            ret = new SpecialEvent(eid,node,actions);
            EventDict.Add(eventId, ret);
        }

        return ret;
    }


    public void LoadEventTable(string roleId){
		TextAsset eventTable = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<TextAsset> ("EventTable");
		string[] src = new string[]{"all:2,3,4,5,6"};
		List<string> ret = new List<string> ();
		foreach (string line in src) {
			string type = line.Substring (0,line.IndexOf(":"));
			if (type == "all" || type == roleId) {
				string[] events = line.Substring (line.IndexOf (":") + 1).Trim ().Split (',');
				foreach (string ss in events) {
					ret.Add (ss);
				}
			}
		}
        //foreach (string eventId in ret) {
        //	EventAsset eventAsset = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<EventAsset> ("events/" + eventId);
        //	ListenDict [eventId] = new SpecialEvent ();
        //}
        {
            GetEvent("e00");

        }

        {
            GetEvent("e01");
        }

        //EventGroup.choose(3);

        ListenEvents.Add("e00");

    }

    private void SetupInitListener()
    {

    }

}

