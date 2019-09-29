using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialEvent{

	public string EventId;
	public LogicNode TriggerCond;
	public string action;
}

public class SpeEventMgr : ModuleBase, ISpeEventMgr
{

	public Dictionary<string, SpecialEvent> ListenDict = new Dictionary<string, SpecialEvent>();


	public override void Setup(){
		
	}

	public List<string> CheckEvent(){

		List<string> ret = new List<string> ();

		foreach(var kv in ListenDict){
			bool trigger = kv.Value.TriggerCond.Check ();
			if (trigger) {
				ret.Add (kv.Key);
			}
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
		foreach (string eventId in ret) {
			EventAsset eventAsset = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<EventAsset> ("events/" + eventId);
			ListenDict [eventId] = new SpecialEvent ();
		}
	}


}

