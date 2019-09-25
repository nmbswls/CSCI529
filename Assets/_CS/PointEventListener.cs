using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PointEventListener : MonoBehaviour, IPointerClickHandler,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public delegate void OnClickDlg(PointerEventData eventData);
    public event OnClickDlg OnClickEvent;

    public delegate void OnDragDlg(PointerEventData eventData);
    public event OnDragDlg OnDragEvent;

    public delegate void OnBeginDragDlg(PointerEventData eventData);
    public event OnBeginDragDlg OnBeginDragEvent;

    public delegate void OnEndDragDlg(PointerEventData eventData);
    public event OnEndDragDlg OnEndDragEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickEvent != null)
        {
            OnClickEvent(eventData);
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragEvent != null)
        {
            OnDragEvent(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragEvent != null)
        {
            OnBeginDragEvent(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragEvent != null)
        {
            OnEndDragEvent(eventData);
        }
    }
}
