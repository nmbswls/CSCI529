using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class TabGroupChildView{
	public Transform root;
	public virtual void BindView(){
		
	}
}

public class TabGroup : MonoBehaviour
{

	int nowTab;
	public List<TabGroupChildView> tabs = new List<TabGroupChildView> ();

	public delegate void OnValueChangeDlg(int newTab);
	public event OnValueChangeDlg OnValueChangeEvent;

	Type TabType;

	public void InitTab(Type tabType){

		nowTab = -1;
		this.TabType = tabType;
		BindView ();
		RegisterEvent ();

	}

	public void BindView(){
		foreach (Transform child in transform) {
			TabGroupChildView v = (TabGroupChildView)Activator.CreateInstance(TabType);
			if (v == null) {
				Debug.Log ("tab type error must extend tab group child");
				break;
			}
			v.root = child;
			v.BindView ();
			tabs.Add (v);
		}
	}

	public void RegisterEvent(){
		for (int i = 0; i < tabs.Count; i++) {
			int index = i;
			ClickEventListerner listener = tabs[i].root.gameObject.GetComponent<ClickEventListerner> ();
			if(listener == null){
				listener = tabs[i].root.gameObject.AddComponent<ClickEventListerner> ();
				listener.OnClickEvent += delegate(PointerEventData eventData) {
					switchTab(index);
				};
			}
		}
	}


	public void switchTab(int newTab,bool force=false){

		if (newTab < 0 || newTab >= tabs.Count) {
			return;
		}
		if (!force && (newTab == nowTab))
			return;

		nowTab = newTab;
		if (OnValueChangeEvent != null) {
			OnValueChangeEvent (nowTab);
		}

	}

}

