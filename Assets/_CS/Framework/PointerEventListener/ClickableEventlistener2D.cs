using UnityEngine;
using System.Collections;


public class ClickableEventlistener2D : MonoBehaviour,ClickableSprite
{
	// 定义事件代理
	public delegate void FieldEventProxy(GameObject go,Vector3 pos);

	// 鼠标点击事件
	public event FieldEventProxy ClickEvent;

	//拖动
	public event FieldEventProxy BeginDragEvent;

	public event FieldEventProxy OnDragEvent;

	public event FieldEventProxy EndDragEvent;

	public void onClick(Vector3 pos){
		if (interval > 0)
			return;
		if (ClickEvent != null)
			ClickEvent (this.gameObject,pos);
	}

	public void startDrag(Vector3 pos){
		if (interval > 0)
			return;
		if (BeginDragEvent != null)
			BeginDragEvent (this.gameObject,pos);
	}

	public void onDrag(Vector3 pos){
		if (interval > 0)
			return;
		if (OnDragEvent != null)
			OnDragEvent (this.gameObject,pos);
	}

	public void endDrag(Vector3 pos)
	{
		if (interval > 0)
			return;
		if (EndDragEvent != null) {
			EndDragEvent (this.gameObject, pos);
		}
	}

	public float interval = 0.1f;
	void Update(){
		if (interval > 0) {
			interval -= Time.unscaledDeltaTime;
		}
	}

	public bool hasClickEvent(){
		if (ClickEvent != null) {
			return ClickEvent.GetInvocationList ().Length > 0;
		} else {
			return false;
		}
	}




}

