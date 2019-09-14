using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PointEventListener : MonoBehaviour, IPointerClickHandler
{
    public delegate void OnClickDlg(PointerEventData eventData);
    public event OnClickDlg OnClickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickEvent != null)
        {
            OnClickEvent(eventData);
        }
    }
}
