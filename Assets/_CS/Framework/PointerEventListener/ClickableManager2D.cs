#if UNITY_ANDROID && !UNITY_EDITOR
#define ANDROID
#endif



#if UNITY_IPHONE && !UNITY_EDITOR
#define IPHONE
#endif
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickableManager2D : MonoBehaviour
{

	public Camera m_camera;
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		checkTouch ();
	}

	public static float dragYuzhi = 0.1f;
	//坐标均为屏幕坐标系
	Vector3 mouseDownPos;
	Vector3 lastDragPos;
	GameObject nowClickGO;

	enum MouseState{
		NONE,
		CLICK,
		DRAG,
	}
	MouseState nowMode = MouseState.NONE;


	GameObject[] clickedObjs;

	public void checkTouch(){

		//		if (!Stage.isTouchOnUI)
		//		{
		//			RaycastHit hit;
		//			Ray ray = Camera.main.ScreenPointToRay(new Vector2(Stage.inst.touchPosition.x, Screen.height - Stage.inst.touchPosition.y));
		//			if (Physics.Raycast(ray, out hit))
		//			{
		//				if (hit.transform == cube)
		//				{
		//					Debug.Log("Hit the cube");
		//				}
		//			}
		//		}
		if (Input.GetMouseButtonDown(0)||(Input.touchCount >0 && Input.GetTouch(0).phase == TouchPhase.Began))
		{
			#if IPHONE || ANDROID
			if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
			#else
			//Debug.Log(Stage.inst.touchTarget.gameObject.name);	
			if (EventSystem.current.IsPointerOverGameObject ()){
			#endif
				;} 
			else {
				if (nowMode == MouseState.NONE) {
					Vector3 pos = m_camera.ScreenToWorldPoint (Input.mousePosition);
					RaycastHit[] hits = null;
					hits = Physics.RaycastAll (pos, Vector3.forward, Mathf.Infinity);
                    int minDis = int.MaxValue;
					if (hits.Length > 0) {    //检测是否射线接触物体
						mouseDownPos = Input.mousePosition;
						nowClickGO = hits [0].collider.gameObject;
						nowMode = MouseState.CLICK;
					}
					clickedObjs = new GameObject[hits.Length];
					for (int i = 0; i < hits.Length; i++) {
						clickedObjs [i] = hits [i].collider.gameObject;
					}
				}

			}
		}

		if (Input.GetMouseButton (0)) {
			Vector3 mouseMove = (Input.mousePosition - mouseDownPos);
			mouseMove.z = 0;
			if (nowMode == MouseState.NONE) {

			}
			if (nowMode == MouseState.CLICK) {
				if (mouseMove.magnitude < dragYuzhi) {
					//仍是点击
				} else {
					//超过阀值 变为拖动
					nowMode = MouseState.DRAG;
					lastDragPos = Input.mousePosition;
					if (nowClickGO == null || nowClickGO.GetComponentInParent<ClickableEventlistener2D> () == null) {
						//无法回调
					} else {
						nowClickGO.GetComponentInParent<ClickableSprite> ().startDrag (lastDragPos);
					}

				}
			} else if (nowMode == MouseState.DRAG) {
				Vector3 nowPos = Input.mousePosition;
				Vector3 delta = nowPos - lastDragPos;
				if (nowClickGO == null || nowClickGO.GetComponentInParent<ClickableSprite> () == null) {
					//无法回调
				} else {
					nowClickGO.GetComponentInParent<ClickableSprite> ().onDrag(delta);
				}
				lastDragPos = nowPos;
			}
		}



		if (Input.GetMouseButtonUp (0)) {
			if (nowMode == MouseState.NONE) {

			} else if (nowMode == MouseState.CLICK) {
				if (nowClickGO == null || nowClickGO.GetComponentInParent<ClickableSprite> () == null) {
					//无法回调

				} else {
                    ClickableEventlistener2D cp = nowClickGO.GetComponentInParent<ClickableEventlistener2D>();

                    if (cp == null || !cp.hasClickEvent()) {
						int idx = 1;
						if (clickedObjs != null) {
							while (idx < clickedObjs.Length) {
								nowClickGO = clickedObjs [idx];
                                cp = nowClickGO.GetComponentInParent<ClickableEventlistener2D>();
                                if (cp!=null && cp.hasClickEvent()) {
                                    cp.onClick (Input.mousePosition);
									break;
								}
								idx++;
							}
						}
					} else {
						cp.onClick (Input.mousePosition);
					}
				}
			} else if (nowMode == MouseState.DRAG) {
				if (nowClickGO == null || nowClickGO.GetComponentInParent<ClickableSprite> () == null) {
					//无法回调
				} else {
					nowClickGO.GetComponentInParent<ClickableSprite> ().endDrag (Input.mousePosition);
				}
			}
			nowMode = MouseState.NONE;
		}
	}
}



