using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public class ClickEventListerner :  MonoBehaviour, IPointerClickHandler
{
	

	public delegate void OnClickDlg(PointerEventData eventData);
	public event OnClickDlg OnClickEvent;
	// Use this for initialization
	public void OnPointerClick(PointerEventData eventData)
	{
		if (OnClickEvent != null)
		{
			OnClickEvent(eventData);
		}

	}

	public void ClearClickEvent(){
		if (OnClickEvent != null) {
			Delegate[] invokeList = OnClickEvent.GetInvocationList ();
			if (invokeList != null)
			{
				foreach (Delegate del in invokeList)
				{
					OnClickEvent -= (OnClickDlg)del;
				}
			}
		}

	}
}

