#if UNITY_ANDROID && !UNITY_EDITOR
#define ANDROID
#endif



#if UNITY_IPHONE && !UNITY_EDITOR
#define IPHONE
#endif
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickableManager2D : MonoBehaviour
{
    public static eFieldEventResult FieldEventResult;
    public Camera m_camera;
	// Use this for initialization
	void Start ()
	{

	}

    public static void BindClickEvent(GameObject target, ClickableEventlistener2D.FieldEventProxy Func)
    {
        ClickableEventlistener2D listener = target.GetComponent<ClickableEventlistener2D>();
        if (listener == null)
        {
            listener = target.AddComponent<ClickableEventlistener2D>();
            listener.ClickEvent += Func;
        }
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
    List<GameObject> nowClickedList;
	//GameObject nowClickGO;

	enum MouseState{
		NONE,
		CLICK,
		DRAG,
	}
	MouseState nowMode = MouseState.NONE;


	//GameObject[] clickedObjs;

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
					if (hits.Length > 1) {    //检测是否射线接触物体
						mouseDownPos = Input.mousePosition;
                        nowClickedList = new List<GameObject>();
                        List<RaycastHit> hitsList = new List<RaycastHit>(hits);

                        hitsList.Sort((x,y) => {
                            return x.distance.CompareTo(y.distance);
                        });
                        for (int i=0;i< hits.Length; i++)
                        {
                            nowClickedList.Add(hits[i].collider.gameObject);
                        }
                        //nowClickGO = hits [0].collider.gameObject;
						nowMode = MouseState.CLICK;
					}else if (hits.Length > 0)
                    {
                        mouseDownPos = Input.mousePosition;
                        nowClickedList = new List<GameObject>() { hits[0].collider.gameObject};
                        nowMode = MouseState.CLICK;
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
                    
					if (nowClickedList == null || nowClickedList.Count == 0|| nowClickedList[0].GetComponentInParent<ClickableEventlistener2D> () == null) {
						//无法回调
					} else {
                        nowClickedList[0].GetComponentInParent<ClickableSprite> ().startDrag (lastDragPos);
					}

				}
			} else if (nowMode == MouseState.DRAG) {
				Vector3 nowPos = Input.mousePosition;
				Vector3 delta = nowPos - lastDragPos;
				if (nowClickedList == null || nowClickedList.Count == 0 || nowClickedList[0].GetComponentInParent<ClickableSprite> () == null) {
					//无法回调
				} else {
                    nowClickedList[0].GetComponentInParent<ClickableSprite> ().onDrag(delta);
				}
				lastDragPos = nowPos;
			}
		}



		if (Input.GetMouseButtonUp (0)) {
			if (nowMode == MouseState.NONE) {

			} else if (nowMode == MouseState.CLICK) {
				if (nowClickedList == null || nowClickedList.Count == 0  || nowClickedList[0].GetComponentInParent<ClickableSprite> () == null) {
					//无法回调

				} else {
                    
                    {
                        for (int i = 0; i < nowClickedList.Count; i++)
                        {
                            ClickableEventlistener2D cp = nowClickedList[i].GetComponentInParent<ClickableEventlistener2D>();
                            if (cp == null || !cp.hasClickEvent())
                            {
                                continue;
                            }
                            if (cp != null && cp.hasClickEvent())
                            {
                                cp.onClick(Input.mousePosition);
                                if(FieldEventResult == eFieldEventResult.Block)
                                {
                                    break;
                                }
                            }
                        }

                    }
                    FieldEventResult = eFieldEventResult.Block;


                }
			} else if (nowMode == MouseState.DRAG) {
				if (nowClickedList == null || nowClickedList.Count == 0|| nowClickedList[0].GetComponentInParent<ClickableSprite> () == null) {
					//无法回调
				} else {
                    nowClickedList[0].GetComponentInParent<ClickableSprite> ().endDrag (Input.mousePosition);
				}
			}
			nowMode = MouseState.NONE;
		}
	}
}



