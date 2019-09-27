using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PointerEventListernerNoDrag :  MonoBehaviour, IPointerClickHandler
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
}

