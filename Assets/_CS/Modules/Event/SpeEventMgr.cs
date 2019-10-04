using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialEvent
{

    public string EventId;
    public LogicNode TriggerCond;
    public string action;
    public bool OneTime;

    public SpecialEvent()
    {

    }

    public SpecialEvent(string EventId, LogicNode TriggerCond, string action)
    {
        this.EventId = EventId;
        this.TriggerCond = TriggerCond;
        this.action = action;
    }
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
            ret = new SpecialEvent();
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
            string eid = "e0";
            LogicNode node = pLogidTree.ConstructFromString("True()&!False()");
            string action = "d0";
            EventDict["e0"] = new SpecialEvent(eid, node, action);
        }

        {
            string eid = "e1";
            LogicNode node = pLogidTree.ConstructFromString("True()&!False()");
            string action = "d1";
            EventDict["e1"] = new SpecialEvent(eid, node, action);
        }

        ListenEvents.Add("e0");

    }

    private void SetupInitListener()
    {

    }

}

